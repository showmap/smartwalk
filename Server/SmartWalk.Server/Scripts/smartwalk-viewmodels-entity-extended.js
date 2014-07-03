EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self.contacts = ko.computed(function () {
        return self.allContacts()
            ? VmItemUtil.AvailableItems(self.allContacts()) : undefined;
    });

    self.addresses = ko.computed(function () {
        return self.allAddresses()
            ? VmItemUtil.AvailableItems(self.allAddresses()) : undefined;
    });

    self.contacts().forEach(function (contact) {
        EntityViewModelExtended.initContactViewModel(contact, self);
    });

    self.addresses().forEach(function (address) {
        EntityViewModelExtended.initAddressViewModel(address, self);
    });

    self.contacts.subscribe(function (contacts) {
        if (contacts) {
            contacts.forEach(function (contact) {
                if (contact.IsEditing === undefined) {
                    EntityViewModelExtended.initContactViewModel(contact, self);
                }
            });
        }
    });

    self.addresses.subscribe(function (addresses) {
        if (addresses) {
            addresses.forEach(function (address) {
                if (address.IsEditing === undefined) {
                    EntityViewModelExtended.initAddressViewModel(address, self);
                }
            });
        }
    });

    self.isMapVisible = ko.computed(function () {
        if (self.addresses() && self.addresses().length > 1)
            return true;

        if (self.addresses() &&
            self.addresses().length == 1 &&
            !self.addresses()[0].IsEditing())
            return true;

        return false;
    }, self);

    self.initSelectedItem(self);
    self.setupValidation(self, settings);
};

EntityViewModelExtended.ENTITY_CANCEL_EVENT = "OnEntityCancelled";
EntityViewModelExtended.ENTITY_SAVE_EVENT = "OnEntitySaved";

inherits(EntityViewModelExtended, EntityViewModel);

EntityViewModelExtended.prototype.initSelectedItem = function (model) {
    model._selectedItem = ko.observable();
    model.selectedItem = ko.computed({
        read: function () {
            return model._selectedItem();
        },
        write: function (value) {
            model._selectedItem(value);

            if (model.addresses()) {
                model.addresses().forEach(function (address) {
                    address.IsEditing(value == address);
                });
            }

            if (model.contacts()) {
                model.contacts().forEach(function (contact) {
                    contact.IsEditing(value == contact);
                });
            }
        }
    }, model);
}

EntityViewModelExtended.prototype.setupValidation = function (entity, settings) {
    entity.name
        .extend({ asyncValidation: {
            validationUrl: settings.validationUrl,
            propName: "Name",
            model: entity.toJSON() // TODO: To figure if there are other ways to pass model, also traffic issues
    } });

    entity.picture
        .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });

    entity.isValidating = ko.computed(function () {
        return entity.name.isValidating() || entity.picture.isValidating();
    }, entity);

    entity.errors = ko.validation.group(entity);
}

EntityViewModelExtended.prototype.setupContactValidation = function (contact, settings) {
    contact.contact.extend({ asyncValidation: {
        validationUrl: settings.contactValidationUrl,
        propName: "Contact",
        model: $.parseJSON(ko.toJSON(contact.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    } });
    contact.title.extend({ asyncValidation: {
        validationUrl: settings.contactValidationUrl,
        propName: "Title",
        model: $.parseJSON(ko.toJSON(contact.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    } });

    contact.isValidating = ko.computed(function () {
        return contact.contact.isValidating() || contact.title.isValidating();
    }, contact);

    contact.type.extend({ dependencies: [contact.contact] });

    contact.contact
        .extend({ required: { params: true, message: settings.contactMessages.contactRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.contactMessages.contactLengthValidationMessage } })
        .extend({ contactValidation: { allowEmpty: true, contactType: contact.type, messages: settings.contactMessages } });

    contact.title
        .extend({ maxLength: { params: 255, message: settings.contactMessages.contactTitleValidationMessage } });

    contact.errors = ko.validation.group(contact);
}

EntityViewModelExtended.prototype.setupAddressValidation = function (address, settings) {
    address.address.extend({ asyncValidation: {
        validationUrl: settings.addressValidationUrl,
        propName: "Address",
        model: $.parseJSON(ko.toJSON(address.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    }
    });

    address.tip.extend({ asyncValidation: {
        validationUrl: settings.addressValidationUrl,
        propName: "Tip",
        model: $.parseJSON(ko.toJSON(address.toJSON())) // TODO: To figure if there are other ways to pass model, also traffic issues
    } });

    address.isValidating = ko.computed(function () {
        return address.address.isValidating() || address.tip.isValidating();
    }, address);

    address.address
        .extend({ required: { params: true, message: settings.addressMessages.addressRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressLengthValidationMessage } });

    address.tip
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressTipValidationMessage } });

    address.errors = ko.validation.group(address);
}

//Contacts
EntityViewModelExtended.initContactViewModel = function (contact, entity) {
    contact.IsEditing = ko.observable(false);
    entity.setupContactValidation(contact, entity.settings);
}

EntityViewModelExtended.prototype.addContact = function (root) {
    var newContactModel = new ContactViewModel(
        {
            Id: 0,
            EntityId: root.id(),
            Type: ContactType.Url,
            State: VmItemState.Added
        });

    root.allContacts.push(newContactModel);
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
    return this.settings.contactTypes[item.type()];
};

EntityViewModelExtended.prototype.cancelContact = function (item, root) {
    root.selectedItem(null);
    
    if (item.id() == 0) {
        root.allContacts.remove(item);
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

        if (root.id() != 0) {
            var ajdata = ko.toJSON(item.toJSON());

            ajaxJsonRequest(ajdata, root.settings.contactSaveUrl,
                function (data) {
                    if (item.id() == 0 || item.id() != data)
                        item.id(data);
                }
            );
        }
    } else {
        item.errors.showAllMessages();
    }    
};

//Addresses
EntityViewModelExtended.initAddressViewModel = function (address, entity) {
    address.IsEditing = ko.observable(false);
    entity.setupAddressValidation(address, entity.settings);
}

EntityViewModelExtended.prototype.addAddress = function (root) {
    var newAddressModel = new AddressViewModel({
            Id: 0,
            EntityId: root.id(),
            State: VmItemState.Added,
            Address: ""
        });

    root.allAddresses.push(newAddressModel);
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
    
    if (item.id() == 0) {
        root.allAddresses.remove(item);
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

        if (root.id() != 0) {
            var ajdata = ko.toJSON(item.toJSON());

            ajaxJsonRequest(ajdata, root.settings.addressSaveUrl,
                function(data) {
                    if (item.id() == 0 || item.id() != data)
                        item.id(data);
                }
            );
        }
    } else {
        item.errors.showAllMessages();
    }
};

// TODO: What "this" is doing in here?
EntityViewModelExtended.prototype.getMapLink = function () {
    var res = "";

    if (this.addresses().length > 0) {
        var addr = this.addresses()[0];
        return addr.getMapLink();
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
                if (root.id() == 0) root.loadData(data);

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