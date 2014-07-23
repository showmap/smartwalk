EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    self.serverErrorsManager = new ServerErrorsManager();

    self.setEditingItem = function (item) {
        if (self.addressesManager.items()) {
            self.addressesManager.items().forEach(function (address) {
                address.isEditing(item == address);
            });
        }

        if (self.contactsManager.items()) {
            self.contactsManager.items().forEach(function (contact) {
                contact.isEditing(item == contact);
            });
        }
    };

    self.contactsManager = new VmItemsManager(
        self.contacts,
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
        self.addresses,
        function () {
            var address = new AddressViewModel({});
            return address;
        },
        {
            setEditingItem: self.setEditingItem,
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

    self.getMapLink = function () {
        if (self.addressesManager.items() &&
            self.addressesManager.items().length > 0) {
            var addr = self.addressesManager.items()[0];
            return addr.getMapLink();
        }

        return "";
    };

    self.toTinyJSON = function() {
        return {
            Id: self.id(),
            Type: self.type(),
            Name: self.name()
        };
    };

    self.saveEntity = function (resultHandler) {
        if (!self.errors) {
            EntityViewModelExtended.setupValidation(self, settings);
        }

        if (self.isValidating()) {
            setTimeout(function () { self.saveEntity(resultHandler); }, 50);
            return false;
        }

        if (self.errors().length == 0) {
            ajaxJsonRequest(self.toJSON(), self.settings.entitySaveUrl,
                function (entityData) {
                    if (resultHandler && $.isFunction(resultHandler)) {
                        resultHandler(entityData);
                    } else {
                        self.settings.entityAfterSaveAction(entityData.Id);
                    }
                },
                function (errorResult) {
                    self.serverErrorsManager.handleError(errorResult);
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
        .extend({ required: { params: true, message: settings.addressMessages.addressRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressLengthValidationMessage } });

    address.tip
        .extend({ maxLength: { params: 255, message: settings.addressMessages.addressTipValidationMessage } });

    address.errors = ko.validation.group(address);
};