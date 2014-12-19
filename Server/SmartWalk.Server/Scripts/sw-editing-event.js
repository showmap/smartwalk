EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    self.model = new EventViewModel(data);

    self.actualVenues = function() {
        var result = self.model.venues() 
            ? $.grep(self.model.venues(),
                function (venue) { return venue.id() > 0 && !venue._destroy; })
            : [];
        return result;
    };

    EventViewModelExtended.setupAutocomplete(self);
    EventViewModelExtended.setupMultiday(self);
    EventViewModelExtended.setupSorting(self);
    EventViewModelExtended.setupDialogs(self);

    self.uploadManager = new FileUploadManager(self, self.model.picture);
    self.showUploadManagers = new Hashtable();
    
    self.venuesManager = new VmItemsManager(
        self.model.venues,
        function () {
            var venue = new EntityViewModel({ Type: sw.vm.EntityType.Venue });
            return venue;
        },
        {
            initItem: function (venue) {
                EventViewModelExtended.initVenueViewModel(venue, self);
            },
            beforeEdit: function () {
                self.model.venues().forEach(function(venue) {
                    venue.showsManager.cancelAll();
                });
            },
            beforeSave: function (venue) {
                if (!venue.errors) {
                    EventViewModelExtended.setupVenueValidation(venue, self.settings);
                }
            },
            afterSave: function (venue) {
                if (!venue.eventDetail().sortOrder()) {
                    venue.eventDetail().sortOrder(self.actualVenues().length);
                }
            },
            afterDelete: function () {
                if (self.model.venueOrderType() == sw.vm.VenueOrderType.Custom) {
                    self.updateVenueDetailOrder();
                }
            },
            itemView: self.settings.eventVenueView,
            itemEditView: self.settings.eventVenueEditView
        });

    self.sortVenues();

    self.createVenue = function () {
        $(self.settings.venueFormName).dialog("open");
    };
    
    self.createHost = function () {
        $(self.settings.hostFormName).dialog("open");
    };
    
    self.saveEvent = function () {
        if (!self.model.errors) {
            EventViewModelExtended.setupValidation(self.model, settings);
        }
        
        if (self.model.isValidating()) {
            setTimeout(function () { self.saveEvent(); }, 50);
            return false;
        }

        if (self.model.errors().length == 0) {
            self.isBusy(true); // explicitly setting busy in case if image is being uploaded
            self.request(self.uploadManager.request());
            self.request().done(self._saveEvent);
        } else {
            self.model.errors.showAllMessages();
        }

        return true;
    };

    self._saveEvent = function () {
        self.request(sw.ajaxJsonRequest(
                self.model.toJSON(), self.settings.eventSaveUrl, self)
            .done(function (eventData) {
                data = eventData;
                self.model.loadData(eventData);
                self.settings.eventAfterSaveAction(eventData.Id, self);
            })
            .fail(function (errorResult) {
                self.handleServerError(errorResult);
            }));
    };

    self.cancelEvent = function () {
        if (self.isBusy()) {
            self.isBusy(false);
        } else {
            self.settings.eventAfterCancelAction(self);
        }
    };

    self.onWindowClose = function () {
        var initModel = new EventViewModel(data);

        if (JSON.stringify(initModel.toJSON()) != JSON.stringify(self.model.toJSON())) {
            return settings.unsavedChangesMessage;
        }

        return undefined;
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
            event.description.isValidating() ||
            event.host.isValidating();
    });

    event.errors = ko.validation.group(event);
};

EventViewModelExtended.setupShowValidation = function (show, event, settings) {
    show.title
        .extend({ required: { params: true, message: settings.showMessages.titleRequiredValidationMessage } })
        .extend({ maxLength: { params: 255, message: settings.showMessages.titleLengthValidationMessage } });

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
                        function () {
                            return event.startDate()
                                ? moment(event.startDate()).subtract(1, "days").toDate()
                                : undefined;
                        }),
                    compareValTo: ko.computed(
                        function () {
                            return event.endDate() || event.startDate()
                                ? moment(event.endDate() || event.startDate()).add(1, "days").toDate()
                                : undefined;
                        }),
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
                        function () {
                            return event.startDate()
                                ? moment(event.startDate()).subtract(1, "days").toDate()
                                : undefined;
                        }),
                    compareValTo: ko.computed(
                        function () {
                            return event.endDate() || event.startDate()
                                ? moment(event.endDate() || event.startDate()).add(2, "days").toDate()
                                : undefined;
                        }), // 2 days, to cover the whole last day + night of next one
                },
                message: settings.showMessages.endDateValidationMessage
            }
        });
    
    show.isValidating = ko.computed(function () {
        return show.title.isValidating() || show.detailsUrl.isValidating() ||
            show.startTime.isValidating() || show.endTime.isValidating();
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

    venue.eventDetail().description
        .extend({
            maxLength: {
                params: 3000,
                message: settings.descriptionLengthValidationMessage
            }
        });

    venue.errors = ko.validation.group([venue.id, venue.eventDetail().description]);
};

EventViewModelExtended.initVenueViewModel = function (venue, event) {
    venue.autocompleteName = ko.pureComputed({
        read: function () { return venue.name() || null; },
        write: function() {}
    });

    venue.autocompleteData = ko.pureComputed({
        read: function () { return null; },
        write: function (venueData) {
            var data = venueData || {};

            // keeping current venue details for new venue
            if (venue.eventDetail()) {
                data.EventDetail = venue.eventDetail().toJSON();
            }

            // keeping current shows for new venue
            if (venue.shows()) {
                data.Shows = $.map(venue.shows(), function (show) { return show.toJSON(); });
            }

            venue.loadData(data);
        }
    });

    venue.autocompleteName.extend({ notify: "always" });
    venue.name.extend({ notify: "always" });

    venue.number = ko.computed(function () {
        var number = event.model.venueTitleFormatType() == sw.vm.VenueTitleFormatType.NameAndNumber
            ? event.actualVenues().indexOf(venue) + 1
            : null;

        return number ? number + ". " : "";
    });
    
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
                    EventViewModelExtended.isTimeThisDay(show.startTime(), event, 0);
            },
            beforeEdit: function (show) {
                event.venuesManager.cancelAll();
                event.model.venues().forEach(function (otherVenue) {
                    otherVenue.showsManager.cancelAll();
                });

                var uploadManager = event.showUploadManagers.get(show);
                if (!uploadManager) {
                    uploadManager = new FileUploadManager(event, show.picture);
                    event.showUploadManagers.put(show, uploadManager);
                }

                venue.showsManager.request = uploadManager.request;
            },
            beforeSave: function (show) {
                if (!show.errors) {
                    EventViewModelExtended.setupShowValidation(show, event.model, event.settings);
                }

                event.isBusy(true);
            },
            afterSave: function() {
                event.isBusy(false);
            },
            saveFailed: function () {
                event.isBusy(false);
            },
            itemView: event.settings.showView,
            itemEditView: event.settings.showEditView
        });
};

EventViewModelExtended.setupAutocomplete = function (event) {
    var self = event;
    
    self.autocompleteHostName = ko.pureComputed({
        read: function () {
            return self.model.host() ? self.model.host().name() : null;
        },
        write: function () { }
    });

    self.autocompleteHostData = ko.pureComputed({
        read: function () { return null; },
        write: function (hostData) {
            self.model.host(hostData && $.isPlainObject(hostData)
                ? new EntityViewModel(hostData) : null);
        }
    });

    // to make sure bindings are re-evaluated on empty values
    self.model.host.extend({ notify: "always" });
    self.autocompleteHostName.extend({ notify: "always" });
    
    self.getAutocompleteHosts = function (searchTerm, callback) {
        sw.ajaxJsonRequest({ term: searchTerm }, self.settings.hostAutocompleteUrl)
            .done(callback);
    };

    self.getAutocompleteVenues = function (searchTerm, callback) {
        sw.ajaxJsonRequest(
            {
                term: searchTerm,
                excludeIds: $.map(self.actualVenues(), function (venue) { return venue.id(); })
            },
            self.settings.venueAutocompleteUrl).done(callback);
    };
};

EventViewModelExtended.setupMultiday = function (event) {
    var self = event;
    
    self.daysCount = ko.computed(function () {
        if (!self.model.startDate() || !self.model.endDate()) return 0;

        return moment(self.model.endDate()).diff(moment(self.model.startDate()), "days");
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
                        momentDate: moment(self.model.startDate()).add(i, "days")
                    };
                })
            : null;
        return result;
    });

    self.currentDate = ko.observable(undefined);
    self.currentDay = ko.observable(self.isMultiday() ? 1 : undefined);
    
    self.currentDay.subscribe(function (day) {
        EventViewModelExtended.setCurrentDate(self, self.model.startDate(), day);
    });
    
    self.model.startDate.subscribe(function (date) {
        EventViewModelExtended.setCurrentDate(self, date, self.currentDay());
    });

    self.daysCount.subscribe(function () {
        self.currentDay(self.isMultiday() ? 1 : undefined);
    });
    
    if (self.settings.currentDay) {
        self.currentDay(self.settings.currentDay);
    }

    EventViewModelExtended.setCurrentDate(self, self.model.startDate(), self.currentDay());

    self.defaultDate = ko.computed(function() {
         return self.currentDate() || self.model.startDate();
    });
};

EventViewModelExtended.setCurrentDate = function(event, startDate, day) {
    var self = event;

    if (startDate && day) {
        self.currentDate(moment(startDate).add(day - 1, "days").toDate());
    } else {
        self.currentDate(undefined);
    }
};

EventViewModelExtended.isTimeThisDay = function(time, event, nightEdgeHour) {
    if (!time || !event.currentDate()) return true; // if time is not set we asume it goes to all days
    
    if (nightEdgeHour != 0 && !nightEdgeHour) {
        nightEdgeHour = 6;
    }

    var t = moment(time);
    var tDay = moment(time).startOf("day");
    var day = moment(event.currentDate());
    var nextDay = moment(event.currentDate()).add(1, "days");
    var firstDay = event.model.startDate() ? moment(event.model.startDate()) : undefined;
    var lastDay = event.model.endDate() || event.model.startDate()
        ? moment(event.model.endDate() || event.model.startDate()) : undefined; // using first day as last one to do not lose some out-of-range shows

    var result =
        (firstDay && (tDay.isBefore(firstDay) || tDay.isSame(firstDay)) && day.isSame(firstDay)) || // times ahead of first day
        (tDay.isSame(day) && t.hours() >= nightEdgeHour) ||
        (tDay.isSame(nextDay) && t.hours() < nightEdgeHour) || // late night times go to next day
        (lastDay && (tDay.isAfter(lastDay) || tDay.isSame(lastDay)) && day.isSame(lastDay)); // times behind of last day

    return result;
};

EventViewModelExtended.setupSorting = function (event) {
    var self = event;

    self.model.venueOrderType.subscribe(function (orderType) {
        self.sortVenues();

        if (orderType == sw.vm.VenueOrderType.Custom) {
            self.updateVenueDetailOrder();
        }
    });

    self.sortVenues = function () {
        switch (self.model.venueOrderType()) {
            case sw.vm.VenueOrderType.Name:
                self.model.venues.sort(function (left, right) {
                    return left.name().localeCompare(right.name());
                });
                break;

            case sw.vm.VenueOrderType.Custom:
                self.model.venues.sort(function (left, right) {
                    var leftNum = (left.eventDetail() && left.eventDetail().sortOrder()) || 0;
                    var rightNum = (right.eventDetail() && right.eventDetail().sortOrder()) || 0;

                    return leftNum == rightNum ? 0 : (leftNum < rightNum ? -1 : 1);
                });
                break;
        }
    };

    self.updateVenueDetailOrder = function () {
        var i = 1;
        self.actualVenues().forEach(function (venue) {
            venue.eventDetail().sortOrder(i++);
        });
    };

    self.moveVenueUp = function (venue) {
        var sortOrder = venue.eventDetail().sortOrder();
        venue.eventDetail().sortOrder(sortOrder - 1.5);

        self.sortVenues();
        self.updateVenueDetailOrder();
    };

    self.moveVenueDown = function(venue) {
        var sortOrder = venue.eventDetail().sortOrder();
        venue.eventDetail().sortOrder(sortOrder + 1.5);

        self.sortVenues();
        self.updateVenueDetailOrder();        
    };
}

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
            entity.model.loadData({ Type: entity.model.type() });
            entity.resetServerErrors();
            if (entity.model.errors) {
                entity.model.errors.showAllMessages(false);
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
                        event.model.host(newHost);
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
                        var editingVenue = $.grep(event.model.venues(),
                            function (item) { return item.isEditing(); })[0];
                        if (editingVenue) {
                            editingVenue.autocompleteData(entityData);
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