ListViewModel = function (parameters, url) {
    this.parameters_ = parameters;
    this.url_ = url;
    this.query = ko.observable();

    attachVerticalScroll.call(this, this.getNextPage);
};

ListViewModel.prototype.Items = ko.observableArray();
ListViewModel.prototype.currentPage = ko.observable(0);

// TODO: What "this" is doing in here?
ListViewModel.prototype.getData = function (pageNumber) {
    var self = this;

    if (self.currentPage() != pageNumber) {
        var ajData = JSON.stringify({
            pageNumber: pageNumber,
            query: self.query(),
            parameters: self.parameters_
        });

        ajaxJsonRequest.call(
            self,
            ajData,
            self.url_,
            function (data) {
                if (data.length > 0) {
                    self.currentPage(self.currentPage() + 1);

                    for (var i = 0; i < data.length; i++) {
                        self.addItem(data[i]);
                    }
                }
            }
        );
    }
};

// TODO: What "this" is doing in here?
ListViewModel.prototype.getNextPage = function () {
    return this.getData(this.currentPage() + 1);
};

// TODO: What "this" is doing in here?
ListViewModel.prototype.search = function (data) {
    $("a").remove(".default-rows");
    this.Items.removeAll();
    this.currentPage(-1);
    this.getNextPage();
};

ContactType = {
    Email: 0,
    Url: 1,
    Phone: 2
};

function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Type = ko.observable();
    self.State = ko.observable();

    self.Title = ko.observable();
    self.Contact = ko.observable();

    self.DisplayContact = ko.computed(function () {
        return (self.Title() ? self.Title() : "") + (self.Contact() ? ' [' + self.Contact() + ']' : "");
    }, self);    

    self.loadData = function (contactData) {
        self.Id(contactData.Id);
        self.EntityId(contactData.EntityId);
        self.Type(contactData.Type);
        self.State(contactData.State);

        self.Title(contactData.Title);
        self.Contact(contactData.Contact);
    };

    self.loadData(data);

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EntityId: self.EntityId(),
            Type: self.Type(),
            State: self.State(),
            Title: self.Title(),
            Contact: self.Contact()
        };
    }, self);
}

function AddressViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Address = ko.observable();
    self.Tip = ko.observable();
    self.State = ko.observable();

    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.GetMapLink = function () {
        if (!self.Address())
            return "";
        var res = self.Address().replace(/&/g, "").replace(/,\s+/g, ",").replace(/\s+/g, "+");
        return "https://www.google.com/maps/embed/v1/place?q=" + res +
            "&key=AIzaSyAOwfPuE85Mkr-xoNghkIB7enlmL0llMgo";
    };    

    self.loadData = function (addressData) {
        self.Id(addressData.Id);
        self.EntityId(addressData.EntityId);
        self.Address(addressData.Address);
        self.Tip(addressData.Tip);
        self.State(addressData.State);

        self.Latitude(addressData.Latitude);
        self.Longitude(addressData.Longitude);
    };

    self.loadData(data);

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EntityId: self.EntityId(),
            State: self.State(),
            Address: self.Address(),
            Tip: self.Tip(),
            Latitude: self.Latitude(),
            Longitude: self.Longitude()
        };
    }, self);
}

function EventViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.Title = ko.observable();
    self.StartTime = ko.observable();
    self.EndTime = ko.observable();
    self.DisplayDate = ko.observable();
    self.IsPublic = ko.observable();
    self.Picture = ko.observable();
    self.CombineType = ko.observable(data.CombineType);
    self.Description = ko.observable(data.Description);
    self.Latitude = ko.observable(data.Latitude);
    self.Longitude = ko.observable(data.Longitude);

    self.Host = ko.observable();
    self.AllVenues = ko.observableArray();

    self.loadData = function (eventData) {
        self.Id(eventData.Id);
        self.Title(eventData.Title);
        self.StartTime(eventData.StartTime ? eventData.StartTime : "");
        self.EndTime(eventData.EndTime ? eventData.EndTime : "");
        self.DisplayDate(eventData.DisplayDate);
        self.IsPublic(eventData.IsPublic);
        self.Picture(eventData.Picture);
        self.CombineType(eventData.CombineType);
        self.Description(eventData.Description);
        self.Latitude(eventData.Latitude);
        self.Longitude(eventData.Longitude);

        self.Host(eventData.Host ? new EntityViewModel(eventData.Host) : undefined);
        self.AllVenues(
            data.AllVenues
                ? $.map(data.AllVenues,
                    function (venue) { return new EntityViewModel(venue); })
                : undefined);
    };

    // TODO: Should not be computed, 'cause it's slows down shit
    // There is only one place where it's needed as computed, 
    // for ValidateModel async request, maybe use something else there?
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            CombineType: self.CombineType(),
            Title: self.Title(),
            StartTime: self.StartTime(),
            EndTime: self.EndTime(),
            IsPublic: self.IsPublic(),
            Picture: self.Picture(),

            Description: self.Description(),
            Latitude: self.Latitude(),
            Longitude: self.Longitude(),

            Host: self.Host() ? self.Host().toJSON() : undefined,
            AllVenues: self.AllVenues() 
                ? $.map(self.AllVenues(), function (venue) { return venue.toJSON(); })
                : undefined,
        };
    }, self);

    self.loadData(data);
};

EntityType =
{
    Host: 0,
    Venue: 1
};

function EntityViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EventMetadataId = ko.observable();
    self.State = ko.observable();
    self.Type = ko.observable();
    self.Name = ko.observable();
    self.Abbreviation = ko.observable();
    self.Picture = ko.observable();
    self.Description = ko.observable();

    self.AllContacts = ko.observableArray();
    self.AllAddresses = ko.observableArray();
    self.AllShows = ko.observableArray();

    self.DisplayAddress = ko.computed(function () {
        return self.AllAddresses() && self.AllAddresses().length > 0
            ? self.AllAddresses()[0] : "";
    }, self);

    self.loadData = function (entityData) {
        self.Id(entityData.Id);
        self.EventMetadataId(entityData.EventMetadataId);
        self.State(entityData.State);
        self.Type(entityData.Type);
        self.Name(entityData.Name);
        self.Abbreviation(entityData.Abbreviation);
        self.Picture(entityData.Picture);
        self.Description(entityData.Description);

        self.AllContacts(
            data.AllContacts
                ? $.map(data.AllContacts,
                    function (contact) { return new ContactViewModel(contact); })
                : undefined);

        self.AllAddresses(
            data.AllAddresses
                ? $.map(data.AllAddresses,
                    function (address) { return new AddressViewModel(address); })
                : undefined);

        self.AllShows(
            data.AllShows
                ? $.map(data.AllShows,
                    function (show) { return new ShowViewModel(show); })
                : undefined);
    };

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EventMetadataId: self.EventMetadataId(),
            State: self.State(),
            Type: self.Type(),
            Name: self.Name(),
            Abbreviation: self.Abbreviation(),
            Picture: self.Picture(),
            Description: self.Description(),

            AllContacts: self.AllContacts() 
                ? $.map(self.AllContacts(), function (contact) { return contact.toJSON(); })
                : undefined,
            AllAddresses: self.AllAddresses()
                ? $.map(self.AllAddresses(), function (address) { return address.toJSON(); })
                : undefined,
            AllShows: self.AllShows()
                ? $.map(self.AllShows(), function (show) { return show.toJSON(); })
                : undefined,
        };
    }, self);

    self.loadData(data);
};

function ShowViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EventMetadataId = ko.observable();
    self.VenueId = ko.observable();
    self.IsReference = ko.observable();
    self.Title = ko.observable();
    self.Description = ko.observable();
    self.StartDate = ko.observable();
    self.StartTime = ko.observable();
    self.EndDate = ko.observable();
    self.EndTime = ko.observable();
    self.Picture = ko.observable();
    self.DetailsUrl = ko.observable();
    self.State = ko.observable();

    self.TimeText = ko.computed(function () {
        return self.EndTime()
            ? self.StartTime() + '&nbsp-&nbsp' + self.EndTime()
            : self.StartTime();
    }, self);

    self.loadData = function (showData) {
        self.Id(showData.Id);
        self.EventMetadataId(showData.EventMetadataId);
        self.VenueId(showData.VenueId);
        self.IsReference(showData.IsReference);
        self.Title(showData.Title);
        self.Description(showData.Description);
        self.StartDate(showData.StartDate ? showData.StartDate : "");
        self.StartTime(showData.StartTime ? showData.StartTime : "");
        self.EndDate(showData.EndDate ? showData.EndDate : "");
        self.EndTime(showData.EndTime ? showData.EndTime : "");
        self.Picture(showData.Picture);
        self.DetailsUrl(showData.DetailsUrl);
        self.State(showData.State);
    }

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EventMetadataId: self.EventMetadataId(),
            VenueId: self.VenueId(),
            IsReference: self.IsReference(),
            Title: self.Title(),
            Description: self.Description(),
            StartDate: self.StartDate(),
            StartTime: self.StartTime(),
            EndDate: self.EndDate(),
            EndTime: self.EndTime(),
            Picture: self.Picture(),
            DetailsUrl: self.DetailsUrl(),
            State: self.State()
        };
    }, self);

    self.loadData(data);
}