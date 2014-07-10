EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    
    self.autocompleteHosts = ko.observableArray();
    self.autocompleteVenues = ko.observableArray();
    self.selectedVenue = ko.observable();
    
    if (self.host()) {
        self.autocompleteHosts.push(self.host());
    }

    self.host.subscribe(function (host) { if (host) self.autocompleteHosts.push(host); });
    
    EventViewModelExtended.setupValidation(self, settings);
    EventViewModelExtended.setupDialogs(self);

    self.venues = ko.computed(function () {
        return self.allVenues()
            ? VmItemUtil.availableItems(self.allVenues()) : undefined;
    });
    
    self.setEditingItem = function (item) {
        if (self.venues()) {
            self.venues().forEach(function (venue) {
                venue.isEditing(item == venue);

                if (venue.shows()) {
                    venue.shows().forEach(function (show) {
                        show.isEditing(item == show);
                    });
                }
            });
        }
    };

    self.venues().forEach(function (venue) {
        EventViewModelExtended.initVenueViewModel(venue, self);
    });

    self.venues.subscribe(function (venues) {
        if (venues) {
            venues.forEach(function (venue) {
                if (venue.isEditing === undefined) {
                    EventViewModelExtended.initVenueViewModel(venue, self);
                }
            });
        }
    });
    
    self.venuesManager = new VmItemsManager(
        self.allVenues,
        self.setEditingItem,
        function () {
            var venue = new EntityViewModel({
                Id: 0,
                Type: EntityType.Venue,
                State: VmItemState.Added
            });
            return venue;
        },
        function (venue) {
            venue.loadData(self.selectedVenue().toJSON());
            venue.eventMetadataId(self.id());
            
            // adding a show reference if there aren't
            if ($.grep(venue.allShows(),
                function (show) { return show.isReference(); }).length == 0) {
                venue.allShows.push(new ShowViewModel({
                    State: VmItemState.Hidden,
                    IsReference: true,
                    EventMetadataId: self.id(),
                    VenueId: venue.id()
                }));
            }

            self.selectedVenue(null);
        });

    self.getShowView = function (show) {
        return show.isEditing()
            ? self.settings.showEditView
            : self.settings.showView;
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

    self.getVenueView = function (venue) {
        return venue.isEditing()
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
    
    self.saveEvent = function () {
        if (self.isValidating()) {
            setTimeout(function () { self.saveEvent(); }, 50);
            return false;
        }

        if (self.errors().length == 0) {
            ajaxJsonRequest(self.toJSON(), self.settings.eventSaveUrl,
                function (eventData) {
                    self.settings.eventAfterSaveUrlHandler(eventData.Id);
                },
                function () {
                    // TODO: To show error message
                }
            );
        } else {
            self.errors.showAllMessages();
        }

        return true;
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
            message: settings.venueRequiredValidationMessage
        }
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
    
    show.isValidating = ko.computed(function () {
        return show.title.isValidating() || show.picture.isValidating() ||
            show.detailsUrl.isValidating() || show.startDate.isValidating() ||
            show.endDate.isValidating();
    });

    show.errors = ko.validation.group(show);
};

EventViewModelExtended.initVenueViewModel = function (venue, event) {
    venue.isEditing = ko.observable(false);
    venue.isEditing.subscribe(function (isEditing) {
        VmItemsManager.processIsEditingChange(
            venue,
            isEditing,
            event.venuesManager
        );
    });

    venue.shows = ko.computed(function () {
        return venue.allShows()
            ? VmItemUtil.availableItems(venue.allShows()) : undefined;
    });

    if (venue.shows()) {
        venue.shows().forEach(function (show) {
            EventViewModelExtended.initShowViewModel(show, venue, event);
        });
    }

    venue.shows.subscribe(function (shows) {
        if (shows) {
            shows.forEach(function (show) {
                if (show.isEditing === undefined) {
                    EventViewModelExtended.initShowViewModel(show, venue, event);
                }
            });
        }
    });
    
    venue.isValidating = ko.computed(function () {
        return event.selectedVenue.isValidating();
    });
    
    venue.errors = ko.validation.group({
        selectedVenue: event.selectedVenue
    });
    
    venue.showsManager = new VmItemsManager(
        venue.allShows,
        event.setEditingItem,
        function () {
            var show = new ShowViewModel({
                Id: 0,
                EventMetadataId: event.id(),
                VenueId: venue.id(),
                State: VmItemState.Added,
                StartDate: event.startTime(), // TODO: To check this out
                EndDate: event.endTime()
            });
            return show;
        });
};

EventViewModelExtended.initShowViewModel = function (show, venue, event) {
    show.isEditing = ko.observable(false);
    show.isEditing.subscribe(function (isEditing) {
        VmItemsManager.processIsEditingChange(
            show,
            isEditing,
            venue.showsManager
        );
    });
    
    EventViewModelExtended.setupShowValidation(show, event, event.settings);
};

EventViewModelExtended.setupDialogs = function (event) {
    var dialogOptions = {
        modal: true,
        autoOpen: false,
        resizable: false,
        width: 700,
        maxHeight: 600,
        close: function () {
            var entity = ko.dataFor(this);
            entity.loadData({});
        },
    };

    var hostOptions = {
        title: event.settings.dialogCreateHostText,
        buttons: [
            {
                "class": "btn btn-default",
                text: event.settings.dialogCancelText,
                click: function () { $(this).dialog("close"); }
            },
            {
                "class": "btn btn-success",
                text: event.settings.dialogAddHostText,
                click: function () {
                    var dialog = this;
                    var host = ko.dataFor(dialog);
                    host.saveEntity(function (entityData) {
                        var newHost = new EntityViewModel(entityData);
                        event.host(newHost);
                        $(dialog).dialog("close");
                    });
                }
            }
        ]};
    $(event.settings.hostFormName).dialog($.extend(dialogOptions, hostOptions));
    
    var venueOptions = {
        title: event.settings.dialogCreateVenueText,
        buttons: [
            {
                "class": "btn btn-default",
                text: event.settings.dialogCancelText,
                click: function () { $(this).dialog("close"); }
            },
            {
                "class": "btn btn-success",
                text: event.settings.dialogAddVenueText,
                click: function () {
                    var dialog = this;
                    var venue = ko.dataFor(dialog);
                    venue.saveEntity(function (entityData) {
                        var newVenue = new EntityViewModel(entityData);
                        event.autocompleteVenues.push(newVenue);
                        event.selectedVenue(newVenue);
                        $(dialog).dialog("close");
                    });
                }
            }
        ]};
    $(event.settings.venueFormName).dialog($.extend(dialogOptions, venueOptions));
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