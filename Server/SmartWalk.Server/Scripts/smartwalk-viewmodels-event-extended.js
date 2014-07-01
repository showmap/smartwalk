EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self.Description = ko.observable();
    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.CombineType = ko.observable();

    self.AllVenues = ko.observableArray();
    self.AllHosts = ko.observableArray();
    self.OtherVenues = ko.observableArray();

    self.Venues = ko.computed(function () {
        return self.Items_(self.AllVenues());
    }, self);

    self._selectedItem = ko.observable();
    self.selectedItem = ko.computed({
        read: function () {
            return self._selectedItem();
        },
        write: function (value) {
            self._selectedItem(value);

            self.AllVenues().forEach(
                function (venue) {
                    venue.IsEditing(self._selectedItem() == venue);

                    venue.AllShows().forEach(
                        function (show) {
                            show.IsEditing(self._selectedItem() == show);
                        });
                });
        }
    }, self);

    self.selectedVenue = ko.observable();

    // TODO: Why we need this method at all?
    self.loadDataExtended = function (eventData) {
        self.loadData(eventData);

        self.Description(eventData.Description);
        self.Latitude(eventData.Latitude);
        self.Longitude(eventData.Longitude);

        self.CombineType(eventData.CombineType);
        self.AllVenues($.map(eventData.AllVenues, function (venue) { return new EntityViewModel(venue); }));

        self.AllVenues().forEach(
            function (venue) {
                venue.AllShows().forEach(
                    function (show) {
                        show.extendValidation({
                            messages: settings.showMessages,
                            eventDtFrom: self.StartTime,
                            eventDtTo: self.EndTime
                        });
                    });
            });

        if (eventData.Host != null) {
            var item = new EntityViewModel(eventData.Host);
            self.Host(item);
            self.AllHosts.push(item);
        } else {
            self.Host(null);
        }
    };

    self.loadDataExtended(data);

    // TODO: Should not be computed, 'cause it's slows down shit
    // There is only one place where it's needed as computed, 
    // for ValidateModel async request, maybe use something else there?
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            CombineType: self.CombineType(),
            Title: self.Title(),
            StartTime: self.StartTime(),
            EndTime: self.EndTime(),
            IsPublic: self.IsPublic(),
            Picture: self.Picture(),

            Description: self.Description(),
            Latitude: self.Latitude(),
            Longitude: self.Longitude(),

            Host: self.Host() ? self.Host().toJSON() : "",
            AllVenues: ko.utils.arrayMap(self.Venues(), function (venue) { return venue.toJSON(); }),
        };
    }, self);

    // Validation Extensions

    self.StartTime
        .extend({ required: { message: settings.startTimeRequiredValidationMessage } })
        .extend({
            dateCompareValidation: {
                    params: {
                        allowEmpty: true,
                        cmp: "LESS_THAN",
                        compareVal: self.EndTime
                    },
                    message: settings.startTimeCompareValidationMessage
            }
        });
                
    self.EndTime.extend({
        dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "GREATER_THAN",
                    compareVal: self.StartTime
                },
                message: settings.endTimeCompareValidationMessage
        },
    });
        
    self.Host.extend({
        required: { message: settings.hostRequiredValidationMessage },
    });

    self.Picture
        .extend({
            maxLength: {
                params: 255,
                message: settings.pictureLengthValidationMessage
            }
        })
        .extend({
            urlValidation: {
                params: { allowEmpty: true },
                message: settings.picturePatternValidationMessage
            }
        });

    self.selectedVenue.extend({
        required: {
            message: settings.venueRequiredValidationMessage,
            onlyIf: function () { return self.selectedItem() != null; }
        },            
    });

    self.isValidating = ko.computed(function () {
        return self.StartTime.isValidating() || self.Host.isValidating() || self.Picture.isValidating();
    }, self);

    self.errors = ko.validation.group({
        StartTime: self.StartTime,
        EndTime: self.EndTime,
        Host: self.Host,
        Picture: self.Picture,
    });
    
    self.venueErrors = ko.validation.group({
        selectedVenue: self.selectedVenue
    });
    
    self.attachEvents();
    self.setupDialogs();
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

    $(self.settings.hostFormName).bind(EntityViewModelExtended.ENTITY_CANCEL_EVENT, function (event) {
        $(self.settings.hostFormName).dialog("close");
    });

    $(self.settings.hostFormName).bind(EntityViewModelExtended.ENTITY_SAVE_EVENT, function (event) {
        var newHost = new EntityViewModel(event.item.toJSON());
        self.AllHosts.push(newHost);
        self.Host(newHost);
        event.item.loadData(self.getNewHost().toJSON());
        $(self.settings.hostFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_CANCEL_EVENT, function (event) {
        $(self.settings.venueFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_SAVE_EVENT, function (event) {
        var newVenue = new EntityViewModel(event.item.toJSON());
        self.OtherVenues.push(newVenue);
        self.selectedVenue(newVenue);
        event.item.loadData(self.getNewVenue().toJSON());
        $(self.settings.venueFormName).dialog("close");
    });
};

EventViewModelExtended.prototype.saveEvent = function (root) {
    var self = this;

    if (root.errors().length == 0) {
        var ajdata = ko.toJSON(self.toJSON());

        ajaxJsonRequest(
            ajdata,
            self.settings.eventSaveUrl,
            function (data) {
                window.location.href = "/event/" + data.Id; // TODO: replace by settings property
            }
        );
    } else {
        root.errors.showAllMessages();
    }    
};

EventViewModelExtended.prototype.clearItem = function (root, item, condition, deleteItem) {
    if (item() != null) {
        if (condition() && deleteItem) {
            root.DeleteItem_(item());
        }

        item(null);
    }
};

EventViewModelExtended.prototype.clearSelectedItem = function (root, deleteItem) {
    root.clearItem(root, root.selectedItem, function () { return root.selectedItem().Id() == 0; }, deleteItem);
};

EventViewModelExtended.prototype.clearSelectedVenue = function (root, deleteItem) {
    root.clearItem(root, root.selectedVenue, function () { return true; }, deleteItem);
};

EventViewModelExtended.prototype.clearInner = function (root) {
    root.clearSelectedItem(root, true);
    root.clearSelectedVenue(root, true);
};

// Show
EventViewModelExtended.prototype.addShow = function (root, item) {
    root.clearInner(root);
    root.selectedItem(item.addShow(root));
};

EventViewModelExtended.prototype.cancelShow = function (root, item) {
    root.clearInner(root);
    
    if (item.Id() != 0) {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(ajdata, root.settings.showGetUrl,
            function (data) {
                if (data) item.loadData(data);
            }
        );
    }
};

EventViewModelExtended.prototype.saveShow = function (root, item) {
    if (item.errors().length == 0) {
        if (root.Id() != 0) {
            var ajdata = ko.toJSON(item.toJSON());

            ajaxJsonRequest(
                ajdata,
                root.settings.showSaveUrl,
                function (data) {
                    if (item.Id() == 0 || item.Id() != data) item.Id(data);

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
    root.clearInner(root);
    
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(
            ajdata,
            root.settings.showDeleteUrl,
            function () { root.DeleteItem_(item); }
        );
    } else {
        root.DeleteItem_(item);
    }
};

EventViewModelExtended.prototype.getShowView = function (item, bindingContext) {
    return item.IsEditing()
        ? bindingContext.$root.settings.showEditView
        : bindingContext.$root.settings.showView;
};

EventViewModelExtended.prototype.saveVenue = function (root, item) {
    if (root.venueErrors().length == 0) {
        var venue = root.selectedVenue();

        if (root.Id() != 0) {
            venue.EventMetadataId(root.Id());
            var ajdata = ko.toJSON(venue.toJSON());

            ajaxJsonRequest(
                ajdata,
                root.settings.eventVenueSaveUrl,
                function (data) {
                    if (data) venue.AllShows.push(new ShowViewModel(data));

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

EventViewModelExtended.prototype.deleteVenue = function (root, item) {
    root.clearInner(root);

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
    var self = this;

    var ajdata = JSON.stringify({
        term: searchTerm,
        eventId: self.Id(),
        currentEvent: self.Id() == 0 ? self.toJSON() : null
    });
    
    ajaxJsonRequest(
        ajdata,
        self.settings.venueAutocompleteUrl,
        function (data) {
            if (data && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    sourceArray($.map(data, function (venue) { return new EntityViewModel(venue); }));
                }
            }
        }
    );
};

EventViewModel.prototype.getNewHost = function () {
    return new EntityViewModel({ Id: 0, Type: 0, State: 1 });
};

EventViewModel.prototype.getNewVenue = function () {
    return new EntityViewModel({ Id: 0, Type: 1, State: 1 });
};

EventViewModel.prototype.addVenue = function (root) {
    root.clearInner(root);

    var newVenue = root.getNewVenue();
    root.AllVenues.push(newVenue);
    root.selectedItem(newVenue);
};

// TODO: What "this" is doing in here?
EventViewModel.prototype.removeVenue = function (item) {
    this.DeleteItem_(item);
};

EventViewModel.prototype.cancelVenue = function (root) {
    root.clearInner(root);    
};

EventViewModelExtended.prototype.getVenueView = function (item, bindingContext) {
    return item.IsEditing()
        ? bindingContext.$root.settings.eventVenueEditView
        : bindingContext.$root.settings.eventVenueView;
};

EventViewModelExtended.prototype.getHosts = function(searchTerm, sourceArray) {
    var ajdata = JSON.stringify({ term: searchTerm });

    ajaxJsonRequest(ajdata, this.settings.hostAutocompleteUrl,
        function (data) {
            if (data && data.length > 0)
                sourceArray($.map(data, function (host) { return new EntityViewModel(host); }));
        }
    );
};

EventViewModelExtended.prototype.getAutoItem = function(item) {
    var text = "<div>";
    text += "<div class='autocomplete-name'>" + item.Name() + "</div>";
    if (item.DisplayAddress()) {
        if (item.DisplayAddress().Address() != "")
            text += "<div class='autocomplete-detail'>" + item.DisplayAddress().Address() + "</div>";
    }

    text += "</div>";
    return text;
};

EventViewModelExtended.prototype.deleteAction = function (root) {
    root.deleteVenues(root);
};

// TODO: What "this" is doing in here?
EventViewModelExtended.prototype.createHost = function() {
    $(this.settings.hostFormName).dialog("open");
};