EntityViewModelExtended = function (settings, data) {    
    this.setupValidations = function (model) {
        this.Name.extend({ asyncValidation: { validationUrl: settings.validationUrl, propName: 'Name', model: model.toJSON() } });
        this.Picture
            .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
            .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });
        
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
    
    this.PrepareCollectionData(data.AllContacts, { messages: settings.contactMessages });
    this.PrepareCollectionData(data.AllAddresses, { messages: settings.addressMessages });

    EntityViewModelExtended.superClass_.constructor.call(this, data, settings);

    this.settings = settings;    
    
    this.errors = ko.validation.group(this);
};

EntityViewModelExtended.VENUE_CANCEL_EVENT = "OnVenueCancelled";
EntityViewModelExtended.VENUE_SAVE_EVENT = "OnVenueSaved";

EntityViewModelExtended.HOST_CANCEL_EVENT = "OnHostCancelled";
EntityViewModelExtended.HOST_SAVE_EVENT = "OnHostSaved";

inherits(EntityViewModelExtended, EntityViewModel);


//Contacts
EntityViewModelExtended.prototype.addContact = function () {
    this.AllContacts.push(new ContactViewModel({ Id: 0, EntityId: this.Id(), Type: 1, State: 1, messages: this.settings.contactMessages }));
    this.selectedItem(this.AllContacts()[this.AllContacts().length - 1]);
};

EntityViewModelExtended.prototype.deleteContact = function (item) {
    this.DeleteItem_(item);
};

EntityViewModelExtended.prototype.getContactView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.settings.contactEditView : bindingContext.$root.settings.contactView;
};
        
EntityViewModelExtended.prototype.GetContactType = function (item) {
    return this.settings.contactTypes[item.Type()];
};

EntityViewModelExtended.prototype.cancelContact = function (item, root) {
    root.selectedItem(null);
    
    if (item.Id() == 0) {
        root.AllContacts.remove(item);
    } else {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.settings.contactGetUrl,
            function (data) {
                item.loadData(data);
            }
        );
    }
};

EntityViewModelExtended.prototype.saveContact = function (item, root) {
    //if (item.isValidating()) {
    //    setTimeout(function () {
    //        root.saveContact(item, root);
    //    }, 50);
    //    return false;
    //}

    if (item.errors().length == 0) {
        root.selectedItem(null);

        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item);

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

EntityViewModelExtended.prototype.deleteContacts = function (root) {
    this.selectedItem(null);
    
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(root.CheckedContacts);

        ajaxJsonRequest(ajdata, root.settings.contactDeleteUrl,
            function (data) {
                while (root.CheckedContacts().length > 0){
                    root.AllContacts.remove(root.CheckedContacts()[0]);
                }
            }
        );
    }
};

//Addresses
EntityViewModelExtended.prototype.addAddress = function () {
    this.AllAddresses.push(new AddressViewModel({ Id: 0, EntityId: this.Id(), State: 1, Address: "", messages: this.settings.addressMessages }));
    this.selectedItem(this.AllAddresses()[this.AllAddresses().length - 1]);
};

EntityViewModelExtended.prototype.deleteAddress = function (item) {
    this.DeleteItem_(item);
};

EntityViewModelExtended.prototype.getAddressView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.settings.addressEditView : bindingContext.$root.settings.addressView;
};

EntityViewModelExtended.prototype.cancelAddress = function (item, root) {
    root.selectedItem(null);
    
    if (item.Id() == 0) {
        root.AllAddresses.remove(item);
    } else {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.settings.addressGetUrl,
            function(data) {
                item.loadData(data);
            }
        );
    }
};        

EntityViewModelExtended.prototype.saveAddress = function (item, root) {
    //if (item.isValidating()) {
    //    setTimeout(function () {
    //        root.saveContact(item, root);
    //    }, 50);
    //    return false;
    //}

    if (item.errors().length == 0) {
        root.selectedItem(null);

        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item);

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

EntityViewModelExtended.prototype.deleteAddresses = function (root) {    
    this.selectedItem(null);

    if (root.Id() != 0) {
        var ajdata = ko.toJSON(root.CheckedAddresses);

        ajaxJsonRequest(ajdata, root.settings.addressDeleteUrl,
            function (data) {
                while (root.CheckedAddresses().length > 0){
                    root.AllAddresses.remove(root.CheckedAddresses()[0]);
                }
            }
        );
    }
};        

// Shows    
EntityViewModel.prototype.addShow = function (metadata) {
    var newShow = new ShowViewModel({ Id: 0, EventMetadataId: metadata.Id(), VenueId: this.Id(), State: 1, StartDate: metadata.StartTime(), EndDate: metadata.StartTime(), messages: metadata.settings.showMessages, eventDtFrom: metadata.StartTime, eventDtTo: metadata.EndTime });
    //this.AllShows.splice(0, 0, newShow);
    this.AllShows.push(newShow);
    return newShow;
};

EntityViewModel.prototype.removeShow = function (item) {
    this.DeleteItem_(item);
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
    $(this.settings.entityFormName).trigger({
        type: this.settings.entityCancelEvent
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

        ajaxJsonRequest(ajdata, this.settings.entitySaveUrl,
            function (data) {
                if (self.Id() == 0)
                    self.loadData(data);
                $(self.settings.entityFormName).trigger({
                    type: self.settings.entitySaveEvent,
                    item: self
                });
            }
        );
    } else {
        root.errors.showAllMessages();
    }
};