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
                if (contact.isEditing === undefined) {
                    EntityViewModelExtended.initContactViewModel(contact, self);
                }
            });
        }
    });

    self.addresses.subscribe(function (addresses) {
        if (addresses) {
            addresses.forEach(function (address) {
                if (address.isEditing === undefined) {
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
            !self.addresses()[0].isEditing())
            return true;

        return false;
    }, self);

    EntityViewModelExtended.setupValidation(self, settings);
    
    self.setEditingItem = function(item) {
        if (self.addresses()) {
            self.addresses().forEach(function (address) {
                address.isEditing(item == address);
            });
        }

        if (self.contacts()) {
            self.contacts().forEach(function (contact) {
                contact.isEditing(item == contact);
            });
        }
    };

    self.contactsManager = new VmItemsManager(
        self.allContacts,
        self.setEditingItem,
        function() {
            var contact = new ContactViewModel({
                    Id: 0,
                    EntityId: self.id(),
                    Type: ContactType.Url,
                    State: VmItemState.Added
                });
            return contact;
        });
    
    self.addressesManager = new VmItemsManager(
        self.allAddresses,
        self.setEditingItem,
        function () {
            var address = new AddressViewModel({
                Id: 0,
                EntityId: self.id(),
                State: VmItemState.Added,
                Address: ""
            });
            return address;
        });
    
    self.getContactView = function (contact) {
        return contact.isEditing()
            ? self.settings.contactEditView : self.settings.contactView;
    };

    self.getContactType = function (contact) {
        return self.settings.contactTypes[contact.type()];
    };
    
    self.getAddressView = function (address) {
        return address.isEditing()
            ? self.settings.addressEditView : self.settings.addressView;
    };

    self.getMapLink = function () {
        if (self.addresses() && self.addresses().length > 0) {
            var addr = self.addresses()[0];
            return addr.getMapLink();
        }

        return "";
    };

    self.saveEntity = function (resultHandler) {
        if (self.isValidating()) {
            setTimeout(function () { self.saveEntity(); }, 50);
            return false;
        }

        if (self.errors().length == 0) {
            ajaxJsonRequest(self.toJSON(), self.settings.entitySaveUrl,
                function (entityData) {
                    if (resultHandler && $.isFunction(resultHandler)) {
                        resultHandler(entityData);
                    } else {
                        self.settings.entityAfterSaveUrlHandler(entityData.Id);
                    }
                },
                function () {
                    // TODO: To show error message
                }
            );
        } else {
            self.errors.showAllMessages();
        }

        return true;
    };
};

inherits(EntityViewModelExtended, EntityViewModel);

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
    contact.isEditing = ko.observable(false);
    contact.isEditing.subscribe(function (isEditing) {
        VmItemsManager.processIsEditingChange(
            contact,
            isEditing,
            entity.contactsManager
        );
    });
    
    EntityViewModelExtended.setupContactValidation(contact, entity.settings);
};

EntityViewModelExtended.initAddressViewModel = function (address, entity) {
    address.isEditing = ko.observable(false);
    address.isEditing.subscribe(function (isEditing) {
        VmItemsManager.processIsEditingChange(
            address,
            isEditing,
            entity.addressesManager
        );
    });

    EntityViewModelExtended.setupAddressValidation(address, entity.settings);
};