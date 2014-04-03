function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Type = ko.observable();
    self.State = ko.observable();
    self.IsChecked = ko.observable();

    self.Title = ko.observable();
    self.Contact = ko.observable();
    
    self.DisplayContact = ko.computed(function () {
        return (self.Title() ? self.Title() : "") + (self.Contact() ? ' [' +  self.Contact() + ']' : "");
    }, self);

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EntityId(data.EntityId);
        self.Type(data.Type);
        self.State(data.State);
        self.IsChecked(false);

        self.Title(data.Title);
        self.Contact(data.Contact);
    };

    self.loadData(data);
}

function AddressViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Address = ko.observable();
    self.State = ko.observable();
    self.IsChecked = ko.observable();

    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.GetMapLink = function () {
        if (!self.Address())
            return "";
        var res = self.Address().replace(/&/g, "").replace(/,\s+/g, ",").replace(/\s+/g, "+");
        return "https://www.google.com/maps/embed/v1/place?q=" + res + "&key=AIzaSyAOwfPuE85Mkr-xoNghkIB7enlmL0llMgo";
    };
    
    self.loadData = function (data) {
        self.Id(data.Id);
        self.EntityId(data.EntityId);
        self.Address(data.Address);
        self.State(data.State);
        self.IsChecked(false);

        self.Latitude(data.Latitude);
        self.Longitude(data.Longitude);
    };

    self.loadData(data);
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

    self.selectedItem = ko.observable();
    
    // Contacts
    self.AllContacts = ko.observableArray($.map(data.AllContacts, function (item) { return new ContactViewModel(item); }));
    self.Contacts = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllContacts(), function (item) {
            return item.State() != 2;
        });
    }, this);    
    self.DeletedContacts = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllContacts(), function (item) {
            return item.State() == 2;
        });
    }, this);
    self.CheckedContacts = ko.computed(function () {
        return ko.utils.arrayFilter(self.Contacts(), function (item) {
            return item.IsChecked() && item != self.selectedItem();
        });
    }, this);


    self.addContact = function () {
        self.AllContacts.push(new ContactViewModel({ Id: 0, EntityId: self.Id(), Type: 1, State: 1 }));
        self.selectedItem(self.AllContacts()[self.AllContacts().length - 1]);
    };

    self.deleteContact = function (item) {
        item.State(2);
    };

    self.IsAllContactsSelected = ko.computed(function () {
        if (self.Contacts().length == 0)
            return false;

        for (var i = 0; i < self.Contacts().length; i++) {
            if (!self.Contacts()[i].IsChecked()) {
                return false;
            }
        }
        return true;
    });

    self.IsAnyContactSelected = ko.computed(function () {
        if (self.Contacts().length == 0)
            return false;

        for (var i = 0; i < self.Contacts().length; i++) {
            if (self.Contacts()[i].IsChecked()) {
                return true;
            }
        }
        return false;
    });

    self.AllContactsChecked = ko.computed({
        read: self.IsAllContactsSelected,
        write: function (value) {
            for (var i = 0; i < self.Contacts().length; i++) {
                self.Contacts()[i].IsChecked(value);
            }
        },
        owner: this
    });

    //self.selectedContact = ko.observable();

    // Addresses
    self.AllAddresses = ko.observableArray($.map(data.AllAddresses, function (item) { return new AddressViewModel(item); }));
    self.Addresses = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllAddresses(), function (item) {
            return item.State() != 2;
        });
    }, this);
    self.DeletedAddresses = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllAddresses(), function (item) {
            return item.State() == 2;
        });
    }, this);    
    self.CheckedAddresses = ko.computed(function () {
        return ko.utils.arrayFilter(self.Addresses(), function (item) {
            return item.IsChecked() && item != self.selectedItem();
        });
    }, this);

    self.addAddress = function () {
        self.AllAddresses.push(new AddressViewModel({ Id: 0, EntityId: self.Id(), State: 1, Address: "" }));
        self.selectedItem(self.AllAddresses()[self.AllAddresses().length - 1]);
    };

    self.deleteAddress = function (item) {
        item.State(2);
    };

    self.IsAllAddressSelected = ko.computed(function () {
        if (self.Addresses().length == 0)
            return false;

        for (var i = 0; i < self.Addresses().length; i++) {
            if (!self.Addresses()[i].IsChecked()) {
                return false;
            }
        }
        return true;
    });
    
    self.IsAnyAddressSelected = ko.computed(function () {
        if (self.Addresses().length == 0)
            return false;

        for (var i = 0; i < self.Addresses().length; i++) {
            if (self.Addresses()[i].IsChecked()) {
                return true;
            }
        }
        return false;
    });

    self.AllAddressesChecked = ko.computed({
        read: self.IsAllAddressSelected,
        write: function (value) {
            for (var i = 0; i < self.Addresses().length; i++) {
                self.Addresses()[i].IsChecked(value);
            }
        },
        owner: this
    });

    //self.selectedAddress = ko.observable();
    // Shows
    self.AllShows = ko.observableArray($.map(data.AllShows, function (item) { return new ShowViewModel(item); }));
    self.Shows = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllShows(), function (item) {
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
    self.Picture = ko.observable();
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

        self.Picture(data.Picture);

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