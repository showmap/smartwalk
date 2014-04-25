EntityViewModelBase = function (data) {
    EntityViewModelBase.superClass_.constructor.call(this);

    this.Id = ko.observable(),
    this.State = ko.observable(),
    this.Name = ko.observable(),
    this.Picture = ko.observable(),
    this.EventMetadataId = ko.observable(),
    this.Description = ko.observable(),
    
    this.AllContacts = ko.observableArray(),
    this.AllAddresses = ko.observableArray(),
    this.AllShows = ko.observableArray(),

    this.loadData(data);
};

inherits(EntityViewModelBase, ViewModelBase);

EntityViewModelBase.prototype.DisplayAddress = ko.computed(function() {
    if (!this.AllAddresses || this.AllAddresses().length <= 0)
        return null;
    return this.AllAddresses()[0];
});

EntityViewModelBase.prototype.loadCollections_ = function(data) {
    if (data.AllContacts)
        this.AllContacts($.map(data.AllContacts, function(item) { return new ContactViewModel(item); }));
    if (data.AllAddresses)
        this.AllAddresses($.map(data.AllAddresses, function(item) { return new AddressViewModel(item); }));
    if (data.AllShows)
        this.AllShows($.map(data.AllShows, function(item) { return new ShowViewModel(item); }));
};

EntityViewModelBase.prototype.loadData = function (data) {
    this.Id(data.Id);
    this.State(data.State);
    this.Name(data.Name);
    this.Picture(data.Picture);
    this.EventMetadataId(data.EventMetadataId);
    this.Description(data.Description);

    this.loadCollections_(data);
};

EntityViewModel = function (data) {
    EntityViewModelBase.call(this, data);
};

//Contacts
EntityViewModel.prototype.Contacts = ko.computed(function() {
    return this.Items_ ? this.Items_(this.AllContacts()) : null;
});

EntityViewModel.prototype.DeletedContacts = ko.computed(function() {
    return this.DeleteItem_ ? this.DeletedItems_(this.AllContacts()) : null;
});

EntityViewModel.prototype.CheckedContacts = ko.computed(function() {
    return this.CheckedItems_ ? this.CheckedItems_(this.Contacts()) : null;
});

EntityViewModel.prototype.addContact = function() {
    this.AllContacts.push(new ContactViewModel({ Id: 0, EntityId: this.Id(), Type: 1, State: 1 }));
    this.selectedItem(this.AllContacts()[this.AllContacts().length - 1]);
};

EntityViewModel.prototype.deleteContact = function(item) {
    this.DeleteItem_(item);
};

EntityViewModel.prototype.IsAnyContactSelected = ko.computed(function() {
    return this.IsAnyItemSelected_ ? this.IsAnyItemSelected_(this.Contacts()) : false;
});

EntityViewModel.prototype.AllContactsChecked = ko.computed({
    read: function() {
        return this.GetAllItemsChecked_ ? this.GetAllItemsChecked_(this.Contacts()) : false;
    },
    write: function(value) {
        if (this.SetAllItemsChecked_)
            this.SetAllItemsChecked_(this.Contacts(), value);
    },
});

//Addresses
EntityViewModel.prototype.Addresses = ko.computed(function() {
    return this.Items_ ? this.Items_(this.AllAddresses()) : null;
});

EntityViewModel.prototype.DeletedAddresses = ko.computed(function() {
    return this.DeleteItem_ ? this.DeletedItems_(this.AllAddresses()) : null;
});

EntityViewModel.prototype.CheckedAddresses = ko.computed(function() {
    return this.CheckedItems_ ? this.CheckedItems_(this.Addresses()) : null;
});

EntityViewModel.prototype.addAddress = function() {
    this.AllAddresses.push(new AddressViewModel({ Id: 0, EntityId: this.Id(), State: 1, Address: "" }));
    this.selectedItem(this.AllAddresses()[this.AllAddresses().length - 1]);
};

EntityViewModel.prototype.deleteAddress = function(item) {
    this.DeleteItem_(item);
};

EntityViewModel.prototype.IsAnyAddressSelected = ko.computed(function() {
    return this.IsAnyItemSelected_ ? this.IsAnyItemSelected_(this.Addresses()) : false;
});

EntityViewModel.prototype.AllAddressesChecked = ko.computed({
    read: function() {
        return this.GetAllItemsChecked_ ? this.GetAllItemsChecked_(this.Addresses()) : false;
    },
    write: function(value) {
        if (this.SetAllItemsChecked_)
            this.SetAllItemsChecked_(this.Addresses(), value);
    }
});

EntityViewModel.prototype.IsMapVisible = ko.computed(function() {
    if (!this.Addresses)
        return false;
    if (this.Addresses().length > 1)
        return true;
    if (this.Addresses().length == 1 && this.Addresses()[0] != this.selectedItem())
        return true;

    return false;
});
    
// Shows    
EntityViewModel.prototype.Shows = ko.computed(function() {
    return this.Items_ ? this.Items_(this.AllShows()) : null;
});

EntityViewModel.prototype.CheckedShows = ko.computed(function() {
    return this.CheckedItems_ ? this.CheckedItems_(this.Shows()) : null;
});


EntityViewModel.prototype.addShow = function() {
    this.AllShows.push(new ShowViewModel({ Id: 0, EventMetadataId: this.EventMetadataId(), VenueId: this.Id(), State: 1, StartDate: '', EndDate: '' }));
    return this.AllShows()[this.AllShows().length - 1];
};

EntityViewModel.prototype.removeShow = function(item) {
    this.DeleteItem_(item);
};

EntityViewModel.prototype.AllShowsChecked = ko.computed({
    read: function () {
        return this.GetAllItemsChecked_ ? this.GetAllItemsChecked_(this.Shows()) : null;
    },
    write: function (value) {
        if (this.SetAllItemsChecked_)
            this.SetAllItemsChecked_(this.Shows(), value);
    }
});

EntityViewModel.prototype.IsChecked = EntityViewModel.prototype.AllShowsChecked;
