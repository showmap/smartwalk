EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data, settings);

    self.settings = settings;

    self._selectedItem = ko.observable();
    self.selectedItem = ko.computed({
        read: function () {
            return self._selectedItem();
        },
        write: function (value) {
            self._selectedItem(value);

            self.AllAddresses().forEach(
                function (address) {
                    address.IsEditing(value == address);
                });

            self.AllContacts().forEach(
                function (contact) {
                    contact.IsEditing(value == contact);
                });
        }
    }, self);

    self.Name
        .extend({ asyncValidation: { validationUrl: settings.validationUrl, propName: 'Name', model: self.toJSON() } });

    self.Picture
        .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });

    self.isValidating = ko.computed(function () {
        return self.Name.isValidating() || self.Picture.isValidating();
    }, self);

    self.PrepareCollectionData = function (collection, extData) {
        if (collection && collection.length > 0) {
            for (var i = 0; i < collection.length; i++) {
                collection[i] = $.extend(collection[i], extData);
            }
        }
    };

    self.PrepareCollectionData(data.AllContacts, { messages: settings.contactMessages });
    self.PrepareCollectionData(data.AllAddresses, { messages: settings.addressMessages });
    
    self.errors = ko.validation.group(self);
};

EntityViewModelExtended.ENTITY_CANCEL_EVENT = "OnEntityCancelled";
EntityViewModelExtended.ENTITY_SAVE_EVENT = "OnEntitySaved";

inherits(EntityViewModelExtended, EntityViewModel);


//Contacts
// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.addContact = function () {
    this.AllContacts.push(new ContactViewModel({ Id: 0, EntityId: this.Id(), Type: 1, State: 1, messages: this.settings.contactMessages }));
    this.selectedItem(this.AllContacts()[this.AllContacts().length - 1]);
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.deleteContact = function (item) {
    this.DeleteItem_(item);
};

EntityViewModelExtended.prototype.getContactView = function (item, bindingContext) {
    return item.IsEditing()
        ? bindingContext.$root.settings.contactEditView
        : bindingContext.$root.settings.contactView;
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.GetContactType = function (item) {
    return this.settings.contactTypes[item.Type()];
};

EntityViewModelExtended.prototype.cancelContact = function (item, root) {
    root.selectedItem(null);
    
    if (item.Id() == 0) {
        root.AllContacts.remove(item);
    } else {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(ajdata, root.settings.contactGetUrl,
            function (data) {
                item.loadData(data);
            }
        );
    }
};

EntityViewModelExtended.prototype.saveContact = function (item, root) {
    if (item.errors().length == 0) {
        root.selectedItem(null);

        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item.toJSON());

            ajaxJsonRequest(ajdata, root.settings.contactSaveUrl,
                function (data) {
                    if (item.Id() == 0 || item.Id() != data)
                        item.Id(data);
                }
            );
        }
    } else {
        item.errors.showAllMessages();
    }    
};

//Addresses
// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.addAddress = function () {
    this.AllAddresses.push(new AddressViewModel({ Id: 0, EntityId: this.Id(), State: 1, Address: "", messages: this.settings.addressMessages }));
    this.selectedItem(this.AllAddresses()[this.AllAddresses().length - 1]);
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.deleteAddress = function (item) {
    this.DeleteItem_(item);
};

EntityViewModelExtended.prototype.getAddressView = function (item, bindingContext) {
    return item.IsEditing()
        ? bindingContext.$root.settings.addressEditView
        : bindingContext.$root.settings.addressView;
};

EntityViewModelExtended.prototype.cancelAddress = function (item, root) {
    root.selectedItem(null);
    
    if (item.Id() == 0) {
        root.AllAddresses.remove(item);
    } else {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(ajdata, root.settings.addressGetUrl,
            function(data) {
                item.loadData(data);
            }
        );
    }
};        

EntityViewModelExtended.prototype.saveAddress = function (item, root) {
    if (item.errors().length == 0) {
        root.selectedItem(null);

        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item.toJSON());

            ajaxJsonRequest(ajdata, root.settings.addressSaveUrl,
                function(data) {
                    if (item.Id() == 0 || item.Id() != data)
                        item.Id(data);
                }
            );
        }
    } else {
        item.errors.showAllMessages();
    }
};

// Shows 
// TODO: What "this" is doing in here?
EntityViewModel.prototype.addShow = function (metadata) {
    var newShow = new ShowViewModel({ Id: 0, EventMetadataId: metadata.Id(), VenueId: this.Id(), State: 1, StartDate: metadata.StartTime(), EndDate: metadata.StartTime(), messages: metadata.settings.showMessages, eventDtFrom: metadata.StartTime, eventDtTo: metadata.EndTime });
    //this.AllShows.splice(0, 0, newShow);
    this.AllShows.push(newShow);
    return newShow;
};

// TODO: What "this" is doing in here?
EntityViewModel.prototype.removeShow = function (item) {
    this.DeleteItem_(item);
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.GetMapLink = function () {
    var res = "";

    if (this.Addresses().length > 0) {
        var addr = this.Addresses()[0];
        return addr.GetMapLink();
    }

    return res;
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.cancel = function () {
    $(this.settings.entityFormName).trigger({
        type: this.settings.entityCancelEvent,
        item: this
    });
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.saveOrAdd = function (root) {
    var self = this;

    if (root.isValidating()) {
        setTimeout(function () {
            root.saveOrAdd(root);
        }, 50);
        return false;
    }

    if (root.errors().length == 0) {
        var ajdata = ko.toJSON(root.toJSON());

        ajaxJsonRequest(
            ajdata,
            this.settings.entitySaveUrl,
            function (data) {
                if (self.Id() == 0) self.loadData(data);

                $(self.settings.entityFormName).trigger({
                    type: self.settings.entitySaveEvent,
                    item: self
                });
            }
        );
    } else {
        root.errors.showAllMessages();
    }

    return true;
};