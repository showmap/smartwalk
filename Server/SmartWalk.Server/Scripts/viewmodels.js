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
    self.Tip = ko.observable();
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
        self.Tip(data.Tip);
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
    
    self.IsChecked = ko.observable();


    self.TimeText = ko.computed(function () {
        if (self.EndTime()) {
            return self.StartTime() + ' - ' + self.EndTime();
        }

        return self.StartTime();
    }, this);

    self.loadData = function(data) {
        self.Id(data.Id);
        self.EventMetadataId(data.EventMetadataId);
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
        self.IsChecked(false);
    };

    self.loadData(data);
}

function EntityViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EventMetadataId = ko.observable();

    self.Type = ko.observable();

    self.Name = ko.observable();
    self.DisplayName = ko.observable();

    self.Description = ko.observable();
    self.Picture = ko.observable();
    self.State = ko.observable();

    self.selectedItem = ko.observable();
    
    self.AllContacts = ko.observableArray();
    self.AllAddresses = ko.observableArray();
    self.AllShows = ko.observableArray();

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EventMetadataId(data.EventMetadataId);
        self.Type(data.Type);

        self.Name(data.Name);
        self.DisplayName(data.DisplayName);

        self.Description(data.Description);
        self.Picture(data.Picture);
        self.State(data.State);

        if (data.AllContacts)
            self.AllContacts($.map(data.AllContacts, function (item) { return new ContactViewModel(item); }));
        if (data.AllAddresses)
            self.AllAddresses($.map(data.AllAddresses, function (item) { return new AddressViewModel(item); }));
        if (data.AllShows)
            self.AllShows($.map(data.AllShows, function (item) { return new ShowViewModel(item); }));
    };

    self.loadData(data);

    // Contacts    
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
        read: function () {
            if (self.Contacts().length == 0)
                return false;

            for (var i = 0; i < self.Contacts().length; i++) {
                if (!self.Contacts()[i].IsChecked()) {
                    return false;
                }
            }
            return true;
        },
        write: function (value) {
            for (var i = 0; i < self.Contacts().length; i++) {
                self.Contacts()[i].IsChecked(value);
            }
        },
        owner: this
    });

    //self.selectedContact = ko.observable();

    // Addresses    
    self.Addresses = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllAddresses(), function (item) {
            return item.State() != 2;
        });
    }, this);
    self.DisplayAddress = ko.computed(function () {
        if (self.Addresses().length <= 0)
            return null;
        return self.Addresses()[0];
    });
    self.IsMapVisible = ko.computed(function () {
        if (self.Addresses().length > 1)
            return true;
        if (self.Addresses().length == 1 && self.Addresses()[0] != self.selectedItem())
            return true;

        return false;
    });
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
        read: function () {
            if (self.Addresses().length == 0)
                return false;

            for (var i = 0; i < self.Addresses().length; i++) {
                if (!self.Addresses()[i].IsChecked()) {
                    return false;
                }
            }
            return true;
        },
        write: function (value) {
            for (var i = 0; i < self.Addresses().length; i++) {
                self.Addresses()[i].IsChecked(value);
            }
        },
        owner: this
    });

    //self.selectedAddress = ko.observable();
    // Shows    

    self.Shows = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllShows(), function (item) {
            return item.State() != 2 && item.State() != 3;
        });
    }, self);
    self.CheckedShows = ko.computed(function () {
        return ko.utils.arrayFilter(self.Shows(), function (item) {
            return item.IsChecked();
        });
    }, self);
    self.AllShowsChecked = ko.computed({
        read: function () {
            if (self.Shows().length == 0)
                return false;

            for (var i = 0; i < self.Shows().length; i++) {
                if (!self.Shows()[i].IsChecked()) {
                    return false;
                }
            }
            return true;
        },
        write: function (value) {
            for (var i = 0; i < self.Shows().length; i++) {
                self.Shows()[i].IsChecked(value);
            }
        },
        owner: self
    });
    self.addShow = function () {
        self.AllShows.push(new ShowViewModel({ Id: 0, EventMetadataId: self.EventMetadataId(), VenueId: self.Id(), State: 1, StartDate: '', EndDate: '' }));
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
    self.DisplayDate = ko.observable();

    self.Host = ko.observable();
    self.AllVenues = ko.observableArray();
    self.AllHosts = ko.observableArray();
    self.OtherVenues = ko.observableArray();
    
    self.selectedItem = ko.observable();
    self.selectedVenue = ko.observable();

    self.loadData = function (data) {
        self.Id(data.Id);
        self.UserId(data.UserId);

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
        self.DisplayDate(data.DisplayDate);
        
        self.AllVenues($.map(data.AllVenues, function (item) { return new EntityViewModel(item); }));

        if (data.Host != null) {
            var item = new EntityViewModel(data.Host);
            self.Host(item);
            self.AllHosts.push(item);
        }
    };

    self.loadData(data);

    self.Venues = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllVenues(), function (item) {
            return item.State() != 2 && item.State() != 3;
        });
    }, self);
    //self.OtherVenues = ko.computed(function () {
    //    return ko.utils.arrayFilter(self.AllVenues(), function (item) {
    //        return item.State() == 3;
    //    });
    //}, self);
    self.CheckedShows = ko.computed(function () {
        var venueShows = ko.utils.arrayMap(self.Venues(), function (venue) {
            return venue.CheckedShows();            
        });

        var res = new Array();
        for (var i = 0; i < venueShows.length; i++) {
            for (var j = 0; j < venueShows[i].length; j++) {
                res.push(venueShows[i][j]);
            }
        }

        return res;
    });
    self.AllVenuesChecked = ko.computed({
        read: function () {
            if (self.Venues().length == 0)
                return false;

            for (var i = 0; i < self.Venues().length; i++) {
                if (!self.Venues()[i].AllShowsChecked()) {
                    return false;
                }
            }
            return true;
        },
        write: function (value) {
            for (var i = 0; i < self.Venues().length; i++) {
                self.Venues()[i].AllShowsChecked(value);
            }
        },
        owner: self
    });
    self.CalcVenues = ko.computed(function () {
        return ko.utils.arrayFilter(self.AllVenues(), function (item) {
            return item.Id() == 0;
        });
    }, self);
    self.addVenue = function (){
        var newVenue = new EntityViewModel({ Id: 0, Type: 1, State: 1 });
        self.AllVenues.push(newVenue);
        self.selectedItem(newVenue);
    };
    self.removeVenue = function (item) {
        item.State(2);
    };
    self.cancelVenue = function (item) {
        if (self.selectedItem() && self.selectedItem().Id() == 0) {
            self.removeVenue(self.selectedItem());
        }
        self.selectedItem(null);
    };    

    self.toJSON = function() {
        var copy = ko.toJS(self); //just a quick way to get a clean copy
        delete copy.AllVenues;
        delete copy.Venues;
        delete copy.OtherVenues;
        return copy;
    };
}