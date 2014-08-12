sw = {};

// #####################   C o m m o n    ##########################


sw.inherits = function(child, parent) {
    var f = function() {};
    f.prototype = parent.prototype;
    child.prototype = new f();
    child.prototype.constructor = child;

    // `child` function is an object like all functions    
    child.superClass_ = parent.prototype;
};

sw.attachVerticalScroll = function(callback) {
    $(window).scroll(function(evt) {
        evt.preventDefault();

        if ($(window).scrollTop() >=
            $(document).height() - $(window).height()) {
            callback();
        }
    });
};

sw.ajaxJsonRequest = function(ajData, url, onSuccess, onError, busyObject) {
    var self = this;

    var config = {
        async: true,
        url: url,
        type: "POST",
        data: JSON.stringify(ajData),
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
    };

    if (busyObject && busyObject.isBusy) busyObject.isBusy(true);

    var request = $.ajax(config)
        .done(function(response, statusText, xhr) {
            if (onSuccess) onSuccess.call(self, response, statusText, xhr);
        })
        .fail(function(response, statusText, xhr) {
            if (onError) onError.call(self, response, statusText, xhr);
        })
        .always(function() {
            if (busyObject && busyObject.isBusy) busyObject.isBusy(false);
        });
    
    return request;
};

sw.convertToLocal = function(date) {
    var result = date
        ? new Date(date.getTime() + (date.getTimezoneOffset() * 60000))
        : date;
    return result;
};

sw.convertToUTC = function(date) {
    var result = date
        ? new Date(date.getTime() - (date.getTimezoneOffset() * 60000))
        : date;
    return result;
};

sw.scaleImages = function(elements) {
    $(elements).find("img.scale").imageScale();
};

sw.setCSSRule = function (selector, property, value) {
    for (var i = 0; i < document.styleSheets.length; i++) {
        var ss = document.styleSheets[i];
        var rules = (ss.cssRules || ss.rules);
        var sel = selector.toLowerCase();

        for (var j = 0, len = rules.length; j < len; j++) {
            if (rules[j].selectorText && (rules[j].selectorText.toLowerCase() == sel)) {
                if (value != null) {
                    rules[j].style[property] = value;
                    return;
                } else {
                    if (ss.deleteRule) {
                        ss.deleteRule(j);
                    } else if (ss.removeRule) {
                        ss.removeRule(j);
                    } else {
                        rules[j].style.cssText = "";
                    }
                }
            }
        }
    }
};

sw.fitThumbs = function(container, thumbSel, defaultWidth) {
    var containerWidth = $(container).width();
    var itemsCount = Math.max(Math.round(containerWidth / defaultWidth), 1);
    sw.setCSSRule(thumbSel, "width", (containerWidth / itemsCount) + "px");
};

// #########    B i n d i n g    H a n d l e r s     ################


ko.bindingHandlers.fadeIn = {
    init: function (element, valueAccessor) {
        var duration = ko.utils.unwrapObservable(valueAccessor());
        $(element).hide().fadeIn(duration);
    }
};

ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value) || false);
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).fadeIn() : $(element).fadeOut();
    }
};

ko.bindingHandlers.fadeScrollVisible = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value) || false);
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (ko.unwrap(value)) {
            $(element).fadeIn();
            ko.bindingHandlers.scroll.scrollTop(element, "slow");
        } else {
            $(element).fadeOut();
        }
    }
};

ko.bindingHandlers.scroll = {
    init: function (element, valueAccessor) {
        var duration = ko.utils.unwrapObservable(valueAccessor());
        ko.bindingHandlers.scroll.scrollTop(element, duration);
    }
};

ko.bindingHandlers.scroll.scrollTop = function (element, duration) {
    var topMargin = 80;

    var parentDialog = $(element).closest(".ui-dialog-content");
    if (parentDialog.length > 0) {
        parentDialog.animate({
            scrollTop: $(element).offset().top - $(parentDialog).offset().top +
                $(parentDialog).scrollTop() - topMargin
        }, duration);
    }
    else if (!$(element).visible(false, false, "vertical")) {
        $("html, body").animate({
            scrollTop: $(element).offset().top - topMargin
        }, duration);
    }
};


// ###############    V i e w    M o d e l s     ####################

ViewModelBase = function () {
    var self = this;

    self.currentRequest = null;

    self.isBusy = ko.observable(false);
    self.isEnabled = ko.computed(function () { return !self.isBusy(); });

    self.isBusy.subscribe(function (isBusy) {
        if (!isBusy && self.currentRequest) {
            self.currentRequest.abort();
            self.currentRequest = undefined;
        }
    });

    self.serverValidErrors = ko.observableArray();
    self.serverError = ko.observable();

    self.handleServerError = function (errorResult) {
        self.serverValidErrors(
            errorResult.responseJSON ?
                errorResult.responseJSON.ValidationErrors
                : undefined);

        if (!errorResult.responseJSON) {
            if (errorResult.statusText) {
                if (errorResult.statusText.toLowerCase() != "abort") {
                    self.serverError(errorResult.statusText);
                }
            } else {
                self.serverError("There was an error. Please try again.");
            }
        }
    };

    self.resetServerErrors = function () {
        self.serverValidErrors(undefined);
        self.serverError(undefined);
    };
};

ListViewModel = function (parameters, url) {
    var self = this;
    
    ListViewModel.superClass_.constructor.call(self);
    
    self._currentPage = 0;
    self._finished = false;
   
    self.query = ko.observable();
    self.items = ko.observableArray();

    self.addItem = function () { }; // abstract ;-)

    self.getData = function (pageNumber) {
        if (!self._finished && !self.isBusy() &&
            self._currentPage != pageNumber) {
            sw.ajaxJsonRequest(
                {
                    pageNumber: pageNumber,
                    query: self.query(),
                    parameters: parameters
                },
                url,
                function (items) {
                    if (items && items.length > 0) {
                        self._currentPage = self._currentPage + 1;
                        items.forEach(function(item) { self.addItem(item); });
                    } else {
                        self._finished = true;
                    }
                },
                function (errorResult) {
                    self.handleServerError(errorResult);
                },
                self
            );
        }
    };

    self.getNextPage = function () {
        return self.getData(self._currentPage + 1);
    };

    self.search = function () {
        self.items.removeAll();
        self._currentPage = -1;
        self.getNextPage();
    };
};

sw.inherits(ListViewModel, ViewModelBase);

ContactType = {
    Email: 0,
    Url: 1,
    Phone: 2
};

function ContactViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.type = ko.observable();
    self.title = ko.observable();
    self.contact = ko.observable();

    self.loadData = function (contactData) {
        self.id(contactData.Id);
        self.type(contactData.Type);
        self.title(contactData.Title);
        self.contact(contactData.Contact);
    };

    self.loadData(data);

    self.toJSON = function () {
        var json = {
            Id: self.id(),
            Type: self.type(),
            Title: self.title(),
            Contact: self.contact(),
            Destroy: self._destroy
        };
        return json;
    };
}

function AddressViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.address = ko.observable();
    self.tip = ko.observable();
    self.latitude = ko.observable();
    self.longitude = ko.observable();

    self.loadData = function (addressData) {
        self.id(addressData.Id);
        self.address(addressData.Address);
        self.tip(addressData.Tip);
        self.latitude(addressData.Latitude);
        self.longitude(addressData.Longitude);
    };

    self.loadData(data);

    self.toJSON = function () {
        var json = {
            Id: self.id(),
            Address: self.address(),
            Tip: self.tip(),
            Latitude: self.latitude(),
            Longitude: self.longitude(),
            Destroy: self._destroy
        };
        return json;
    };
}

function EventViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.title = ko.observable();
    self.startDate = ko.observable();
    self.endDate = ko.observable();
    self.isPublic = ko.observable();
    self.picture = ko.observable();
    self.combineType = ko.observable(data.combineType);
    self.description = ko.observable(data.description);
    /*self.latitude = ko.observable(data.latitude);
    self.longitude = ko.observable(data.longitude);*/
    
    self.host = ko.observable();
    self.venues = ko.observableArray();

    self.loadData = function (eventData) {
        self.id(eventData.Id);
        self.title(eventData.Title);
        self.startDate(eventData.StartDate
            ? sw.convertToLocal(moment(eventData.StartDate).toDate()) : undefined);
        self.endDate(eventData.EndDate
            ? sw.convertToLocal(moment(eventData.EndDate).toDate()) : undefined);
        self.isPublic(eventData.IsPublic);
        self.picture(eventData.Picture);
        self.combineType(eventData.CombineType);
        self.description(eventData.Description);
        /*self.latitude(eventData.Latitude);
        self.longitude(eventData.Longitude);*/

        self.host(eventData.Host ? new EntityViewModel(eventData.Host) : undefined);
        self.venues(
            eventData.Venues
                ? $.map(eventData.Venues,
                    function (venue) { return new EntityViewModel(venue); })
                : undefined);
    };

    self.toJSON = function () {
        var json = {
            Id: self.id(),
            CombineType: self.combineType(),
            Title: self.title(),
            StartDate: self.startDate()
                ? sw.convertToUTC(self.startDate()).toJSON() : undefined,
            EndDate: self.endDate()
                ? sw.convertToUTC(self.endDate()).toJSON() : undefined,
            IsPublic: self.isPublic(),
            Picture: self.picture(),
            Description: self.description(),
            /*Latitude: self.latitude(),
            Longitude: self.longitude(),*/

            Host: self.host() ? self.host().toJSON() : undefined,
            Venues: self.venues()
                ? $.map(self.venues(), function (venue) { return venue.toJSON(); })
                : undefined,
            
            Destroy: self._destroy
        };
        return json;
    };

    self.loadData(data);
};

EntityType =
{
    Host: 0,
    Venue: 1
};

function EntityViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.type = ko.observable();
    self.name = ko.observable();
    self.abbreviation = ko.observable();
    self.picture = ko.observable();
    self.description = ko.observable();

    self.contacts = ko.observableArray();
    self.addresses = ko.observableArray();
    self.shows = ko.observableArray();

    self.loadData = function (entityData) {
        self.id(entityData.Id);
        self.type(entityData.Type);
        self.name(entityData.Name);
        self.abbreviation(entityData.Abbreviation);
        self.picture(entityData.Picture);
        self.description(entityData.Description);

        self.contacts(
            entityData.Contacts
                ? $.map(entityData.Contacts,
                    function (contact) { return new ContactViewModel(contact); })
                : undefined);

        self.addresses(
            entityData.Addresses
                ? $.map(entityData.Addresses,
                    function (address) { return new AddressViewModel(address); })
                : undefined);

        self.shows(
            entityData.Shows
                ? $.map(entityData.Shows,
                    function (show) { return new ShowViewModel(show); })
                : undefined);
    };

    self.toJSON = function () {
        var json = {
            Id: self.id(),
            Type: self.type(),
            Name: self.name(),
            Abbreviation: self.abbreviation(),
            Picture: self.picture(),
            Description: self.description(),

            Contacts: self.contacts()
                ? $.map(self.contacts(), function (contact) { return contact.toJSON(); })
                : undefined,
            Addresses: self.addresses()
                ? $.map(self.addresses(), function (address) { return address.toJSON(); })
                : undefined,
            Shows: self.shows()
                ? $.map(self.shows(), function (show) { return show.toJSON(); })
                : undefined,
            
            Destroy: self._destroy
        };
        return json;
    };

    self.loadData(data);
};

function ShowViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.title = ko.observable();
    self.description = ko.observable();
    self.startTime = ko.observable();
    self.endTime = ko.observable();
    self.picture = ko.observable();
    self.detailsUrl = ko.observable();

    self.loadData = function (showData) {
        self.id(showData.Id);
        self.title(showData.Title);
        self.description(showData.Description);
        self.startTime(showData.StartTime
            ? sw.convertToLocal(moment(showData.StartTime).toDate()) : undefined);
        self.endTime(showData.EndTime
            ? sw.convertToLocal(moment(showData.EndTime).toDate()) : undefined);
        self.picture(showData.Picture);
        self.detailsUrl(showData.DetailsUrl);
    };

    self.toJSON = function () {
        var json = {
            Id: self.id(),
            Title: self.title(),
            Description: self.description(),
            StartTime: self.startTime()
                ? sw.convertToUTC(self.startTime()).toJSON() : undefined,
            EndTime: self.endTime()
                ? sw.convertToUTC(self.endTime()).toJSON() : undefined,
            Picture: self.picture(),
            DetailsUrl: self.detailsUrl(),
            Destroy: self._destroy
        };
        return json;
    };

    self.loadData(data);
}

// #########    V i e w    M o d e l s    E x t e n s i o n s     ##############

sw.ext = {};

sw.ext.displayName = function (event) {
    return event.title() || (event.host() != null ? event.host().name() : null);
};

sw.ext.displayPicture = function (event) {
    return event.picture() || (event.host() != null ? event.host().picture() : null);
};

sw.ext.displayDate = function (event) {
    return (event.startDate() ? moment(event.startDate()).format("LL") : "") +
        (event.endDate() ? " - " + moment(event.endDate()).format("LL") : "");
};

sw.ext.displayTime = function (show, allDayStr) {
    var result = (show.startTime() ? moment(show.startTime()).format("LT") : "") +
        (show.endTime() ? " - " + moment(show.endTime()).format("LT") : "");

    if (result == "") {
        result = allDayStr;
    }

    return result;
};