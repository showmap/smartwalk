EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    self.data = new EventViewModel(data);

    self.hostData = ko.computed({
        read: function () {
            return self.data.host() ? self.data.host().toJSON() : null;
        },
        write: function (hostData) {
            self.data.host(hostData && $.isPlainObject(hostData)
                ? new EntityViewModel(hostData) : null);
        }
    });
    
    self.hostName = ko.computed({
        read: function () {
            return self.data.host() ? self.data.host().name() : null;
        },
        write: function() {}
    });

    EventViewModelExtended.setupMultiday(self);
    EventViewModelExtended.setupDialogs(self);
    
    self.venuesManager = new VmItemsManager(
        self.data.venues,
        function () {
            var venue = new EntityViewModel({ Type: sw.vm.EntityType.Venue });
            return venue;
        },
        {
            setEditingItem: function (editingItem) {
                if (self.data.venues()) {
                    self.data.venues().forEach(function (venue) {
                        venue.isEditing(editingItem == venue);

                        if (venue.shows()) {
                            venue.shows().forEach(function (show) {
                                show.isEditing(editingItem == show);
                            });
                        }
                    });
                }
            },
            initItem: function (venue) {
                EventViewModelExtended.initVenueViewModel(venue, self);
            },
            beforeSave: function (venue) {
                if (!venue.errors) {
                    EventViewModelExtended.setupVenueValidation(venue, self.settings);
                }
            },
            itemView: self.settings.eventVenueView,
            itemEditView: self.settings.eventVenueEditView
        });

    self.createVenue = function () {
        $(self.settings.venueFormName).dialog("open");
    };
    
    self.createHost = function () {
        $(self.settings.hostFormName).dialog("open");
    };
    
    self.getAutocompleteHosts = function (searchTerm, callback) {
        sw.ajaxJsonRequest(
            { term: searchTerm },
            self.settings.hostAutocompleteUrl,
            callback
        );
    };

    self.getAutocompleteVenues = function (searchTerm, callback) {
        sw.ajaxJsonRequest(
            {
                term: searchTerm,
                onlyMine: false,
                excludeIds: self.data.venues() 
                    ? $.map(self.data.venues(),
                        function (venue) { return venue.id(); })
                    : null
            },
            self.settings.venueAutocompleteUrl,
            callback
        );
    };
    
    self.saveEvent = function () {
        if (!self.data.errors) {
            EventViewModelExtended.setupValidation(self.data, settings);
        }
        
        if (self.data.isValidating()) {
            setTimeout(function () { self.saveEvent(); }, 50);
            return false;
        }

        if (self.data.errors().length == 0) {
            self.currentRequest = sw.ajaxJsonRequest(
                self.data.toJSON(),
                self.settings.eventSaveUrl,
                function (eventData) {
                    self.settings.eventAfterSaveAction(eventData.Id, self);
                },
                function (errorResult) {
                    self.handleServerError(errorResult);
                },
                self
            );
        } else {
            self.data.errors.showAllMessages();
        }

        return true;
    };

    self.cancelEvent = function () {
        if (self.isBusy()) {
            self.isBusy(false);
        } else {
            self.settings.eventAfterCancelAction(self);
        }
    };
};

sw.inherits(EventViewModelExtended, ViewModelBase);

// Static Methods
EventViewModelExtended.setupValidation = function (event, settings) {
    event.title
        .extend({
            maxLength: {
                params: 255,
                message: settings.titleLengthValidationMessage
            }
        });

    event.startDate
        .extend({ required: { message: settings.startTimeRequiredValidationMessage } })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "LESS_THAN",
                    compareVal: event.endDate
                },
                message: settings.startTimeCompareValidationMessage
            }
        });

    event.endDate.extend({
        dateCompareValidation: {
            params: {
                allowEmpty: true,
                cmp: "GREATER_THAN",
                compareVal: event.startDate
            },
            message: settings.endTimeCompareValidationMessage
        },
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

    event.description
        .extend({
            maxLength: {
                params: 3000,
                message: settings.descriptionLengthValidationMessage
            }
        });

    event.host.extend({
        required: { message: settings.hostRequiredValidationMessage },
    });

    event.isValidating = ko.computed(function () {
        return event.title.isValidating() ||
            event.startDate.isValidating() ||
            event.endDate.isValidating() ||
            event.picture.isValidating() ||
            event.description.isValidating() ||
            event.host.isValidating();
    });

    event.errors = ko.validation.group(event);
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

    show.startTime
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "LESS_THAN",
                    compareVal: show.endTime
                },
                message: settings.showMessages.startTimeValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "REGION",
                    compareVal: ko.computed(
                        function () { return event.startDate() ? moment(event.startDate()).subtract("days", 1).toDate() : undefined; }),
                    compareValTo: ko.computed(
                        function () { return event.endDate() ? moment(event.endDate()).add("days", 1).toDate() : undefined; }),
                },
                message: settings.showMessages.startDateValidationMessage
            }
        });

    show.endTime
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "GREATER_THAN",
                    compareVal: show.startTime
                },
                message: settings.showMessages.endTimeValidationMessage
            }
        })
        .extend({
            dateCompareValidation: {
                params: {
                    allowEmpty: true,
                    cmp: "REGION",
                    compareVal: ko.computed(
                        function () { return event.startDate() ? moment(event.startDate()).subtract("days", 1).toDate() : undefined; }),
                    compareValTo: ko.computed(
                        function () { return event.endDate() ? moment(event.endDate()).add("days", 2).toDate() : undefined; }), // 2 days, to cover the whole last day + night of next one
                },
                message: settings.showMessages.endDateValidationMessage
            }
        });
    
    show.isValidating = ko.computed(function () {
        return show.title.isValidating() || show.picture.isValidating() ||
            show.detailsUrl.isValidating() || show.startTime.isValidating() ||
            show.endTime.isValidating();
    });

    show.errors = ko.validation.group(show);
};

EventViewModelExtended.setupVenueValidation = function(venue, settings) {
    venue.id
        .extend({
            required: {
                message: settings.venueRequiredValidationMessage
            }
        });

    venue.errors = ko.validation.group(venue);
};

EventViewModelExtended.initVenueViewModel = function (venue, event) {
    venue.showsManager = new VmItemsManager(
        venue.shows,
        function () {
            var show = new ShowViewModel({});
            return show;
        },
        {
            setEditingItem: function (item) {
                 return event.venuesManager.setEditingItem(item);
            },
            filterItem: function (show) {
                return show.isEditing() ||
                    EventViewModelExtended.IsTimeThisDay(show.startTime(), event, 0);
            },
            beforeSave: function (show) {
                if (!show.errors) {
                    EventViewModelExtended.setupShowValidation(show, event.data, event.settings);
                }
            },
            itemView: event.settings.showView,
            itemEditView: event.settings.showEditView
        });
};

EventViewModelExtended.setupMultiday = function (event) {
    var self = event;
    
    self.daysCount = ko.computed(function () {
        if (!self.data.startDate() || !self.data.endDate()) return 0;

        return moment(self.data.endDate()).diff(moment(self.data.startDate()), "days");
    });

    self.isMultiday = ko.computed(function () {
        return self.daysCount() > 0;
    });

    self.days = ko.computed(function () {
        var result = self.isMultiday()
            ? Array.apply(0, Array(self.daysCount() + 1))
                .map(function (x, i) {
                    return {
                        day: i + 1,
                        momentDate: moment(self.data.startDate()).add("days", i)
                    };
                })
            : null;
        return result;
    });

    self.currentDate = ko.observable(self.data.startDate());

    self.currentDay = ko.observable(self.isMultiday() ? 1 : undefined);
    self.currentDay.subscribe(function (day) {
        self.currentDate(self.data.startDate() && day
            ? moment(self.data.startDate()).add("days", day - 1).toDate()
            : undefined);
    });

    self.daysCount.subscribe(function () {
        self.currentDay(self.isMultiday() ? 1 : undefined);
    });
    
    if (self.settings.currentDay) {
        self.currentDay(self.settings.currentDay);
    }
};

EventViewModelExtended.IsTimeThisDay = function(time, event, nightEdgeHour) {
    if (!time || !event.currentDate()) return true; // if time is not set we asume it goes to all days
    
    if (nightEdgeHour != 0 && !nightEdgeHour) {
        nightEdgeHour = 6;
    }

    var t = moment(time);
    var tDay = moment(time).startOf("day");
    var day = moment(event.currentDate());
    var nextDay = moment(event.currentDate()).add("days", 1);
    var firstDay = event.data.startDate() ? moment(event.data.startDate()) : undefined;
    var lastDay = event.data.endDate() ? moment(event.data.endDate()) : undefined;

    var result =
        (firstDay && (tDay.isBefore(firstDay) || tDay.isSame(firstDay)) && day.isSame(firstDay)) || // times ahead of first day
        (tDay.isSame(day) && t.hours() >= nightEdgeHour) ||
        (tDay.isSame(nextDay) && t.hours() < nightEdgeHour) || // late night times go to next day
        (lastDay && (tDay.isAfter(lastDay) || tDay.isSame(lastDay)) && day.isSame(lastDay)); // times behind of last day

    return result;
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
            entity.isBusy(false);
            entity.data.loadData({ Type: entity.data.type() });
            entity.resetServerErrors();
            if (entity.data.errors) {
                entity.data.errors.showAllMessages(false);
            }
        },
    };

    var cancelClickHandler = function () {
        var entity = ko.dataFor(this);
        if (entity.isBusy()) {
            entity.isBusy(false);
        } else {
            $(this).dialog("close");
        }
    };

    var hostOptions = {
        title: event.settings.dialogCreateHostText,
        buttons: [
            {
                "class": "btn btn-default",
                text: event.settings.dialogCancelText,
                click: cancelClickHandler
            },
            {
                "class": "btn btn-success",
                "data-bind": "enable: isEnabled",
                text: event.settings.dialogAddHostText,
                click: function () {
                    var dialog = this;
                    var host = ko.dataFor(dialog);
                    host.saveEntity(function (entityData) {
                        var newHost = new EntityViewModel(entityData);
                        event.data.host(newHost);
                        $(dialog).dialog("close");
                    });
                }
            }
        ]};
    $(event.settings.hostFormName).dialog($.extend(dialogOptions, hostOptions));
    EventViewModelExtended.setupDialogBottomBar(
        $(event.settings.hostFormName),
        event.settings.loadingTemplate);
    
    var venueOptions = {
        title: event.settings.dialogCreateVenueText,
        buttons: [
            {
                "class": "btn btn-default",
                text: event.settings.dialogCancelText,
                click: cancelClickHandler
            },
            {
                "class": "btn btn-success",
                "data-bind": "enable: isEnabled",
                text: event.settings.dialogAddVenueText,
                click: function () {
                    var dialog = this;
                    var venue = ko.dataFor(dialog);
                    venue.saveEntity(function (entityData) {
                        var editingVenue = $.grep(event.data.venues(),
                            function (item) { return item.isEditing(); })[0];
                        if (editingVenue) {
                            editingVenue.loadData(entityData);
                            $(dialog).dialog("close");
                        }
                    });
                }
            }
        ]};
    $(event.settings.venueFormName).dialog($.extend(dialogOptions, venueOptions));
    EventViewModelExtended.setupDialogBottomBar(
        $(event.settings.venueFormName),
        event.settings.loadingTemplate);
};

EventViewModelExtended.setupDialogBottomBar = function (form, loadingTemplate) {
    var buttonSet = form.dialog("instance").uiButtonSet;
    $("<span>")
        .attr("data-bind", "template: { name: '" + loadingTemplate + "'}")
        .prependTo(buttonSet);

    var model = ko.dataFor(form[0]);
    ko.applyBindings(model, buttonSet[0]);
};