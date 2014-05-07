EntityViewModelExtended = function (settings, data) {    
    this.setupValidations = function (model) {
        this.Name.extend({ asyncValidation: { validationUrl: settings.validationUrl, propName: 'Name', model: model.toJSON() } });
        this.Picture.extend({ asyncValidation: { validationUrl: settings.validationUrl, propName: 'Picture', model: model.toJSON() } });
        
        this.isValidating = ko.computed(function () {
            return this.Name.isValidating() || this.Picture.isValidating();
        }, this);
    };

    this.PrepareCollectionData = function(collection, extData) {
        if (collection && collection.length > 0) {
            for (var i = 0; i < collection.length; i++) {
                collection[i] = $.extend(collection[i], extData);
            }
        }
    };
    
    this.PrepareCollectionData(data.AllContacts, { validationUrl: settings.contactValidationUrl });
    this.PrepareCollectionData(data.AllAddresses, { validationUrl: settings.addressValidationUrl });

    EntityViewModelExtended.superClass_.constructor.call(this, data, settings);

    this.generalValidationMessage = settings.generalValidationMessage;

    this.entityFormName = settings.entityFormName;
    
    this.entityCancelEvent = settings.entityCancelEvent;
    this.entitySaveEvent = settings.entitySaveEvent;
    
    this.entitySaveUrl = settings.entitySaveUrl;

    this.addressGetUrl = settings.addressGetUrl;
    this.addressSaveUrl = settings.addressSaveUrl;
    this.addressDeleteUrl = settings.addressDeleteUrl;
    this.addressValidationUrl = settings.addressValidationUrl;
    this.addressView = settings.addressView;
    this.addressEditView = settings.addressEditView;

    this.contactGetUrl = settings.contactGetUrl;
    this.contactSaveUrl = settings.contactSaveUrl;
    this.contactDeleteUrl = settings.contactDeleteUrl;
    this.contactValidationUrl = settings.contactValidationUrl;
    this.contactView = settings.contactView;
    this.contactEditView = settings.contactEditView;
    this.contactTypes = settings.contactTypes;
    
    this.errors = ko.validation.group(this);
};

EntityViewModelExtended.VENUE_CANCEL_EVENT = "OnVenueCancelled";
EntityViewModelExtended.VENUE_SAVE_EVENT = "OnVenueSaved";

EntityViewModelExtended.HOST_CANCEL_EVENT = "OnHostCancelled";
EntityViewModelExtended.HOST_SAVE_EVENT = "OnHostSaved";

inherits(EntityViewModelExtended, EntityViewModel);

EntityViewModelExtended.prototype.getContactView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.contactEditView : bindingContext.$root.contactView;
};
        
EntityViewModelExtended.prototype.GetContactType = function (item) {
    return this.contactTypes[item.Type()];
};

EntityViewModelExtended.prototype.cancelContact = function (item, root) {
    root.selectedItem(null);
    
    if (item.Id() == 0) {
        root.AllContacts.remove(item);
    } else {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.contactGetUrl,
            function (data) {
                item.loadData(data);
            }
        );
    }
};

EntityViewModelExtended.prototype.saveContact = function (item, root) {
    if (item.isValidating()) {
        setTimeout(function () {
            root.saveContact(item, root);
        }, 50);
        return false;
    }

    if (item.errors().length == 0) {
        root.selectedItem(null);

        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item);

            ajaxJsonRequest(ajdata, root.contactSaveUrl,
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

EntityViewModelExtended.prototype.deleteContacts = function (root) {
    this.selectedItem(null);
    
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(root.CheckedContacts);

        ajaxJsonRequest(ajdata, root.contactDeleteUrl,
            function (data) {
                while (root.CheckedContacts().length > 0){
                    root.AllContacts.remove(root.CheckedContacts()[0]);
                }
            }
        );
    }
};

EntityViewModelExtended.prototype.getAddressView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.addressEditView : bindingContext.$root.addressView;
};

EntityViewModelExtended.prototype.cancelAddress = function (item, root) {
    root.selectedItem(null);
    
    if (item.Id() == 0) {
        root.AllAddresses.remove(item);
    } else {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.addressGetUrl,
            function(data) {
                item.loadData(data);
            }
        );
    }
};        

EntityViewModelExtended.prototype.saveAddress = function (item, root) {
    if (item.isValidating()) {
        setTimeout(function () {
            root.saveContact(item, root);
        }, 50);
        return false;
    }

    if (item.errors().length == 0) {
        root.selectedItem(null);

        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item);

            ajaxJsonRequest(ajdata, root.addressSaveUrl,
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

EntityViewModelExtended.prototype.deleteAddresses = function (root) {    
    this.selectedItem(null);

    if (root.Id() != 0) {
        var ajdata = ko.toJSON(root.CheckedAddresses);

        ajaxJsonRequest(ajdata, root.addressDeleteUrl,
            function (data) {
                while (root.CheckedAddresses().length > 0){
                    root.AllAddresses.remove(root.CheckedAddresses()[0]);
                }
            }
        );
    }
};        

EntityViewModelExtended.prototype.GetMapLink = function () {
    var res = "";

    if (this.Addresses().length > 0) {
        var addr = this.Addresses()[0];
        return addr.GetMapLink();
    }

    return res;
};

EntityViewModelExtended.prototype.cancel = function () {
    $(this.entityFormName).trigger({
        type: this.entityCancelEvent
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
        var ajdata = ko.toJSON(root);
        var self = this;

        ajaxJsonRequest(ajdata, this.entitySaveUrl,
            function (data) {
                if (self.Id() == 0)
                    self.loadData(data);
                $(self.entityFormName).trigger({
                    type: self.entitySaveEvent,
                    item: self
                });
            }
        );
    } else {
        alert(root.generalValidationMessage);
        root.errors.showAllMessages();
    }
};