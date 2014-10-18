// #####################   C o m m o n    ##########################

if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}

// SmartWalk Commons

if (typeof sw === "undefined") sw = {};

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


// #########    B i n d i n g    H a n d l e r s     ################

// Just a fade effect on creation of binded element
ko.bindingHandlers.fadeIn = {
    init: function (element, valueAccessor) {
        var duration = ko.utils.unwrapObservable(valueAccessor());
        $(element).hide().fadeIn(duration);
    }
};

// Fade In / Out effect on visibility change
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

// Just Fade In on visible, and no effect on hiding
ko.bindingHandlers.fadeInVisible = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value) || false);
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).fadeIn() : $(element).fadeOut(0);
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

sw.vm = {};

ViewModelBase = function () {
    var self = this;

    self.currentRequest = null;

    self.isBusy = ko.observable(false).extend({ throttle: 1 });
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

    self.onWindowClose = function() { return undefined; };
    window.onbeforeunload = function() { return self.onWindowClose(); };
};

ListViewModel = function (parameters, url) {
    var self = this;
    
    ListViewModel.superClass_.constructor.call(self);
    
    self._currentPage = 0;
    self._finished = false;
   
    self.query = ko.observable().extend({ throttle: 300 });
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
    
    // Searching
    
    self.searchedQuery = ko.observable(null);

    self.isBusy.subscribe(function (isBusy) {
        if (!isBusy && self.query()) {
            self.searchedQuery(self.query());
        }
    });

    self.query.subscribe(function () {
        self.searchedQuery(null);
        self.items.removeAll();
        self._finished = false;
        self._currentPage = -1;
        self.getNextPage();
    });
};

sw.inherits(ListViewModel, ViewModelBase);

sw.vm.ContactType = {
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

    self.loadData(data);
}

ContactViewModel.prototype.loadData = function (contactData) {
    this.id(contactData.Id);
    this.type(contactData.Type);
    this.title(contactData.Title);
    this.contact(contactData.Contact);
};

ContactViewModel.prototype.toJSON = function () {
    var json = {
        Id: this.id(),
        Type: this.type(),
        Title: this.title(),
        Contact: this.contact(),
        Destroy: this._destroy
    };
    return json;
};

function AddressViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.address = ko.observable();
    self.tip = ko.observable();
    self.latitude = ko.observable();
    self.longitude = ko.observable();

    self.loadData(data);
}

AddressViewModel.prototype.loadData = function (addressData) {
    this.id(addressData.Id);
    this.address(addressData.Address);
    this.tip(addressData.Tip);
    this.latitude(addressData.Latitude);
    this.longitude(addressData.Longitude);
};

AddressViewModel.prototype.toJSON = function () {
    var json = {
        Id: this.id(),
        Address: this.address(),
        Tip: this.tip(),
        Latitude: this.latitude(),
        Longitude: this.longitude(),
        Destroy: this._destroy
    };
    return json;
};

function EventEntityDetailViewModel(data) {
    var self = this;

    self.sortOrder = ko.observable();
    self.description = ko.observable();

    self.loadData(data);
}

EventEntityDetailViewModel.prototype.loadData = function (detailData) {
    this.sortOrder(detailData.SortOrder);
    this.description(detailData.Description);
};

EventEntityDetailViewModel.prototype.toJSON = function () {
    var json = {
        SortOrder: this.sortOrder(),
        Description: this.description()
    };
    return json;
};

function EventViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.title = ko.observable();
    self.startDate = ko.observable();
    self.endDate = ko.observable();
    self.status = ko.observable();
    self.venueOrderType = ko.observable();
    self.venueTitleFormatType = ko.observable();
    self.picture = ko.observable();
    self.combineType = ko.observable();
    self.description = ko.observable();
    /*self.latitude = ko.observable();
    self.longitude = ko.observable();*/
    
    self.host = ko.observable();
    self.venues = ko.observableArray();
    
    self.loadData(data);
};

EventViewModel.prototype.loadData = function (eventData) {
    this.id(eventData.Id);
    this.title(eventData.Title);
    this.startDate(eventData.StartDate
        ? sw.convertToLocal(moment(eventData.StartDate).toDate()) : undefined);
    this.endDate(eventData.EndDate
        ? sw.convertToLocal(moment(eventData.EndDate).toDate()) : undefined);
    this.status(eventData.Status);
    this.picture(eventData.Picture);
    this.combineType(eventData.CombineType);
    this.venueOrderType(eventData.VenueOrderType);
    this.venueTitleFormatType(eventData.VenueTitleFormatType);
    this.description(eventData.Description);
    /*this.latitude(eventData.Latitude);
    this.longitude(eventData.Longitude);*/

    this.host(eventData.Host ? new EntityViewModel(eventData.Host) : undefined);
    this.venues(
        eventData.Venues
            ? $.map(eventData.Venues,
                function (venue) { return new EntityViewModel(venue); })
            : undefined);
};

EventViewModel.prototype.toJSON = function () {
    var self = this;

    var json = {
        Id: self.id(),
        CombineType: self.combineType(),
        VenueOrderType: self.venueOrderType(),
        VenueTitleFormatType: self.venueTitleFormatType(),
        Title: self.title(),
        StartDate: self.startDate()
            ? sw.convertToUTC(self.startDate()).toJSON() : undefined,
        EndDate: self.endDate()
            ? sw.convertToUTC(self.endDate()).toJSON() : undefined,
        Status: self.status(),
        Picture: self.picture(),
        Description: self.description(),
        /*Latitude: self.latitude(),
        Longitude: self.longitude(),*/

        Host: self.host() ? self.host().toJSON() : undefined,
        Venues: self.venues()
            ? $.map(self.venues(),
                function (venue) {
                    if (self.venueOrderType() == sw.vm.VenueOrderType.Name &&
                        venue.eventDetail()) {
                        venue.eventDetail().sortOrder(undefined);
                    }

                    return venue.toJSON();
                })
            : undefined,

        Destroy: self._destroy
    };
    return json;
};

sw.vm.CombineType =
{
    None: 0,
    ByVenue: 1,
    ByHost: 2
};

sw.vm.VenueOrderType = {
    Name: 0,
    Custom: 1
};

sw.vm.VenueTitleFormatType = {
    Name: 0,
    NameAndNumber: 1
};

sw.vm.EventStatus =
{
    Private: 0,
    Public: 1,
    Unlisted: 2
};

sw.vm.EntityType =
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
    self.eventDetail = ko.observable();

    self.contacts = ko.observableArray();
    self.addresses = ko.observableArray();
    self.shows = ko.observableArray();

    self.loadData(data);
};

EntityViewModel.prototype.loadData = function (entityData) {
    this.id(entityData.Id);
    this.type(entityData.Type);
    this.name(entityData.Name);
    this.abbreviation(entityData.Abbreviation);
    this.picture(entityData.Picture);
    this.description(entityData.Description);

    if (this.eventDetail()) {
        this.eventDetail().loadData(entityData.EventDetail || {});
    } else {
        this.eventDetail(new EventEntityDetailViewModel(entityData.EventDetail || {}));
    }

    this.contacts(
        entityData.Contacts
            ? $.map(entityData.Contacts,
                function (contact) { return new ContactViewModel(contact); })
            : undefined);

    this.addresses(
        entityData.Addresses
            ? $.map(entityData.Addresses,
                function (address) { return new AddressViewModel(address); })
            : undefined);

    this.shows(
        entityData.Shows
            ? $.map(entityData.Shows,
                function (show) { return new ShowViewModel(show); })
            : undefined);
};

EntityViewModel.prototype.toJSON = function () {
    var json = {
        Id: this.id(),
        Type: this.type(),
        Name: this.name(),
        Abbreviation: this.abbreviation(),
        Picture: this.picture(),
        Description: this.description(),

        EventDetail: this.eventDetail() &&
                (this.eventDetail().sortOrder() || this.eventDetail().description())
            ? this.eventDetail().toJSON()
            : undefined,

        Contacts: this.contacts()
            ? $.map(this.contacts(), function (contact) { return contact.toJSON(); })
            : undefined,
        Addresses: this.addresses()
            ? $.map(this.addresses(), function (address) { return address.toJSON(); })
            : undefined,
        Shows: this.shows()
            ? $.map(this.shows(), function (show) { return show.toJSON(); })
            : undefined,

        Destroy: this._destroy
    };
    return json;
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

    self.loadData(data);
}

ShowViewModel.prototype.loadData = function (showData) {
    this.id(showData.Id);
    this.title(showData.Title);
    this.description(showData.Description);
    this.startTime(showData.StartTime
        ? sw.convertToLocal(moment(showData.StartTime).toDate()) : undefined);
    this.endTime(showData.EndTime
        ? sw.convertToLocal(moment(showData.EndTime).toDate()) : undefined);
    this.picture(showData.Picture);
    this.detailsUrl(showData.DetailsUrl);
};

ShowViewModel.prototype.toJSON = function () {
    var json = {
        Id: this.id(),
        Title: this.title(),
        Description: this.description(),
        StartTime: this.startTime()
            ? sw.convertToUTC(this.startTime()).toJSON() : undefined,
        EndTime: this.endTime()
            ? sw.convertToUTC(this.endTime()).toJSON() : undefined,
        Picture: this.picture(),
        DetailsUrl: this.detailsUrl(),
        Destroy: this._destroy
    };
    return json;
};

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

sw.ext.displayTime = function (show) {
    var result = sw.ext.displayStartTime(show) + sw.ext.displayEndTime(show);
    return result;
};

sw.ext.displayStartTime = function (show) {
    var result = show.startTime() && show.endTime() ? moment(show.startTime()).format("LT") : "";
    return result;
};

sw.ext.displayEndTime = function (show) {
    var result = show.endTime() || show.startTime() ? moment(show.endTime() || show.startTime()).format("LT") : "";
    return result;
};