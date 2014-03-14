function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.HostId = ko.observable(data.HostId);
    self.Type = ko.observable(data.Type);
    self.State = ko.observable(data.State);

    self.Title = ko.observable(data.Title);
    self.Contact = ko.observable(data.Contact);
}

function AddressViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.Address = ko.observable(data.Address);
    self.State = ko.observable(data.State);

    self.Latitude = ko.observable(data.Latitude);
    self.Longitude = ko.observable(data.Longitude);
}

function ShowViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.VenueId = ko.observable(data.VenueId);
    self.IsReference = ko.observable(data.IsReference);
    self.Title = ko.observable(data.Title);
    self.Description = ko.observable(data.Description);
    self.StartDate = ko.observable(data.StartDate);
    self.StartTime = ko.observable(data.StartTime);
    self.EndDate = ko.observable(data.EndDate);
    self.EndTime = ko.observable(data.EndTime);
    self.Picture = ko.observable(data.Picture);
    self.DetailsUrl = ko.observable(data.DetailsUrl);
    self.State = ko.observable(data.State);
}

function EntityViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.Type = ko.observable(data.Type);

    self.Name = ko.observable(data.Name);
    self.Description = ko.observable(data.Description);
    self.Picture = ko.observable(data.Picture);
    self.State = ko.observable(data.State);

    // Contacts
    self.AllContacts = ko.observableArray($.map(data.AllContacts, function (item) { return new ContactViewModel(item); }));
    self.Contacts = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllContacts(), function (item) {
            return item.State() != 2;
        });
    }, this);
    self.addContact = function () {
        self.AllContacts.push(new ContactViewModel({ Id: 0, HostId: 0, Type: 1, State: 1 }));
    };

    self.removeContact = function (item) {
        item.State(2);
    };

    // Addresses
    self.AllAddresses = ko.observableArray($.map(data.AllAddresses, function (item) { return new AddressViewModel(item); }));
    self.Addresses = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllAddresses(), function (item) {
            return item.State() != 2;
        });
    }, this);
    self.addAddress = function () {
        self.AllAddresses.push(new AddressViewModel({ Id: 0, State: 1 }));
    };

    self.removeAddress = function (item) {
        item.State(2);
    };

    // Shows
    self.AllShows = ko.observableArray($.map(data.AllShows, function (item) { return new ShowViewModel(item); }));
    self.Shows = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllShows(), function (item) {
            return item.State() != 2;
        });
    }, this);
    self.addShow = function () {
        self.AllShows.push(new ShowViewModel({ Id: 0, VenueId: self.Id(), State: 1 }));
    };
    self.removeShow = function (item) {
        item.State(2);
    };
}

function EventViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.HostId = ko.observable(data.HostId);
    self.HostName = ko.observable(data.HostName);

    self.Title = ko.observable(data.Title);
    self.Description = ko.observable(data.Description);
    self.StartTime = ko.observable(data.StartTime);
    self.EndTime = ko.observable(data.EndTime);
    self.Latitude = ko.observable(data.Latitude);
    self.Longitude = ko.observable(data.Longitude);

    self.CombineType = ko.observable(data.CombineType);
    self.IsPublic = ko.observable(data.IsPublic);
    self.DateCreated = ko.observable(data.DateCreated);
    self.DateModified = ko.observable(data.DateModified);

    self.Host = ko.observable(new EntityViewModel(data.Host));

    // Venues
    self.AllVenues = ko.observableArray($.map(data.AllVenues, function (item) { return new EntityViewModel(item); }));
    self.Venues = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllVenues(), function (item) {
            return item.State() != 2;
        });
    }, this);
    self.addVenue = function () {
        self.AllVenues.push(new EntityViewModel({ Id: 0, Type: 1, State: 1 }));
    };
    self.removeVenue = function (item) {
        item.State(2);
    };
}