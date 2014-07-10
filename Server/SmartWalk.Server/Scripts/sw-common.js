﻿
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

VmItemState =
{
    Normal: 0,
    Added: 1,
    Deleted: 2,
    Hidden: 3
};

ContactType = {
    Email: 0,
    Url: 1,
    Phone: 2
};

function ContactViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.entityId = ko.observable();
    self.type = ko.observable();
    self.state = ko.observable();

    self.title = ko.observable();
    self.contact = ko.observable();

    self.displayContact = ko.computed(function () {
        return (self.title() ? self.title() : "") +
            (self.contact() ? ' [' + self.contact() + ']' : "");
    });

    self.loadData = function (contactData) {
        self.id(contactData.Id);
        self.entityId(contactData.EntityId);
        self.type(contactData.Type);
        self.state(contactData.State);

        self.title(contactData.Title);
        self.contact(contactData.Contact);
    };

    self.loadData(data);

    self.toJSON = function () {
        return {
            Id: self.id(),
            EntityId: self.entityId(),
            Type: self.type(),
            State: self.state(),
            Title: self.title(),
            Contact: self.contact()
        };
    };
}

function AddressViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.entityId = ko.observable();
    self.address = ko.observable();
    self.tip = ko.observable();
    self.state = ko.observable();

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
        self.entityId(addressData.EntityId);
        self.address(addressData.Address);
        self.tip(addressData.Tip);
        self.state(addressData.State);

        self.latitude(addressData.Latitude);
        self.longitude(addressData.Longitude);
    };

    self.loadData(data);

    self.toJSON = function () {
        return {
            Id: self.id(),
            EntityId: self.entityId(),
            State: self.state(),
            Address: self.address(),
            Tip: self.tip(),
            Latitude: self.latitude(),
            Longitude: self.longitude()
        };
    };
}

function EventViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.title = ko.observable();
    self.startTime = ko.observable();
    self.endTime = ko.observable();
    self.displayDate = ko.observable();
    self.isPublic = ko.observable();
    self.picture = ko.observable();
    self.combineType = ko.observable(data.combineType);
    self.description = ko.observable(data.description);
    self.latitude = ko.observable(data.latitude);
    self.longitude = ko.observable(data.longitude);

    self.host = ko.observable();
    self.allVenues = ko.observableArray();

    self.loadData = function (eventData) {
        self.id(eventData.Id);
        self.title(eventData.Title);
        self.startTime(eventData.StartTime ? eventData.StartTime : "");
        self.endTime(eventData.EndTime ? eventData.EndTime : "");
        self.displayDate(eventData.DisplayDate);
        self.isPublic(eventData.IsPublic);
        self.picture(eventData.Picture);
        self.combineType(eventData.CombineType);
        self.description(eventData.Description);
        self.latitude(eventData.Latitude);
        self.longitude(eventData.Longitude);

        self.host(eventData.Host ? new EntityViewModel(eventData.Host) : undefined);
        self.allVenues(
            eventData.AllVenues
                ? $.map(eventData.AllVenues,
                    function (venue) { return new EntityViewModel(venue); })
                : undefined);
    };

    self.toJSON = function () {
        return {
            Id: self.id(),
            CombineType: self.combineType(),
            Title: self.title(),
            StartTime: self.startTime(),
            EndTime: self.endTime(),
            IsPublic: self.isPublic(),
            Picture: self.picture(),

            Description: self.description(),
            Latitude: self.latitude(),
            Longitude: self.longitude(),

            Host: self.host() ? self.host().toJSON() : undefined,
            AllVenues: self.allVenues()
                ? $.map(self.allVenues(), function (venue) { return venue.toJSON(); })
                : undefined,
        };
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
    self.eventMetadataId = ko.observable();
    self.state = ko.observable();
    self.type = ko.observable();
    self.name = ko.observable();
    self.abbreviation = ko.observable();
    self.picture = ko.observable();
    self.description = ko.observable();

    self.allContacts = ko.observableArray();
    self.allAddresses = ko.observableArray();
    self.allShows = ko.observableArray();

    self.displayAddress = ko.computed(function () {
        return self.allAddresses() && self.allAddresses().length > 0
            ? self.allAddresses()[0] : "";
    });

    self.loadData = function (entityData) {
        self.id(entityData.Id);
        self.eventMetadataId(entityData.EventMetadataId);
        self.state(entityData.State);
        self.type(entityData.Type);
        self.name(entityData.Name);
        self.abbreviation(entityData.Abbreviation);
        self.picture(entityData.Picture);
        self.description(entityData.Description);

        self.allContacts(
            entityData.AllContacts
                ? $.map(entityData.AllContacts,
                    function (contact) { return new ContactViewModel(contact); })
                : undefined);

        self.allAddresses(
            entityData.AllAddresses
                ? $.map(entityData.AllAddresses,
                    function (address) { return new AddressViewModel(address); })
                : undefined);

        self.allShows(
            entityData.AllShows
                ? $.map(entityData.AllShows,
                    function (show) { return new ShowViewModel(show); })
                : undefined);
    };

    self.toJSON = function () {
        return {
            Id: self.id(),
            EventMetadataId: self.eventMetadataId(),
            State: self.state(),
            Type: self.type(),
            Name: self.name(),
            Abbreviation: self.abbreviation(),
            Picture: self.picture(),
            Description: self.description(),

            AllContacts: self.allContacts()
                ? $.map(self.allContacts(), function (contact) { return contact.toJSON(); })
                : undefined,
            AllAddresses: self.allAddresses()
                ? $.map(self.allAddresses(), function (address) { return address.toJSON(); })
                : undefined,
            AllShows: self.allShows()
                ? $.map(self.allShows(), function (show) { return show.toJSON(); })
                : undefined,
        };
    };

    self.loadData(data);
};

function ShowViewModel(data) {
    var self = this;

    self.id = ko.observable();
    self.eventMetadataId = ko.observable();
    self.venueId = ko.observable();
    self.isReference = ko.observable();
    self.title = ko.observable();
    self.description = ko.observable();
    self.startDate = ko.observable();
    self.startTime = ko.observable();
    self.endDate = ko.observable();
    self.endTime = ko.observable();
    self.picture = ko.observable();
    self.detailsUrl = ko.observable();
    self.state = ko.observable();

    self.timeText = ko.computed(function () {
        return self.endTime()
            ? self.startTime() + '&nbsp-&nbsp' + self.endTime()
            : self.startTime();
    });

    self.loadData = function (showData) {
        self.id(showData.Id);
        self.eventMetadataId(showData.EventMetadataId);
        self.venueId(showData.VenueId);
        self.isReference(showData.IsReference);
        self.title(showData.Title);
        self.description(showData.Description);
        self.startDate(showData.StartDate ? showData.StartDate : "");
        self.startTime(showData.StartTime ? showData.StartTime : "");
        self.endDate(showData.EndDate ? showData.EndDate : "");
        self.endTime(showData.EndTime ? showData.EndTime : "");
        self.picture(showData.Picture);
        self.detailsUrl(showData.DetailsUrl);
        self.state(showData.State);
    };

    self.toJSON = function () {
        return {
            Id: self.id(),
            EventMetadataId: self.eventMetadataId(),
            VenueId: self.venueId(),
            IsReference: self.isReference(),
            Title: self.title(),
            Description: self.description(),
            StartDate: self.startDate(),
            StartTime: self.startTime(),
            EndDate: self.endDate(),
            EndTime: self.endTime(),
            Picture: self.picture(),
            DetailsUrl: self.detailsUrl(),
            State: self.state()
        };
    };

    self.loadData(data);
}