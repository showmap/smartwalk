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

function EntityViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.Type = ko.observable(data.Type);

    self.Name = ko.observable(data.Name);
    self.Description = ko.observable(data.Description);
    self.Picture = ko.observable(data.Picture);

    self.AllContacts = ko.observableArray($.map(data.AllContacts, function (item) { return new ContactViewModel(item); }));
    self.AllAddresses = ko.observableArray($.map(data.AllAddresses, function (item) { return new AddressViewModel(item); }));

    self.Addresses = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllAddresses(), function (item) {
            return item.State() != 2;
        });
    }, this);

    self.Contacts = ko.computed(function () {        
        return ko.utils.arrayFilter(this.AllContacts(), function (item) {
            return item.State() != 2;
        });
    }, this);

    // Operations
    self.addContact = function () {
        self.AllContacts.push(new ContactViewModel({ Id: 0, HostId: 0, Type: 1, State: 1 }));
    };

    self.removeContact = function (item) {
        item.State(2);
    };
    
    self.addAddress = function () {
        self.AllAddresses.push(new AddressViewModel({ Id: 0, State: 1 }));
    };

    self.removeAddress = function (item) {
        item.State(2);
    };
}