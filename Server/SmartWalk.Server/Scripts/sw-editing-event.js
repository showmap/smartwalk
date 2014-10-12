EventViewModelExtended = function (settings, data) {
    var self = this;

    EventViewModelExtended.superClass_.constructor.call(self, data);

    self.settings = settings;
    self.data = new EventViewModel(data);

    self.actualVenues = function() {
        var result = self.data.venues() 
            ? $.grep(self.data.venues(),
                function (venue) { return venue.id() > 0 && !venue._destroy; })
            : [];
        return result;
    };

    EventViewModelExtended.setupAutocomplete(self);
    EventViewModelExtended.setupMultiday(self);
    EventViewModelExtended.setupSorting(self);
    EventViewModelExtended.setupDialogs(self);
    
    self.venuesManager = new VmItemsManager(
        self.data.venues,
        function () {
            var venue = new EntityViewModel({ Type: sw.vm.EntityType.Venue });
            return venue;
        },
        {
            initItem: function (venue) {
                EventViewModelExtended.initVenueViewModel(venue, self);
            },
            beforeEdit: function () {
                self.data.venues().forEach(function(venue) {
                    venue.showsManager.cancelAll();
                });
            },
            beforeSave: function (venue) {
                if (!venue.errors) {
                    EventViewModelExtended.setupVenueValidation(venue, self.settings);
                }
            },
            afterSave: function(venue) {
                if (self.data.venueOrderType() == sw.vm.VenueOrderType.Custom) {
                    venue.eventDetail().sortOrder(self.actualVenues().length);
                }
            },
            afterDelete: function () {
                if (self.data.venueOrderType() == sw.vm.VenueOrderType.Custom) {
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
        if (!self.data.errors) {
            EventViewModelExtended.setupValidation(self.data, settings);
        }
        
        if (self.data.isValidating()) {
            setTimeout(function () { self.saveEvent(); }, 50);
            return false;
        }

        if (self.data.errors().length == 0) {
            self.currentRequest = sw.ajaxJsonRequest(
                self.data.toJSON.apply(self.data),
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

    venue.eventDetailDescription
        .extend({
            maxLength: {
                params: 3000,
                message: settings.descriptionLengthValidationMessage
            }
        });

    venue.errors = ko.validation.group(venue);
};

EventViewModelExtended.initVenueViewModel = function (venue, event) {
    if (!venue.eventDetail()) {
        venue.eventDetail(new EventEntityDetailViewModel({}));
    }

    venue.autocompleteName = ko.pureComputed({
        read: function () { return venue.name() || null; },
        write: function() {}
    });

    venue.autocompleteData = ko.computed({
        read: function () { return null; },
        write: function (venueData) {
            var data = venueData || {};

            if (venue.eventDetail()) {
                data.EventDetail = venue.eventDetail().toJSON.apply(venue.eventDetail());
            }

            venue.loadData.apply(venue, [data]);
        }
    });

    venue.autocompleteName.extend({ notify: "always" });
    venue.name.extend({ notify: "always" });

    venue.eventDetailDescription = ko.pureComputed({
        read: function() {
             return venue.eventDetail().description();
        },
        write: function(value) {
            venue.eventDetail().description(value);
        }
    });

    venue.displayName = ko.computed(function () {
        var number = null;

        if (event.data.venueTitleFormatType() == sw.vm.VenueTitleFormatType.NameAndNumber) {
            switch (event.data.venueOrderType()) {
                case sw.vm.VenueOrderType.Name:
                    number = event.actualVenues().indexOf(venue) + 1;
                    break;

                case sw.vm.VenueOrderType.Custom:
                    number = venue.eventDetail().sortOrder();
                    break;
            }
        }

        return (number ? number + ". " : "") + venue.name();
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
            beforeEdit: function () {
                event.venuesManager.cancelAll();
                event.data.venues().forEach(function (otherVenue) {
                    otherVenue.showsManager.cancelAll();
                });
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

EventViewModelExtended.setupAutocomplete = function (event) {
    var self = event;
    
    self.autocompleteHostName = ko.pureComputed({
        read: function () {
            return self.data.host() ? self.data.host().name() : null;
        },
        write: function () { }
    });

    self.autocompleteHostData = ko.pureComputed({
        read: function () {
            return self.data.host() ? self.data.host().toJSON.apply(self.data.host()) : null;
        },
        write: function (hostData) {
            self.data.host(hostData && $.isPlainObject(hostData)
                ? new EntityViewModel(hostData) : null);
        }
    });

    // to make sure bindings are re-evaluated on empty values
    self.data.host.extend({ notify: "always" });
    self.autocompleteHostName.extend({ notify: "always" });
    
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
                excludeIds: self.data.venues()
                    ? $.map(self.actualVenues(),
                        function (venue) { return venue.id(); })
                    : null
            },
            self.settings.venueAutocompleteUrl,
            callback
        );
    };
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
                        momentDate: moment(self.data.startDate()).add(i, "days")
                    };
                })
            : null;
        return result;
    });

    self.currentDate = ko.observable(undefined);
    self.currentDay = ko.observable(self.isMultiday() ? 1 : undefined);
    
    self.currentDay.subscribe(function (day) {
        EventViewModelExtended.setCurrentDate(self, self.data.startDate(), day);
    });
    
    self.data.startDate.subscribe(function (date) {
        EventViewModelExtended.setCurrentDate(self, date, self.currentDay());
    });

    self.daysCount.subscribe(function () {
        self.currentDay(self.isMultiday() ? 1 : undefined);
    });
    
    if (self.settings.currentDay) {
        self.currentDay(self.settings.currentDay);
    }

    EventViewModelExtended.setCurrentDate(self, self.data.startDate(), self.currentDay());

    self.defaultDate = ko.computed(function() {
         return self.currentDate() || self.data.startDate();
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
    var firstDay = event.data.startDate() ? moment(event.data.startDate()) : undefined;
    var lastDay = event.data.endDate() || event.data.startDate()
        ? moment(event.data.endDate() || event.data.startDate()) : undefined; // using first day as last one to do not lose some out-of-range shows

    var result =
        (firstDay && (tDay.isBefore(firstDay) || tDay.isSame(firstDay)) && day.isSame(firstDay)) || // times ahead of first day
        (tDay.isSame(day) && t.hours() >= nightEdgeHour) ||
        (tDay.isSame(nextDay) && t.hours() < nightEdgeHour) || // late night times go to next day
        (lastDay && (tDay.isAfter(lastDay) || tDay.isSame(lastDay)) && day.isSame(lastDay)); // times behind of last day

    return result;
};

EventViewModelExtended.setupSorting = function (event) {
    var self = event;

    self.data.venueOrderType.subscribe(function (orderType) {
        if (orderType == sw.vm.VenueOrderType.Custom) {
            var isOrderEmpty = 
                $.grep(self.actualVenues(),
                    function (venue) { return !venue.eventDetail().sortOrder(); }).length > 0;
            if (isOrderEmpty) {
                self.updateVenueDetailOrder();
            }
        }

        self.sortVenues();
    });

    self.sortVenues = function () {
        switch (self.data.venueOrderType()) {
            case sw.vm.VenueOrderType.Name:
                self.data.venues.sort(function (left, right) {
                    return left.name().localeCompare(right.name());
                });
                break;

            case sw.vm.VenueOrderType.Custom:
                self.data.venues.sort(function (left, right) {
                    var leftNumber = left.eventDetail() && left.eventDetail().sortOrder()
                        ? left.eventDetail().sortOrder() : 0;
                    var rightNumber = right.eventDetail() && right.eventDetail().sortOrder()
                        ? right.eventDetail().sortOrder() : 0;

                    return leftNumber == rightNumber ? 0 : (leftNumber < rightNumber ? -1 : 1);
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
            entity.data.loadData.apply(entity.data, [{ Type: entity.data.type() }]);
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
                            editingVenue.loadData.apply(editingVenue, [entityData]);
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