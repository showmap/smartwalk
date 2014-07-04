EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;

    self.venues = ko.computed(function() {
        return self.allVenues()
            ? VmItemUtil.availableItems(self.allVenues()) : undefined;
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

    self._selectedItem = ko.observable();
    self.selectedItem = ko.computed({
        read: function () {
            return self._selectedItem();
        },
        write: function (value) {
            self._selectedItem(value);

            if (self.venues()) {
                self.venues().forEach(function (venue) {
                    venue.IsEditing(value == venue);

                    if (venue.Shows()) {
                        venue.Shows().forEach(function (show) {
                            show.IsEditing(value == show);
                        });
                    }
                });
            }
        }
    }, self);

    self.selectedVenue = ko.observable();

    EventViewModelExtended.setupValidation(self, settings);
    EventViewModelExtended.attachEvents(self);
    EventViewModelExtended.setupDialogs(self);

    self.saveEvent = function () {
        if (self.errors().length == 0) {
            ajaxJsonRequest(self.toJSON(), self.settings.eventSaveUrl,
                function (eventData) {
                    window.location.href = "/event/" + eventData.Id; // TODO: replace by settings property
                }
            );
        } else {
            self.errors.showAllMessages();
        }
    };

    self.clearSelectedItem = function (deleteItem) {
        EventViewModelExtended.clearItem(self.selectedItem,
            function () { return self.selectedItem().id() == 0; }, deleteItem);
    };

    self.clearSelectedVenue = function (deleteItem) {
        EventViewModelExtended.clearItem(self.selectedVenue,
            function () { return true; }, deleteItem);
    };

    self.clearInner = function () {
        self.clearSelectedItem(true);
        self.clearSelectedVenue(true);
    };

    // Shows
    self.addShow = function (venue) {
        self.clearInner();

        var show = new ShowViewModel({
            Id: 0,
            EventMetadataId: self.id(),
            VenueId: venue.id(),
            State: VmItemState.Added,
            StartDate: self.startTime(),
            EndDate: self.startTime()
        });

        venue.allShows.push(show);
        self.selectedItem(show);
    };

    self.cancelShow = function (show) {
        self.clearInner();

        if (show.id() != 0) {
            ajaxJsonRequest(show.toJSON(), self.settings.showGetUrl,
                function (showData) {
                    if (showData) show.loadData(showData);
                }
            );
        }
    };

    self.saveShow = function (show) {
        if (show.errors().length == 0) {
            if (self.id() != 0) {
                ajaxJsonRequest(show.toJSON(), self.settings.showSaveUrl,
                    function (showId) {
                        if (show.id() == 0 || show.id() != showId) show.id(showId);

                        self.selectedItem(null);
                    }
                );
            } else {
                self.clearSelectedItem(false);
            }
        } else {
            show.errors.showAllMessages();
        }
    };

    self.deleteShow = function (show) {
        self.clearInner();

        if (self.id() != 0) {
            ajaxJsonRequest(show.toJSON(), self.settings.showDeleteUrl,
                function () { VmItemUtil.deleteItem(show); }
            );
        } else {
            VmItemUtil.deleteItem(show);
        }
    };

    self.getShowView = function (show) {
        return show.IsEditing()
            ? self.settings.showEditView
            : self.settings.showView;
    };

    // Venues
    self.saveVenue = function () {
        if (self.venueErrors().length == 0) {
            var venue = self.selectedVenue();

            if (self.id() != 0) {
                venue.eventMetadataId(self.id());

                ajaxJsonRequest(venue.toJSON(), self.settings.eventVenueSaveUrl,
                    function (referenceShowData) {
                        if (referenceShowData) {
                            venue.allShows.push(new ShowViewModel(referenceShowData));
                        }

                        self.allVenues.push(venue);
                        self.clearSelectedItem(true);
                        self.clearSelectedVenue(false);
                    }
                );
            } else {
                self.allVenues.push(venue);
                self.clearSelectedItem(true);
                self.clearSelectedVenue(false);
            }
        } else {
            self.venueErrors.showAllMessages();
        }
    };

    self.deleteVenue = function (venue) {
        self.clearInner();

        if (self.id() != 0) {
            ajaxJsonRequest(venue.toJSON(), self.settings.eventVenueDeleteUrl,
                function () {
                    VmItemUtil.deleteItem(venue);
                }
            );
        } else {
            VmItemUtil.deleteItem(venue);
        }
    };

    self.createVenue = function () {
        $(self.settings.venueFormName).dialog("open");
    };

    self.getVenues = function (searchTerm, sourceArray) {
        ajaxJsonRequest(
            {
                term: searchTerm,
                eventId: self.id(),
                // TODO: Why passing whole event, maybe just venue ids
                currentEvent: self.id() == 0 ? self.toJSON() : null
            },
            self.settings.venueAutocompleteUrl,
            function (venues) {
                if (venues && venues.length > 0) {
                    sourceArray($.map(venues,
                        function (venue) { return new EntityViewModel(venue); }));
                }
            }
        );
    };

    self.addVenue = function () {
        self.clearInner();

        var venue = EventViewModelExtended.getNewVenue();
        self.allVenues.push(venue);
        self.selectedItem(venue);
    };

    self.cancelVenue = function () {
        self.clearInner();
    };

    self.getVenueView = function (venue) {
        return venue.IsEditing()
            ? self.settings.eventVenueEditView
            : self.settings.eventVenueView;
    };

    self.getHosts = function (searchTerm, sourceArray) {
        ajaxJsonRequest(
            { term: searchTerm },
            self.settings.hostAutocompleteUrl,
            function (hosts) {
                if (hosts && hosts.length > 0) {
                    sourceArray($.map(
                        hosts,
                        function (host) { return new EntityViewModel(host); }));
                }
            }
        );
    };

    self.createHost = function () {
        $(self.settings.hostFormName).dialog("open");
    };
};

inherits(EventViewModelExtended, EventViewModel);

// Static Methods
EventViewModelExtended.setupValidation = function (event, settings) {
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

EventViewModelExtended.setupShowValidation = function (show, event, settings) {
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
            ? VmItemUtil.availableItems(venue.allShows()) : undefined;
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
    EventViewModelExtended.setupShowValidation(show, event, event.settings);
}

EventViewModelExtended.setupDialogs = function (event) {
    $(event.settings.hostFormName).dialog({
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

    $(event.settings.venueFormName).dialog({
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

EventViewModelExtended.attachEvents = function (event) {
    $(event.settings.hostFormName).bind(EntityViewModelExtended.ENTITY_CANCEL_EVENT, function () {
        $(event.settings.hostFormName).dialog("close");
    });

    $(event.settings.hostFormName).bind(EntityViewModelExtended.ENTITY_SAVE_EVENT, function (e) {
        var newHost = new EntityViewModel(e.item.toJSON());
        event.host(newHost);
        e.item.loadData(EventViewModelExtended.getNewHost().toJSON());
        $(event.settings.hostFormName).dialog("close");
    });

    $(event.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_CANCEL_EVENT, function () {
        $(event.settings.venueFormName).dialog("close");
    });

    $(event.settings.venueFormName).bind(EntityViewModelExtended.ENTITY_SAVE_EVENT, function (e) {
        var newVenue = new EntityViewModel(e.item.toJSON());
        event.otherVenues.push(newVenue);
        event.selectedVenue(newVenue);
        e.item.loadData(EventViewModelExtended.getNewVenue().toJSON());
        $(event.settings.venueFormName).dialog("close");
    });
};

EventViewModelExtended.clearItem = function (item, condition, deleteItem) {
    if (item() != null) {
        if (condition() && deleteItem) {
            VmItemUtil.deleteItem(item());
        }

        item(null);
    }
};

EventViewModelExtended.getNewHost = function () {
    return new EntityViewModel({ Id: 0, Type: EntityType.Host, State: VmItemState.Added });
};

EventViewModelExtended.getNewVenue = function () {
    return new EntityViewModel({ Id: 0, Type: EntityType.Venue, State: VmItemState.Added });
};

EventViewModelExtended.AutocompleteOptions = {
    autoFocus: true
};

EventViewModelExtended.getAutoEntityItem = function (entity) {
    var text = entity.name();

    if (entity.displayAddress()) {
        if (entity.displayAddress().address() != "")
            text += "<br /><i class='description'>" + entity.displayAddress().address() + "</i>";
    }

    return text;
};