EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self.contacts = ko.computed(function () {
        return self.allContacts()
            ? VmItemUtil.availableItems(self.allContacts()) : undefined;
    });

    self.addresses = ko.computed(function () {
        return self.allAddresses()
            ? VmItemUtil.availableItems(self.allAddresses()) : undefined;
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

    self._selectedItem = ko.observable();
    self.selectedItem = ko.computed({
        read: function () {
            return self._selectedItem();
        },
        write: function (value) {
            self._selectedItem(value);

            if (self.addresses()) {
                self.addresses().forEach(function (address) {
                    address.IsEditing(value == address);
                });
            }

            if (self.contacts()) {
                self.contacts().forEach(function (contact) {
                    contact.IsEditing(value == contact);
                });
            }
        }
    });

    EntityViewModelExtended.setupValidation(self, settings);

    // Contacts
    self.addContact = function () {
        var contact = new ContactViewModel(
            {
                Id: 0,
                EntityId: self.id(),
                Type: ContactType.Url,
                State: VmItemState.Added
            });

        self.allContacts.push(contact);
        self.selectedItem(contact);
    };

    self.deleteContact = function (contact) {
        VmItemUtil.deleteItem(contact);
    };

    self.getContactView = function (contact) {
        return contact.IsEditing()
            ? self.settings.contactEditView : self.settings.contactView;
    };

    self.getContactType = function (contact) {
        return self.settings.contactTypes[contact.type()];
    };

    self.cancelContact = function (contact) {
        self.selectedItem(null);

        if (contact.id() == 0) {
            self.allContacts.remove(contact);
        } else {
            ajaxJsonRequest(contact.toJSON(), self.settings.contactGetUrl,
                function (contactData) {
                    contact.loadData(contactData);
                }
            );
        }
    };

    self.saveContact = function (contact) {
        if (contact.errors().length == 0) {
            self.selectedItem(null);

            if (self.id() != 0) {
                ajaxJsonRequest(contact.toJSON(), self.settings.contactSaveUrl,
                    function (contactId) {
                        if (contact.id() == 0 || contact.id() != contactId) {
                            contact.id(contactId);
                        }
                    }
                );
            }
        } else {
            contact.errors.showAllMessages();
        }
    };

    // Addresses
    self.addAddress = function () {
        var address = new AddressViewModel({
            Id: 0,
            EntityId: self.id(),
            State: VmItemState.Added,
            Address: ""
        });

        self.allAddresses.push(address);
        self.selectedItem(address);
    };

    self.deleteAddress = function (contact) {
        VmItemUtil.deleteItem(contact);
    };

    self.getAddressView = function (address) {
        return address.IsEditing()
            ? self.settings.addressEditView : self.settings.addressView;
    };

    self.cancelAddress = function (address) {
        self.selectedItem(null);

        if (address.id() == 0) {
            self.allAddresses.remove(address);
        } else {
            ajaxJsonRequest(address.toJSON(), self.settings.addressGetUrl,
                function (addressData) {
                    address.loadData(addressData);
                }
            );
        }
    };

    self.saveAddress = function (address) {
        if (address.errors().length == 0) {
            self.selectedItem(null);

            if (self.id() != 0) {
                ajaxJsonRequest(address.toJSON(), self.settings.addressSaveUrl,
                    function (addressId) {
                        if (address.id() == 0 || address.id() != addressId) {
                            address.id(addressId);
                        }
                    }
                );
            }
        } else {
            address.errors.showAllMessages();
        }
    };

    self.getMapLink = function () {
        if (self.addresses() && self.addresses().length > 0) {
            var addr = self.addresses()[0];
            return addr.getMapLink();
        }

        return "";
    };

    self.cancel = function () {
        $(self.settings.entityFormName).trigger({
            type: self.settings.entityCancelEvent,
            item: self
        });
    };

    self.saveOrAdd = function () {
        if (self.isValidating()) {
            setTimeout(function () {
                self.saveOrAdd(self);
            }, 50);
            return false;
        }

        if (self.errors().length == 0) {
            ajaxJsonRequest(self.toJSON(), self.settings.entitySaveUrl,
                function (entityData) {
                    if (self.id() == 0) self.loadData(entityData);

                    $(self.settings.entityFormName).trigger({
                        type: self.settings.entitySaveEvent,
                        item: self
                    });
                }
            );
        } else {
            self.errors.showAllMessages();
        }

        return true;
    };
};

inherits(EntityViewModelExtended, EntityViewModel);

EntityViewModelExtended.ENTITY_CANCEL_EVENT = "OnEntityCancelled";
EntityViewModelExtended.ENTITY_SAVE_EVENT = "OnEntitySaved";

// Static Methods
EntityViewModelExtended.setupValidation = function (entity, settings) {
    entity.name
        .extend({
            asyncValidation: {
                validationUrl: settings.validationUrl,
                propName: "Name",
                modelHandler: entity.toJSON
            }
        });

    entity.picture
        .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });

    entity.isValidating = ko.computed(function () {
        return entity.name.isValidating() || entity.picture.isValidating();
    }, entity);

    entity.errors = ko.validation.group(entity);
};

EntityViewModelExtended.setupContactValidation = function (contact, settings) {
    contact.contact.extend({
        asyncValidation: {
            validationUrl: settings.contactValidationUrl,
            propName: "Contact",
            modelHandler: contact.toJSON
        }
    });
    contact.title.extend({
        asyncValidation: {
            validationUrl: settings.contactValidationUrl,
            propName: "Title",
            modelHandler: contact.toJSON
        }
    });

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
};

EntityViewModelExtended.setupAddressValidation = function (address, settings) {
    address.address.extend({
        asyncValidation: {
            validationUrl: settings.addressValidationUrl,
            propName: "Address",
            modelHandler: address.toJSON
        }
    });

    address.tip.extend({
        asyncValidation: {
            validationUrl: settings.addressValidationUrl,
            propName: "Tip",
            modelHandler: address.toJSON
        }
    });

    address.isValidating = ko.computed(function () {
        return address.address.isValidating() || address.tip.isValidating();
    }, address);

    address.address
        .extend({ required: { params: true, message: settings.addressMessages.addressRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressLengthValidationMessage } });

    address.tip
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressTipValidationMessage } });

    address.errors = ko.validation.group(address);
};

EntityViewModelExtended.initContactViewModel = function (contact, entity) {
    contact.IsEditing = ko.observable(false);
    EntityViewModelExtended.setupContactValidation(contact, entity.settings);
};

EntityViewModelExtended.initAddressViewModel = function (address, entity) {
    address.IsEditing = ko.observable(false);
    EntityViewModelExtended.setupAddressValidation(address, entity.settings);
};