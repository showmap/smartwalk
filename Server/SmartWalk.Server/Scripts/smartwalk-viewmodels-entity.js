EntityViewModelBase = function (data) {
    var self = this;

    EntityViewModelBase.superClass_.constructor.call(self);

    self.Id = ko.observable();
    self.State = ko.observable();
    self.Type = ko.observable();
    self.Name = ko.observable();
    self.Abbreviation = ko.observable();
    self.Picture = ko.observable();
    self.EventMetadataId = ko.observable();
    self.Description = ko.observable();

    self.AllContacts = ko.observableArray();
    self.AllAddresses = ko.observableArray();
    self.AllShows = ko.observableArray();

    //Contacts computed
    self.Contacts = ko.computed(function () {
        return self.Items_(self.AllContacts());
    }, self);

    self.DeletedContacts = ko.computed(function () {
        return self.DeletedItems_(self.AllContacts());
    }, self);


    //Addresses computed
    self.Addresses = ko.computed(function () {
        return self.Items_(self.AllAddresses());
    }, self);

    self.DeletedAddresses = ko.computed(function () {
        return self.DeletedItems_(self.AllAddresses());
    }, self);

    self.IsMapVisible = ko.computed(function () {
        if (self.Addresses().length > 1)
            return true;
        if (self.Addresses().length == 1 && !self.Addresses()[0].IsEditing())
            return true;
        return false;
    }, self);

    //Shows computed
    self.Shows = ko.computed(function () {
        return self.Items_(self.AllShows());
    }, self);

    self.IsEditing = ko.observable(false);
    
    self.DisplayAddress = ko.computed(function () {
        if (!self.AllAddresses || self.AllAddresses().length <= 0)
            return '';
        return self.AllAddresses()[0];
    }, self);

  
    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            State: self.State(),
            Type: self.Type(),
            Name: self.Name(),
            Abbreviation: self.Abbreviation(),
            Picture: self.Picture(),
            EventMetadataId: self.EventMetadataId(),
            Description: self.Description(),

            AllContacts: ko.utils.arrayMap(self.AllContacts(), function (contact) { return contact.toJSON(); }),
            AllAddresses: ko.utils.arrayMap(self.AllAddresses(), function (address) { return address.toJSON(); }),
            AllShows: ko.utils.arrayMap(self.AllShows(), function (show) { return show.toJSON(); }),
        };
    }, self);

    self.loadData(data);  
};

inherits(EntityViewModelBase, ViewModelBase);

EntityViewModelBase.prototype.loadCollections_ = function (data) {
    var self = this;
    
    if (data.AllContacts)
        self.AllContacts($.map(data.AllContacts, function (item) { return new ContactViewModel(item); }));

    if (data.AllAddresses)
        self.AllAddresses($.map(data.AllAddresses, function (item) { return new AddressViewModel(item); }));

    if (data.AllShows)
        self.AllShows($.map(data.AllShows, function (item) { return new ShowViewModel(item); }));
};

EntityViewModelBase.prototype.loadData = function (data) {
    var self = this;

    self.Id(data.Id);
    self.State(data.State);
    self.Type(data.Type);
    self.Name(data.Name);
    self.Abbreviation(data.Abbreviation);
    self.Picture(data.Picture);
    self.EventMetadataId(data.EventMetadataId);
    self.Description(data.Description);

    self.loadCollections_(data);
};

EntityViewModel = function (data) {
    EntityViewModel.superClass_.constructor.call(this, data);        
};

inherits(EntityViewModel, EntityViewModelBase);