EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self._selectedItem = ko.observable();
    self.selectedItem = ko.computed({
        read: function () {
            return self._selectedItem();
        },
        write: function (value) {
            self._selectedItem(value);

            if (self.AllAddresses()) {
                self.AllAddresses().forEach(function(address) {
                    address.IsEditing(value == address);
                });
            }

            if (self.AllContacts()) {
                self.AllContacts().forEach(function(contact) {
                    contact.IsEditing(value == contact);
                });
            }
        }
    }, self);

    self.AllContacts().forEach(function (contact) {
        EntityViewModelExtended.initContactViewModel(contact, self);
    });

    self.AllAddresses().forEach(function (address) {
        EntityViewModelExtended.initAddressViewModel(address, self);
    });

    self.Contacts = ko.computed(function () {
        return VmItemUtil.AvailableItems(self.AllContacts());
    }, self);

    self.Addresses = ko.computed(function () {
        return VmItemUtil.AvailableItems(self.AllAddresses());
    }, self);

    self.IsMapVisible = ko.computed(function () {
        if (self.Addresses() && self.Addresses().length > 1)
            return true;

        if (self.Addresses() &&
            self.Addresses().length == 1 &&
            !self.Addresses()[0].IsEditing())
            return true;

        return false;
    }, self);

    self.setupValidation(self, settings);
};

EntityViewModelExtended.ENTITY_CANCEL_EVENT = "OnEntityCancelled";
EntityViewModelExtended.ENTITY_SAVE_EVENT = "OnEntitySaved";

inherits(EntityViewModelExtended, EntityViewModel);

EntityViewModelExtended.prototype.setupValidation = function (model, settings) {
    model.Name
        .extend({ asyncValidation: {
            validationUrl: settings.validationUrl,
            propName: "Name",
            model: model.toJSON() // TODO: To figure if there are other ways to pass model, also traffic issues
    } });

    model.Picture
        .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });

    model.isValidating = ko.computed(function () {
        return model.Name.isValidating() || model.Picture.isValidating();
    }, model);

    model.errors = ko.validation.group(model);
}

EntityViewModelExtended.prototype.setupContactValidation = function (contactModel, settings) {
    contactModel.Contact.extend({ asyncValidation: {
        validationUrl: settings.contactValidationUrl,
        propName: "Contact",
        model: $.parseJSON(ko.toJSON(contactModel.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    } });
    contactModel.Title.extend({ asyncValidation: {
        validationUrl: settings.contactValidationUrl,
        propName: "Title",
        model: $.parseJSON(ko.toJSON(contactModel.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    } });

    contactModel.isValidating = ko.computed(function () {
        return contactModel.Contact.isValidating() || contactModel.Title.isValidating();
    }, contactModel);

    contactModel.Type.extend({ dependencies: [contactModel.Contact] });

    contactModel.Contact
        .extend({ required: { params: true, message: settings.contactMessages.contactRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.contactMessages.contactLengthValidationMessage } })
        .extend({ contactValidation: { allowEmpty: true, contactType: contactModel.Type, messages: settings.contactMessages } });

    contactModel.Title
        .extend({ maxLength: { params: 255, message: settings.contactMessages.contactTitleValidationMessage } });

    contactModel.errors = ko.validation.group(contactModel);
}

EntityViewModelExtended.prototype.setupAddressValidation = function (addressModel, settings) {
    addressModel.Address.extend({ asyncValidation: {
        validationUrl: settings.addressValidationUrl,
        propName: "Address",
        model: $.parseJSON(ko.toJSON(addressModel.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    }
    });

    addressModel.Tip.extend({ asyncValidation: {
        validationUrl: settings.addressValidationUrl,
        propName: "Tip",
        model: $.parseJSON(ko.toJSON(addressModel.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    } });

    addressModel.isValidating = ko.computed(function () {
        return addressModel.Address.isValidating() || addressModel.Tip.isValidating();
    }, addressModel);

    addressModel.Address
        .extend({ required: { params: true, message: settings.addressMessages.addressRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressLengthValidationMessage } });

    addressModel.Tip
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressTipValidationMessage } });

    addressModel.errors = ko.validation.group(addressModel);
}

//Contacts
EntityViewModelExtended.createContactViewModel = function (contactData, entityModel) {
    var model = new ContactViewModel(contactData);
    EntityViewModelExtended.initContactViewModel(model, entityModel);
    return model;
}

EntityViewModelExtended.initContactViewModel = function (model, entityModel) {
    model.IsEditing = ko.observable(false);
    entityModel.setupContactValidation(model, entityModel.settings);
}

EntityViewModelExtended.prototype.addContact = function (root) {
    var newContactModel =
        EntityViewModelExtended.createContactViewModel(
        {
            Id: 0,
            EntityId: root.Id(),
            Type: 1,
            State: VmItemState.Added
        },
        root);

    root.AllContacts.push(newContactModel);
    root.selectedItem(newContactModel);
};

EntityViewModelExtended.prototype.deleteContact = function (item) {
    VmItemUtil.DeleteItem(item);
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
EntityViewModelExtended.createAddressViewModel = function (addressData, entityModel) {
    var model = new AddressViewModel(addressData);
    EntityViewModelExtended.initAddressViewModel(model, entityModel);
    return model;
}

EntityViewModelExtended.initAddressViewModel = function (model, entityModel) {
    model.IsEditing = ko.observable(false);
    entityModel.setupAddressValidation(model, entityModel.settings);
}

EntityViewModelExtended.prototype.addAddress = function (root) {
    var newAddressModel =
        EntityViewModelExtended.createAddressViewModel({
            Id: 0,
            EntityId: root.Id(),
            State: VmItemState.Added,
            Address: ""
        },
        root);

    root.AllAddresses.push(newAddressModel);
    root.selectedItem(newAddressModel);
};

EntityViewModelExtended.prototype.deleteAddress = function (item) {
    VmItemUtil.DeleteItem(item);
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

EntityViewModelExtended.prototype.saveOrAdd = function (root) {
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
            root.settings.entitySaveUrl,
            function (data) {
                if (root.Id() == 0) root.loadData(data);

                $(root.settings.entityFormName).trigger({
                    type: root.settings.entitySaveEvent,
                    item: root
                });
            }
        );
    } else {
        root.errors.showAllMessages();
    }

    return true;
};