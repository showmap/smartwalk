//EventViewModelExtended class
EventViewModelExtended = function (settings, data) {
    var self = this;

    this.setupValidations = function () {
        this.StartTime
            .extend({ required: { message: settings.startTimeRequiredValidationMessage } })
            .extend({
                dateCompareValidation: { params: { allowEmpty: true, cmp: 'LESS_THAN', compareVal: this.EndTime }, message: settings.startTimeCompareValidationMessage }
            });
                
        this.EndTime.extend({
            dateCompareValidation: { params: { allowEmpty: true, cmp: 'GREATER_THAN', compareVal: this.StartTime }, message: settings.endTimeCompareValidationMessage },
        });
        
        this.Host.extend({
            required: { message: settings.hostRequiredValidationMessage },
        });
        this.Picture
            .extend({ maxLength: { params: 255, message: settings.pictureLengthValidationMessage } })
            .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.picturePatternValidationMessage } });
        this.selectedVenue.extend({
            required: {
                message: settings.venueRequiredValidationMessage,
                onlyIf: function () {
                    return self.selectedItem() != null;
                }
            },            
        });

        this.isValidating = ko.computed(function () {
            return this.StartTime.isValidating() || this.Host.isValidating() || this.Picture.isValidating();
        }, this);
    };

    this.PrepareCollectionData = function (collection, extData) {
        if (collection && collection.length > 0) {
            for (var i = 0; i < collection.length; i++) {
                collection[i] = $.extend(collection[i], extData);
            }
        }
    };

    this.prepareData = function() {
        for (var i = 0; i < data.AllVenues.length; i++) {
            this.PrepareCollectionData(data.AllVenues[i].AllShows, { messages: settings.showMessages, eventDtFrom: this.StartTime, eventDtTo: this.EndTime });
        }
    };

    

    EventViewModelExtended.superClass_.constructor.call(this, data);

    this.settings = settings;

    this.errors = ko.validation.group({
        StartTime: this.StartTime,
        EndTime: this.EndTime,
        Host: this.Host,
        Picture: this.Picture,
    });
    
    this.venueErrors = ko.validation.group({
        selectedVenue: this.selectedVenue
    });
    
    this.attachEvents();
    this.setupDialogs();    
};

inherits(EventViewModelExtended, EventViewModel);

EventViewModelExtended.prototype.setupDialogs = function () {
    $(this.settings.hostFormName).dialog({
        modal: true,
        autoOpen: false,
        dialogClass: 'noTitleStuff',
        width: 800,
        height: 600,
        show: {
            effect: "blind",
            duration: 1000
        },
        hide: {
            effect: "explode",
            duration: 1000
        },
    });

    $(this.settings.venueFormName).dialog({
        modal: true,
        autoOpen: false,
        dialogClass: 'noTitleStuff',
        width: 800,
        height: 600,
        show: {
            effect: "blind",
            duration: 1000
        },
        hide: {
            effect: "explode",
            duration: 1000
        },
    });

    $(".ui-dialog-titlebar").hide();
};

EventViewModelExtended.prototype.attachEvents = function () {
    var self = this;

    $(self.settings.hostFormName).bind(EntityViewModelExtended.HOST_CANCEL_EVENT, function (event) {
        $(self.settings.hostFormName).dialog("close");
    });

    $(self.settings.hostFormName).bind(EntityViewModelExtended.HOST_SAVE_EVENT, function (event) {
        self.AllHosts.push(event.item);
        self.Host(event.item);
        $(self.settings.hostFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.VENUE_CANCEL_EVENT, function (event) {
        $(self.settings.venueFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.VENUE_SAVE_EVENT, function (event) {
        self.OtherVenues.push(event.item);
        self.selectedVenue(event.item);

        $(self.settings.venueFormName).dialog("close");
    });
};

EventViewModelExtended.prototype.saveEvent = function (root) {
    if (root.errors().length == 0) {
        var ajdata = ko.toJSON(this.toJSON());
        ajaxJsonRequest(ajdata, this.settings.eventSaveUrl,
            function (data) {
                window.location.href = "/event/" + data.Id;
            }
        );
    } else {
        root.errors.showAllMessages();
    }    
};

EventViewModelExtended.prototype.clearItem = function (root, item, condition, deleteItem) {
    if (item() != null) {
        if(condition() && deleteItem)
            root.DeleteItem_(item());
        item(null);
    }
};

EventViewModelExtended.prototype.clearSelectedItem = function (root, deleteItem) {
    root.clearItem(root, root.selectedItem, function () { return root.selectedItem().Id() == 0; }, deleteItem);
};

EventViewModelExtended.prototype.clearSelectedVenue = function (root, deleteItem) {
    root.clearItem(root, root.selectedVenue, function () { return true; }, deleteItem);
};

EventViewModelExtended.prototype.cancelInner = function (root) {
    root.clearSelectedItem(root, true);
    root.clearSelectedVenue(root, true);
};

// Show
EventViewModelExtended.prototype.addShow = function (root, item) {
    root.cancelInner(root);
    root.selectedItem(item.addShow(root));
};

EventViewModelExtended.prototype.cancelShow = function (root, item) {
    root.cancelInner(root);
    
    if (item.Id() != 0) {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.settings.showGetUrl,
            function (data) {
                if (data)
                    item.loadData(data);
            }
        );
    }
};

EventViewModelExtended.prototype.saveShow = function (root, item) {
    if (item.errors().length == 0) {
        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item);

            ajaxJsonRequest(ajdata, root.settings.showSaveUrl,
                function (data) {
                    if (item.Id() == 0 || item.Id() != data)
                        item.Id(data);
                    root.selectedItem(null);
                }
            );
        } else {
            root.clearSelectedItem(root, false);
        }
    } else {
        item.errors.showAllMessages();
    }    
};

EventViewModelExtended.prototype.deleteShow = function (root, item) {
    root.cancelInner(root);
    
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.settings.showDeleteUrl,
            function(data) {
                root.DeleteItem_(item);
            }
        );
    } else {
        root.DeleteItem_(item);
    }
};

EventViewModelExtended.prototype.deleteShows = function (root) {
    root.cancelInner(root);

    if (root.CheckedShows().length > 0) {
        if (root.Id() != 0) {
            var ajdata = ko.toJSON(root.CheckedShows);
            ajaxJsonRequest(ajdata, root.settings.eventShowsDeleteUrl,
                function(data) {
                    ko.utils.arrayForEach(root.CheckedShows(), function(item) {
                        root.DeleteItem_(item);
                    });
                }
            );
        } else {
            ko.utils.arrayForEach(root.CheckedShows(), function (item) {
                root.DeleteItem_(item);
            });
        }
    }
};

EventViewModelExtended.prototype.getShowView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.settings.showEditView : bindingContext.$root.settings.showView;
};

EventViewModelExtended.prototype.saveVenue = function (root, item) {
    if (root.venueErrors().length == 0) {
        var venue = root.selectedVenue();

        if (root.Id() != 0) {
            //alert('item id = ' + vm.selectedVenue().Id() + ' selected item id = ' + vm.selectedItem().Id());
            venue.EventMetadataId(root.Id());
            var ajdata = ko.toJSON(venue);

            ajaxJsonRequest(ajdata, root.settings.eventVenueSaveUrl,
                function(data) {
                    if (data)
                        venue.AllShows.push(new ShowViewModel(data));

                    root.AllVenues.push(venue);
                    root.clearSelectedItem(root, true);
                    root.clearSelectedVenue(root, false);
                }
            );
        } else {
            root.AllVenues.push(venue);
            root.clearSelectedItem(root, true);
            root.clearSelectedVenue(root, false);
        }
    } else {
        root.venueErrors.showAllMessages();
    }
};


EventViewModelExtended.prototype.deleteVenuesClientSide = function(root) {    
    ko.utils.arrayForEach(root.CheckedVenues(), function (venue) {
        ko.utils.arrayForEach(venue.AllShows(), function (show) {
            root.DeleteItem_(show);
            show.IsChecked(false);
        });
        root.DeleteItem_(venue);
    });
    root.deleteShows(root);
};

EventViewModelExtended.prototype.deleteVenues = function(root) {
    root.cancelInner(root);

    if (root.Id() != 0 && root.CheckedVenues().length > 0) {
        var ajdata = ko.toJSON(root.CheckedVenues);

        ajaxJsonRequest(ajdata, root.settings.eventVenuesDeleteUrl,
            function(data) {
                root.deleteVenuesClientSide(root);
            }
        );
    } else {
        root.deleteVenuesClientSide(root);
    }
};

EventViewModelExtended.prototype.deleteVenue = function (root, item) {
    root.cancelInner(root);

    if (root.Id() != 0) {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(ajdata, root.settings.eventVenueDeleteUrl,
            function(data) {
                root.DeleteItem_(item);
            }
        );
    } else {
        root.DeleteItem_(item);
    }
};

EventViewModelExtended.prototype.createVenue = function (root) {
    $(root.settings.venueFormName).dialog("open");
};

EventViewModelExtended.prototype.getVenues = function (searchTerm, sourceArray) {
    var ajdata = JSON.stringify({ term: searchTerm, eventId: this.Id(), currentEvent: this.Id() == 0 ? this.toJSON() : null });
    
    ajaxJsonRequest(ajdata, this.settings.venueAutocompleteUrl,
        function (data) {
            if (data && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    sourceArray($.map(data, function (item) { return new EntityViewModel(item); }));
                }
            }
        }
    );
};

EventViewModel.prototype.addVenue = function (root) {
    root.cancelInner(root);
    
    var newVenue = new EntityViewModel({ Id: 0, Type: 1, State: 1 });
    root.AllVenues.push(newVenue);
    root.selectedItem(newVenue);
    //root.selectedItem(root.AllVenues()[root.AllVenues().length-1]);
    //root.selectedVenue(newVenue);
};

EventViewModel.prototype.removeVenue = function (item) {
    this.DeleteItem_(item);
};

EventViewModel.prototype.cancelVenue = function (root) {
    root.cancelInner(root);    
};

EventViewModelExtended.prototype.getVenueView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.settings.eventVenueEditView : bindingContext.$root.settings.eventVenueView;
};

EventViewModelExtended.prototype.getHosts = function(searchTerm, sourceArray) {
    var ajdata = JSON.stringify({ term: searchTerm });
    ajaxJsonRequest(ajdata, this.settings.hostAutocompleteUrl,
        function (data) {
            if (data && data.length > 0)
                sourceArray($.map(data, function(item) { return new EntityViewModel(item); }));
        }
    );
};

EventViewModelExtended.prototype.getAutoItem = function(item) {
    var text = "<div>";
    //text += "<div class='categoryIconContainer'><img src='" + (item.Picture() == "" ? "" : item.Picture()) + "' /></div>";
    text += "<div class='autocomplete-name'>" + item.Name() + "</div>";
    if (item.DisplayAddress()) {
        if (item.DisplayAddress().Address() != "")
            text += "<div class='autocomplete-detail'>" + item.DisplayAddress().Address() + "</div>";
        //if (item.DisplayAddress().Tip() != "")
        //    text += "<div class='autocomplete-detail'>" + item.DisplayAddress().Tip() + "</div>";
    }

    text += "</div>";
    return text;
};

EventViewModelExtended.prototype.deleteAction = function (root) {
    root.deleteVenues(root);
};


EventViewModelExtended.prototype.createHost = function() {
    $(this.settings.hostFormName).dialog("open");
};



//EventViewModelExtended.prototype.deleteEvent = function(item) {
//    var ajdata = ko.toJSON(item);
//    var urlUpdate = $("#event-item").attr("delete-event-url");

//    ajaxJsonRequest(ajdata, urlUpdate,
//        function (data) {
//            window.location.href = "@Url.Action("List")";
//        }
//    );   
//};  