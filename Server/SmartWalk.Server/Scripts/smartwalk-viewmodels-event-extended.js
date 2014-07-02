EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self.AllVenues().forEach(function(venue) {
        EventViewModelExtended.initEntityViewModel(venue);
        venue.AllShows().forEach(
            function (showModel) {
                EventViewModelExtended.initShowViewModel(showModel, self);
            });
    });

    self.AllHosts = ko.observableArray();
    self.OtherVenues = ko.observableArray();

    if (self.Host()) {
        self.AllHosts.push(self.Host());
    }

    self.Venues = ko.computed(
        function () { return VmItemUtil.AvailableItems(self.AllVenues()); },
        self);

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

                    if (venue.AllShows()) {
                        venue.AllShows().forEach(
                            function(show) {
                                show.IsEditing(self._selectedItem() == show);
                            });
                    }
                });
        }
    }, self);

    self.selectedVenue = ko.observable();

    self.setupValidation(self, settings);
    self.attachEvents();
    self.setupDialogs();
};

inherits(EventViewModelExtended, EventViewModel);

EventViewModelExtended.prototype.setupValidation = function (model, settings) {
    model.StartTime
        .extend({ required: { message: settings.startTimeRequiredValidationMessage } })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "LESS_THAN",
                    compareVal: model.EndTime
                },
                message: settings.startTimeCompareValidationMessage
            }
        });

    model.EndTime.extend({
        dateCompareValidation: {
            params: {
                allowEmpty: true,
                cmp: "GREATER_THAN",
                compareVal: model.StartTime
            },
            message: settings.endTimeCompareValidationMessage
        },
    });

    model.Host.extend({
        required: { message: settings.hostRequiredValidationMessage },
    });

    model.Picture
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

    model.selectedVenue.extend({
        required: {
            message: settings.venueRequiredValidationMessage,
            onlyIf: function () { return model.selectedItem() != null; }
        },
    });

    model.isValidating = ko.computed(function () {
        return model.StartTime.isValidating() ||
            model.Host.isValidating() ||
            model.Picture.isValidating();
    }, model);

    model.errors = ko.validation.group({
        StartTime: model.StartTime,
        EndTime: model.EndTime,
        Host: model.Host,
        Picture: model.Picture,
    });

    model.venueErrors = ko.validation.group({
        selectedVenue: model.selectedVenue
    });
};

EventViewModelExtended.prototype.setupShowValidation = function(eventModel, showModel, settings) {
    showModel.Title
        .extend({ required: { params: true, message: settings.showMessages.titleRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.showMessages.titleLengthValidationMessage } });

    showModel.Picture
        .extend({ maxLength: { params: 255, message: settings.showMessages.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.showMessages.pictureValidationMessage } });

    showModel.DetailsUrl
        .extend({ maxLength: { params: 255, message: settings.showMessages.detailsLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.showMessages.detailsValidationMessage } });

    // TODO: To setup validation for show Time too
    showModel.StartDate
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'LESS_THAN',
                    compareVal: showModel.EndDate
                },
                message: settings.showMessages.startDateValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'REGION',
                    compareVal: eventModel.StartTime,
                    compareValTo: eventModel.EndTime
                },
                message: settings.showMessages.startTimeValidationMessage
            }
        });

    showModel.EndDate
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'GREATER_THAN',
                    compareVal: showModel.EndDate
                },
                message: settings.showMessages.endDateValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'REGION',
                    compareVal: eventModel.StartTime,
                    compareValTo: eventModel.EndTime
                },
                message: settings.showMessages.endTimeValidationMessage
            }
        });

    showModel.errors = ko.validation.group(showModel);
}

EventViewModelExtended.createEntityViewModel = function (entityData) {
    var model = new EntityViewModel(entityData);
    EventViewModelExtended.initEntityViewModel(model);
    return model;
}

EventViewModelExtended.initEntityViewModel = function (model) {
    model.Shows = ko.computed(
        function () { return VmItemUtil.AvailableItems(model.AllShows()); }, self);
    model.IsEditing = ko.observable(false);
}

EventViewModelExtended.createShowViewModel = function (showData, eventModel) {
    var model = new ShowViewModel(showData);
    EventViewModelExtended.initShowViewModel(model, eventModel);
    return model;
}

EventViewModelExtended.initShowViewModel = function (showModel, eventModel) {
    showModel.IsEditing = ko.observable(false);
    eventModel.setupShowValidation(eventModel, showModel, eventModel.settings);
}

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
        var newHost = EventViewModelExtended.createEntityViewModel(event.item.toJSON());
        self.AllHosts.push(newHost);
        self.Host(newHost);
        event.item.loadData(self.getNewHost().toJSON());
        $(self.settings.hostFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_CANCEL_EVENT, function (event) {
        $(self.settings.venueFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_SAVE_EVENT, function (event) {
        var newVenue = EventViewModelExtended.createEntityViewModel(event.item.toJSON());
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
            VmItemUtil.DeleteItem(item());
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
EventViewModelExtended.prototype.addShow = function (root, venue) {
    root.clearInner(root);

    var newShowModel = EventViewModelExtended.createShowViewModel({
        Id: 0,
        EventMetadataId: root.Id(),
        VenueId: venue.Id(),
        State: VmItemState.Added,
        StartDate: root.StartTime(),
        EndDate: root.StartTime()
    },
    root);

    venue.AllShows.push(newShowModel);
    root.selectedItem(newShowModel);
};

EventViewModelExtended.prototype.cancelShow = function (root, item) {
    root.clearInner(root);
    
    if (item.Id() != 0) {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(
            ajdata,
            root.settings.showGetUrl,
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
            function () { VmItemUtil.DeleteItem(item); }
        );
    } else {
        VmItemUtil.DeleteItem(item);
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
                    if (data) venue.AllShows.push(EventViewModelExtended.createShowViewModel(data), root);

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
                VmItemUtil.DeleteItem(item);
            }
        );
    } else {
        VmItemUtil.DeleteItem(item);
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
                    sourceArray($.map(
                        data,
                        function (venue) { return EventViewModelExtended.createEntityViewModel(venue); }));
                }
            }
        }
    );
};

EventViewModelExtended.prototype.getNewHost = function () {
    return EventViewModelExtended.createEntityViewModel({ Id: 0, Type: 0, State: VmItemState.Added });
};

EventViewModelExtended.prototype.getNewVenue = function () {
    return EventViewModelExtended.createEntityViewModel({ Id: 0, Type: 1, State: VmItemState.Added });
};

EventViewModelExtended.prototype.addVenue = function (root) {
    root.clearInner(root);

    var newVenue = root.getNewVenue();
    root.AllVenues.push(newVenue);
    root.selectedItem(newVenue);
};

EventViewModelExtended.prototype.removeVenue = function (item) {
    VmItemUtil.DeleteItem(item);
};

EventViewModelExtended.prototype.cancelVenue = function (root) {
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
            if (data && data.length > 0) {
                sourceArray($.map(
                    data,
                    function (host) { return EventViewModelExtended.createEntityViewModel(host); }));
            }
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