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

    self.Id = ko.observable();
    self.EventMetedataId = ko.observable();
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

    self.loadData = function(data) {
        self.Id(data.Id);
        self.EventMetedataId(data.EventMetedataId);
        self.VenueId(data.VenueId);
        self.IsReference(data.IsReference);
        self.Title(data.Title);
        self.Description(data.Description);
        self.StartDate(data.StartDate ? data.StartDate : '');
        self.StartTime(data.StartTime ? data.StartTime : '');
        self.EndDate(data.EndDate ? data.EndDate : '');
        self.EndTime(data.EndTime ? data.EndTime : '');
        self.Picture(data.Picture);
        self.DetailsUrl(data.DetailsUrl);
        self.State(data.State);
    };

    self.loadData(data);
}

function EntityViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.EventMetedataId = ko.observable(data.EventMetedataId);

    self.Type = ko.observable(data.Type);

    self.Name = ko.observable(data.Name);
    self.DisplayName = ko.observable(data.DisplayName);

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
            return item.State() != 2 && item.State() != 3;
        });
    }, this);
    self.addShow = function () {
        self.AllShows.push(new ShowViewModel({ Id: 0, EventMetedataId: self.EventMetedataId(), VenueId: self.Id(), State: 1, StartDate: '', EndDate: '' }));
        return self.AllShows()[self.AllShows().length - 1];
    };
    self.removeShow = function (item) {
        item.State(2);
    };
}

function EventViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.UserId = ko.observable();
    self.HostId = ko.observable();
    self.HostName = ko.observable();

    self.Title = ko.observable();
    self.Description = ko.observable();
    self.StartTime = ko.observable();
    self.EndTime = ko.observable();
    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.CombineType = ko.observable();
    self.IsPublic = ko.observable();
    self.DateCreated = ko.observable();
    self.DateModified = ko.observable();

    self.Host = ko.observable();
    self.AllVenues = ko.observableArray();
    self.AllHosts = ko.observableArray();
    
    self.loadData = function (data) {
        self.Id(data.Id);
        self.UserId(data.UserId);
        self.HostId(data.HostId);
        self.HostName(data.HostName);

        self.Title(data.Title);
        self.Description(data.Description);

        self.StartTime(data.StartTime?data.StartTime:'');
        self.EndTime(data.EndTime?data.EndTime:'');
        
        self.Latitude(data.Latitude);
        self.Longitude(data.Longitude);

        self.CombineType(data.CombineType);
        self.IsPublic(data.IsPublic);
        self.DateCreated(data.DateCreated);
        self.DateModified(data.DateModified);

        self.AllVenues($.map(data.AllVenues, function (item) { return new EntityViewModel(item); }));
        self.AllHosts($.map(data.AllHosts, function(item) {
            var vmItem = new EntityViewModel(item);
            if (data.Host != null && vmItem.Id() == data.Host.Id)
                self.Host(vmItem);
            return vmItem;
        }));
    };

    self.loadData(data);

    self.Venues = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllVenues(), function (item) {
            return item.State() != 2 && item.State() != 3;
        });
    }, this);
    self.OtherVenues = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllVenues(), function (item) {
            return item.State() == 3;
        });
    }, this);
    self.addVenue = function () {
        self.AllVenues.push(new EntityViewModel({ Id: 0, Type: 1, State: 1 }));
    };
    self.removeVenue = function (item) {
        item.State(2);
    };

    self.selectedShow = ko.observable();
    self.selectedVenue = ko.observable();

    self.toJSON = function() {
        var copy = ko.toJS(self); //just a quick way to get a clean copy
        delete copy.AllVenues;
        delete copy.Venues;
        delete copy.OtherVenues;
        return copy;
    };
}