﻿EntityViewModelExtended = function (settings, data) {
    var self = this;

    EntityViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    self.model = new EntityViewModel(data);

    self.contactsManager = new VmItemsManager(
        self.model.contacts,
        function() {
            var contact = new ContactViewModel({ Type: sw.vm.ContactType.Url });
            return contact;
        },
        {
            beforeEdit: function () {
                self.addressesManager.cancelAll();
            },
            beforeSave: function (contact) {
                if (!contact.errors) {
                    EntityViewModelExtended.setupContactValidation(contact, self.settings);
                }
            },
            itemView: self.settings.contactView,
            itemEditView: self.settings.contactEditView
        });
    
    self.addressesManager = new VmItemsManager(
        self.model.addresses,
        function () {
            var address = new AddressViewModel({});
            return address;
        },
        {
            initItem: function (address) {
                EntityViewModelExtended.initAddressViewModel(address);
            },
            beforeEdit: function () {
                self.contactsManager.cancelAll();
            },
            beforeSave: function (address) {
                if (!address.errors) {
                    EntityViewModelExtended.setupAddressValidation(address, self.settings);
                }
            },
            itemView: self.settings.addressView,
            itemEditView: self.settings.addressEditView
        });

    self.uploadManager = new FileUploadManager(self, self.model.picture);

    self.getContactType = function (contact) {
        return self.settings.contactTypes[contact.type()];
    };

    self.model.toTinyJSON = function () {
        return {
            Id: self.model.id(),
            Type: self.model.type(),
            Name: self.model.name()
        };
    };

    self.saveEntity = function (resultHandler) {
        if (!self.model.errors) {
            EntityViewModelExtended.setupValidation(self.model, settings);
        }

        if (self.model.isValidating()) {
            setTimeout(function () { self.saveEntity(resultHandler); }, 50);
            return false;
        }

        if (self.model.errors().length == 0) {
            self.isBusy(true); // explicitly setting busy in case if image is being uploaded
            self.request(self.uploadManager.request());
            self.request().done(function () { self._saveEntity(resultHandler); });
        } else {
            self.model.errors.showAllMessages();
        }

        return true;
    };

    self._saveEntity = function(resultHandler) {
        self.request(sw.ajaxJsonRequest(
                self.model.toJSON(), self.settings.entitySaveUrl, self)
            .done(function(entityData) {
                if (resultHandler && $.isFunction(resultHandler)) {
                    resultHandler(entityData);
                } else {
                    data = entityData;
                    self.model.loadData(entityData);
                    self.settings.entityAfterSaveAction(entityData.Id);
                }
            })
            .fail(function(errorResult) {
                self.handleServerError(errorResult);
            }));
    };
    
    self.cancelEntity = function () {
        if (self.isBusy()) {
            self.isBusy(false);
        } else {
            self.settings.entityAfterCancelAction();
        }
    };

    self.onWindowClose = function () {
        var initModel = new EntityViewModel(data);

        if (JSON.stringify(initModel.toJSON()) != JSON.stringify(self.model.toJSON())) {
            return settings.unsavedChangesMessage;
        }

        return undefined;
    };

    EntityViewModelExtended.setupAutocompleteAddress(self);
};

sw.inherits(EntityViewModelExtended, ViewModelBase);

// Static Methods
EntityViewModelExtended.setupValidation = function (entity, settings) {
    entity.name
        .extend({
            required: {
                message: settings.nameRequiredValidationMessage
            }
        })
        .extend({
            maxLength: {
                params: 255,
                message: settings.nameLengthValidationMessage
            }
        })
        .extend({
            asyncValidation: {
                validationUrl: settings.validationUrl,
                propName: "Name",
                modelHandler: entity.toTinyJSON
            }
        });
    
    entity.description
        .extend({
            maxLength: {
                params: 3000,
                message: settings.descriptionLengthValidationMessage
            }
        });

    entity.isValidating = ko.computed(function () {
        return entity.name.isValidating() ||
            entity.description.isValidating();
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
        .extend({ min: -90 })
        .extend({ max: 90 })
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

EntityViewModelExtended.setupAutocompleteAddress = function (model) {
    if (typeof google !== "undefined" && typeof google.maps !== "undefined") {
        model.geocoder = new google.maps.Geocoder();
        model.getAutocompleteAddresses = function(searchAddr, callback) {
            model.geocoder.geocode({ "address": searchAddr }, function(results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    callback(results);
                } else {
                    callback(null);
                    // TODO: Notify about error somehow
                }
            });
        };
    } else {
        model.getAutocompleteAddresses = function () { return null; };
    }
};

EntityViewModelExtended.initAddressViewModel = function (address) {
    address.mapPoint = ko.computed({
        read: function () {
            return address.latitude() && address.longitude()
                ? { lat: address.latitude(), lng: address.longitude() }
                : undefined;
        },
        write: function (point) {
            address.latitude(point ? point.lat : undefined);
            address.longitude(point ? point.lng : undefined);
        }
    });
    
    address.addressData = ko.computed({
        read: function () {
            return { "formatted_address": address.address() || null };
        },
        write: function (data) {
            if (data && data.geometry && data.geometry.location) {
                address.mapPoint(ko.googleMap.toSWLatLng(data.geometry.location));
            }
        }
    });
};