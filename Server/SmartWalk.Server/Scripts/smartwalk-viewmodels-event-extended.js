EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self.venues = ko.computed(function() {
        return self.allVenues()
            ? VmItemUtil.AvailableItems(self.allVenues()) : undefined;
    });

    self.venues().forEach(function (venue) {
        EventViewModelExtended.initVenueViewModel(venue, self);
    });

    self.venues.subscribe(function (venues) {
        if (venues) {
            venues.forEach(function(venue) {
                if (venue.IsEditing === undefined) {
                    EventViewModelExtended.initVenueViewModel(venue, self);
                }
            });
        }
    });

    self.allHosts = ko.observableArray();
    self.otherVenues = ko.observableArray();

    if (self.host()) {
        self.allHosts.push(self.host());
    }

    self.host.subscribe(function (host) { if (host) self.allHosts.push(host); });

    self.initSelectedItem(self);
    self.setupValidation(self, settings);
    self.attachEvents();
    self.setupDialogs();
};

inherits(EventViewModelExtended, EventViewModel);

EventViewModelExtended.prototype.initSelectedItem = function (model) {
    model._selectedItem = ko.observable();
    model.selectedItem = ko.computed({
        read: function () {
            return model._selectedItem();
        },
        write: function (value) {
            model._selectedItem(value);

            if (model.venues()) {
                model.venues().forEach(function (venue) {
                    venue.IsEditing(value == venue);

                    if (venue.Shows()) {
                        venue.Shows().forEach(function (show) {
                            show.IsEditing(value == show);
                        });
                    }
                });
            }
        }
    }, model);

    model.selectedVenue = ko.observable();
}

EventViewModelExtended.prototype.setupValidation = function (event, settings) {
    event.startTime
        .extend({ required: { message: settings.startTimeRequiredValidationMessage } })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "LESS_THAN",
                    compareVal: event.endTime
                },
                message: settings.startTimeCompareValidationMessage
            }
        });

    event.endTime.extend({
        dateCompareValidation: {
            params: {
                allowEmpty: true,
                cmp: "GREATER_THAN",
                compareVal: event.startTime
            },
            message: settings.endTimeCompareValidationMessage
        },
    });

    event.host.extend({
        required: { message: settings.hostRequiredValidationMessage },
    });

    event.picture
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

    event.selectedVenue.extend({
        required: {
            message: settings.venueRequiredValidationMessage,
            onlyIf: function () { return event.selectedItem() != null; }
        },
    });

    event.isValidating = ko.computed(function () {
        return event.startTime.isValidating() ||
            event.host.isValidating() ||
            event.picture.isValidating();
    }, event);

    event.errors = ko.validation.group({
        startTime: event.startTime,
        endTime: event.endTime,
        host: event.host,
        picture: event.picture,
    });

    event.venueErrors = ko.validation.group({
        selectedVenue: event.selectedVenue
    });
};

EventViewModelExtended.prototype.setupShowValidation = function (show, event, settings) {
    show.title
        .extend({ required: { params: true, message: settings.showMessages.titleRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.showMessages.titleLengthValidationMessage } });

    show.picture
        .extend({ maxLength: { params: 255, message: settings.showMessages.pictureLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.showMessages.pictureValidationMessage } });

    show.detailsUrl
        .extend({ maxLength: { params: 255, message: settings.showMessages.detailsLengthValidationMessage } })
        .extend({ urlValidation: { params: { allowEmpty: true }, message: settings.showMessages.detailsValidationMessage } });

    // TODO: To setup validation for show Time too
    show.startDate
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'LESS_THAN',
                    compareVal: show.endDate
                },
                message: settings.showMessages.startDateValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'REGION',
                    compareVal: event.startTime,
                    compareValTo: event.endTime
                },
                message: settings.showMessages.startTimeValidationMessage
            }
        });

    show.endDate
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'GREATER_THAN',
                    compareVal: show.endDate
                },
                message: settings.showMessages.endDateValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: 'REGION',
                    compareVal: event.startTime,
                    compareValTo: event.endTime
                },
                message: settings.showMessages.endTimeValidationMessage
            }
        });

    show.errors = ko.validation.group(show);
}

EventViewModelExtended.initVenueViewModel = function (venue, event) {
    venue.IsEditing = ko.observable(false);

    venue.Shows = ko.computed(function () {
        return venue.allShows()
            ? VmItemUtil.AvailableItems(venue.allShows()) : undefined;
    });

    if (venue.Shows()) {
        venue.Shows().forEach(function (show) {
            EventViewModelExtended.initShowViewModel(show, event);
        });
    }

    venue.Shows.subscribe(function (shows) {
        if (shows) {
            shows.forEach(function(show) {
                if (show.IsEditing === undefined) {
                    EventViewModelExtended.initShowViewModel(show, event);
                }
            });
        }
    });
}

EventViewModelExtended.initShowViewModel = function (show, event) {
    show.IsEditing = ko.observable(false);
    event.setupShowValidation(show, event, event.settings);
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
        var newHost = new EntityViewModel(event.item.toJSON());
        self.host(newHost);
        event.item.loadData(self.getNewHost().toJSON());
        $(self.settings.hostFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_CANCEL_EVENT, function (event) {
        $(self.settings.venueFormName).dialog("close");
    });

    $(self.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_SAVE_EVENT, function (event) {
        var newVenue = new EntityViewModel(event.item.toJSON());
        self.otherVenues.push(newVenue);
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
    root.clearItem(root, root.selectedItem, function () { return root.selectedItem().id() == 0; }, deleteItem);
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

    var newShowModel = new ShowViewModel({
        Id: 0,
        EventMetadataId: root.id(),
        VenueId: venue.id(),
        State: VmItemState.Added,
        StartDate: root.startTime(),
        EndDate: root.startTime()
    },
    root);

    venue.allShows.push(newShowModel);
    root.selectedItem(newShowModel);
};

EventViewModelExtended.prototype.cancelShow = function (root, item) {
    root.clearInner(root);
    
    if (item.id() != 0) {
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
        if (root.id() != 0) {
            var ajdata = ko.toJSON(item.toJSON());

            ajaxJsonRequest(
                ajdata,
                root.settings.showSaveUrl,
                function (data) {
                    if (item.id() == 0 || item.id() != data) item.id(data);

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
    
    if (root.id() != 0) {
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

        if (root.id() != 0) {
            venue.eventMetadataId(root.id());
            var ajdata = ko.toJSON(venue.toJSON());

            ajaxJsonRequest(
                ajdata,
                root.settings.eventVenueSaveUrl,
                function (data) {
                    if (data) venue.allShows.push(new ShowViewModel(data));

                    root.allVenues.push(venue);
                    root.clearSelectedItem(root, true);
                    root.clearSelectedVenue(root, false);
                }
            );
        } else {
            root.allVenues.push(venue);
            root.clearSelectedItem(root, true);
            root.clearSelectedVenue(root, false);
        }
    } else {
        root.venueErrors.showAllMessages();
    }
};

EventViewModelExtended.prototype.deleteVenue = function (root, item) {
    root.clearInner(root);

    if (root.id() != 0) {
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
        eventId: self.id(),
        currentEvent: self.id() == 0 ? self.toJSON() : null
    });
    
    ajaxJsonRequest(
        ajdata,
        self.settings.venueAutocompleteUrl,
        function (data) {
            if (data && data.length > 0) {
                sourceArray($.map(data,
                    function (venue) { return new EntityViewModel(venue); }));
            }
        }
    );
};

EventViewModelExtended.prototype.getNewHost = function () {
    return new EntityViewModel({ Id: 0, Type: EntityType.Host, State: VmItemState.Added });
};

EventViewModelExtended.prototype.getNewVenue = function () {
    return new EntityViewModel({ Id: 0, Type: EntityType.Venue, State: VmItemState.Added });
};

EventViewModelExtended.prototype.addVenue = function (root) {
    root.clearInner(root);

    var newVenue = root.getNewVenue();
    root.allVenues.push(newVenue);
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
                    function (host) { return new EntityViewModel(host); }));
            }
        }
    );
};

EventViewModelExtended.prototype.getAutoItem = function(item) {
    var text = "<div>";
    text += "<div class='autocomplete-name'>" + item.name() + "</div>";
    if (item.displayAddress()) {
        if (item.displayAddress().address() != "")
            text += "<div class='autocomplete-detail'>" + item.displayAddress().address() + "</div>";
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