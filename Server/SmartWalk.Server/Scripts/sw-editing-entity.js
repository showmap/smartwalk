EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    self.data = new EntityViewModel(data);

    self.setEditingItem = function (item) {
        if (self.data.addresses()) {
            self.data.addresses().forEach(function (address) {
                address.isEditing(item == address);
            });
        }

        if (self.data.contacts()) {
            self.data.contacts().forEach(function (contact) {
                contact.isEditing(item == contact);
            });
        }
    };

    self.contactsManager = new VmItemsManager(
        self.data.contacts,
        function() {
            var contact = new ContactViewModel({ Type: ContactType.Url });
            return contact;
        },
        {
            setEditingItem: self.setEditingItem,
            beforeSave: function (contact) {
                if (!contact.errors) {
                    EntityViewModelExtended.setupContactValidation(contact, self.settings);
                }
            },
            itemView: self.settings.contactView,
            itemEditView: self.settings.contactEditView
        });
    
    self.addressesManager = new VmItemsManager(
        self.data.addresses,
        function () {
            var address = new AddressViewModel({});
            return address;
        },
        {
            setEditingItem: self.setEditingItem,
            initItem: function (address) {
                EntityViewModelExtended.initAddressViewModel(address);
            },
            beforeSave: function (address) {
                if (!address.errors) {
                    EntityViewModelExtended.setupAddressValidation(address, self.settings);
                }
            },
            itemView: self.settings.addressView,
            itemEditView: self.settings.addressEditView
        });

    self.getContactType = function (contact) {
        return self.settings.contactTypes[contact.type()];
    };

    self.data.toTinyJSON = function () {
        return {
            Id: self.data.id(),
            Type: self.data.type(),
            Name: self.data.name()
        };
    };

    self.saveEntity = function (resultHandler) {
        if (!self.data.errors) {
            EntityViewModelExtended.setupValidation(self.data, settings);
        }

        if (self.data.isValidating()) {
            setTimeout(function () { self.saveEntity(resultHandler); }, 50);
            return false;
        }

        if (self.data.errors().length == 0) {
            self.currentRequest = sw.ajaxJsonRequest(
                self.data.toJSON(),
                self.settings.entitySaveUrl,
                function (entityData) {
                    if (resultHandler && $.isFunction(resultHandler)) {
                        resultHandler(entityData);
                    } else {
                        self.settings.entityAfterSaveAction(entityData.Id);
                    }
                },
                function (errorResult) {
                    self.handleServerError(errorResult);
                },
                self
            );
        } else {
            self.data.errors.showAllMessages();
        }

        return true;
    };
    
    self.cancelEntity = function () {
        if (self.isBusy()) {
            self.isBusy(false);
        } else {
            self.settings.entityAfterCancelAction();
        }
    };
};

sw.inherits(EntityViewModelExtended, ViewModelBase);

// Static Methods
EntityViewModelExtended.setupValidation = function (entity, settings) {
    entity.name
        .extend({ required: {
            message: settings.nameRequiredValidationMessage
        } })
        .extend({
            asyncValidation: {
                validationUrl: settings.validationUrl,
                propName: "Name",
                modelHandler: entity.toTinyJSON
            }
        });

    entity.picture
        .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });

    entity.isValidating = ko.computed(function () {
        return entity.name.isValidating() || entity.picture.isValidating();
    });

    entity.errors = ko.validation.group(entity);
};

EntityViewModelExtended.setupContactValidation = function (contact, settings) {
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
    address.address
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressLengthValidationMessage } });
    
    address.latitude
        .extend({ number: true })
        .extend({ notEqual: "0" })
        .extend({ min: -85 })
        .extend({ max: 85 })
        .extend({ required: { params: true, message: settings.addressMessages.addressLatitudeValidationMessage } });
    address.longitude
        .extend({ number: true })
        .extend({ notEqual: "0" })
        .extend({ min: -180 })
        .extend({ max: 180 })
        .extend({ required: { params: true, message: settings.addressMessages.addressLongitudeValidationMessage } });

    address.tip
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressTipValidationMessage } });

    address.errors = ko.validation.group(address);
};

EntityViewModelExtended.initAddressViewModel = function (address) {
    address.mapPoint = ko.computed({
        read: function () {
            if (address.latitude() && address.longitude())
                return L.latLng(address.latitude(), address.longitude());

            return L.latLng(12321, 79846);
        },
        write: function(newValue) {
            address.latitude(newValue.lat);
            address.longitude(newValue.lng);
        }
    });
};