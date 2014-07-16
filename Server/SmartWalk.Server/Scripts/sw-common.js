
// #####################   C o m m o n    ##########################


function inherits(child, parent) {
    var f = function () { };
    f.prototype = parent.prototype;
    child.prototype = new f();
    child.prototype.constructor = child;

    // `child` function is an object like all functions    
    child.superClass_ = parent.prototype;
};

function attachVerticalScroll(callback) {
    $(window).scroll(function (evt) {
        evt.preventDefault();

        if ($(window).scrollTop() >=
            $(document).height() - $(window).height()) {
            callback();
        }
    });
};

function ajaxJsonRequest(ajData, url, onSuccess, onError) {
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

    $.ajax(config)
        .done(function (response, statusText, xhr) {
            if (onSuccess) onSuccess.call(self, response, statusText, xhr);
        })
        .fail(function (response, statusText, xhr) {
            if (onError) onError.call(self, response, statusText, xhr);
        });
};

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(date.getDate() + days);
    return result;
}

function convertToLocal(date) {
    var result = date 
        ? new Date(date.getTime() + (date.getTimezoneOffset() * 60000))
        : date;
    return result;
}

function convertToUTC(date) {
    var result = date
        ? new Date(date.getTime() - (date.getTimezoneOffset() * 60000))
        : date;
    return result;
}

// #########    B i n d i n g    H a n d l e r s     ################


ko.bindingHandlers.fadeInVisible = {
    init: function (element, valueAccessor) {
        var duration = ko.utils.unwrapObservable(valueAccessor());
        $(element).hide().fadeIn(duration);
    }
};

ko.bindingHandlers.scrollVisible = {
    init: function (element, valueAccessor) {
        if (!$(element).parents(".ui-dialog").length && // don't scroll if content in dialog
            !$(element).visible(false, false, "vertical")) {
            var duration = ko.utils.unwrapObservable(valueAccessor());
            
            $("html, body").animate({
                scrollTop: $(element).offset().top - 80 // minus small top margin
            }, duration);
        }
    }
};


// ###############    V i e w    M o d e l s     ####################


ListViewModel = function (parameters, url) {
    var self = this;

    self.query = ko.observable();
    self.items = ko.observableArray();
    self.currentPage = ko.observable(0);

    self.addItem = function () { }; // abstract ;-)

    self.getData = function (pageNumber) {
        if (self.currentPage() != pageNumber) {
            ajaxJsonRequest(
                {
                    pageNumber: pageNumber,
                    query: self.query(),
                    parameters: parameters
                },
                url,
                function (items) {
                    if (items && items.length > 0) {
                        self.currentPage(self.currentPage() + 1);
                        items.forEach(function (item) { self.addItem(item); });
                    }
                }
            );
        }
    };

    self.getNextPage = function () {
        return self.getData(self.currentPage() + 1);
    };

    self.search = function () {
        self.items.removeAll();
        self.currentPage(-1);
        self.getNextPage();
    };
};

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

    self.getMapLink = function () {
        if (!self.address()) return "";
        var res = self.address().replace(/&/g, "").replace(/,\s+/g, ",").replace(/\s+/g, "+");
        return "https://www.google.com/maps/embed/v1/place?q=" + res +
            "&key=AIzaSyAOwfPuE85Mkr-xoNghkIB7enlmL0llMgo";
    };

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
    
    self.displayName = ko.computed(function () {
        return self.title() || (self.host() != null ? self.host().name() : null);
    });

    self.displayPicture = ko.computed(function () {
        return self.picture() || (self.host() != null ? self.host().picture() : null);
    });
    
    // TODO: To setup common date format
    self.displayDate = ko.computed(function () {
        return (self.startDate() ? self.startDate().toLocaleDateString() : "") +
            (self.endDate() ? " - " + self.endDate().toLocaleDateString() : "");
    });

    self.loadData = function (eventData) {
        self.id(eventData.Id);
        self.title(eventData.Title);
        self.startDate(eventData.StartDate ? new Date(eventData.StartDate) : undefined);
        self.endDate(eventData.EndDate ? new Date(eventData.EndDate) : undefined);
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
            StartDate: self.startDate() ? self.startDate().toJSON() : undefined,
            EndDate: self.endDate() ? self.endDate().toJSON() : undefined,
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
    
    // TODO: To setup common time format
    self.displayTime = ko.computed(function () {
        return (self.startTime() ? self.startTime().toLocaleTimeString() : "") +
            (self.endTime() ? " - " + self.endTime().toLocaleTimeString() : "");
    });

    self.loadData = function (showData) {
        self.id(showData.Id);
        self.title(showData.Title);
        self.description(showData.Description);
        self.startTime(showData.StartTime ? new Date(showData.StartTime) : undefined);
        self.endTime(showData.EndTime ? new Date(showData.EndTime) : undefined);
        self.picture(showData.Picture);
        self.detailsUrl(showData.DetailsUrl);
    };

    self.toJSON = function () {
        var json = {
            Id: self.id(),
            Title: self.title(),
            Description: self.description(),
            StartTime: self.startTime() ? self.startTime().toJSON() : undefined,
            EndTime: self.endTime() ? self.endTime().toJSON() : undefined,
            Picture: self.picture(),
            DetailsUrl: self.detailsUrl(),
            Destroy: self._destroy
        };
        return json;
    };

    self.loadData(data);
}