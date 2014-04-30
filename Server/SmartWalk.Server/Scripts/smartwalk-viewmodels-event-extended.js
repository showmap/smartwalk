//EventViewModelExtended class
EventViewModelExtended = function (settings, data) {
    EventViewModelExtended.superClass_.constructor.call(this, data);

    this.hostFormName = settings.hostFormName;
    this.venueFormName = settings.venueFormName;
    
    this.eventSaveUrl = settings.eventSaveUrl;
    
    this.showGetUrl = settings.showGetUrl;
    this.showSaveUrl = settings.showSaveUrl;
    this.showDeleteUrl = settings.showDeleteUrl;
    
    this.showView = settings.showView;
    this.showEditView = settings.showEditView;

    this.eventVenueSaveUrl = settings.eventVenueSaveUrl;
    this.eventVenueDeleteUrl = settings.eventVenueDeleteUrl;
    this.eventVenuesDeleteUrl = settings.eventVenuesDeleteUrl;

    this.eventVenueView = settings.eventVenueView;
    this.eventVenueEditView = settings.eventVenueEditView;

    this.eventShowsDeleteUrl = settings.eventShowsDeleteUrl;

    this.hostAutocompleteUrl = settings.hostAutocompleteUrl;
    this.venueAutocompleteUrl = settings.venueAutocompleteUrl;

    this.attachEvents();
    this.setupDialogs();
};

inherits(EventViewModelExtended, EventViewModel);

EventViewModelExtended.prototype.setupDialogs = function () {
    $(this.hostFormName).dialog({
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

    $(this.venueFormName).dialog({
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

    $(self.hostFormName).bind(EntityViewModelExtended.HOST_CANCEL_EVENT, function (event) {
        $(self.hostFormName).dialog("close");
    });

    $(self.hostFormName).bind(EntityViewModelExtended.HOST_SAVE_EVENT, function (event) {
        self.AllHosts.push(event.item);
        self.Host(event.item);
        $(self.hostFormName).dialog("close");
    });

    $(self.venueFormName).bind(EntityViewModelExtended.VENUE_CANCEL_EVENT, function (event) {
        $(self.venueFormName).dialog("close");
    });

    $(self.venueFormName).bind(EntityViewModelExtended.VENUE_SAVE_EVENT, function (event) {        
        self.OtherVenues.push(event.item);
        self.selectedVenue(event.item);

        $(self.venueFormName).dialog("close");
    });
};

EventViewModelExtended.prototype.saveEvent = function () {
    var ajdata = ko.toJSON(this);

    ajaxJsonRequest(ajdata, this.eventSaveUrl,
        function (data) {            
            window.location.href = "/event/" + data.Id;
        }
    );
};

// Show
EventViewModelExtended.prototype.addShow = function (root, item) {
    root.selectedItem(item.addShow());
    //item.addShow();
};

EventViewModelExtended.prototype.cancelShow = function (root, item) {
    root.selectedItem(null);

    if (item.Id() == 0) {
        root.DeleteItem_(item);
    } else {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.showGetUrl,
            function (data) {
                if (data)
                    item.loadData(data);
            }
        );
    }
};

EventViewModelExtended.prototype.saveShow = function (root, item) {
    root.selectedItem(null);
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.showSaveUrl,
            function(data) {
                if (item.Id() == 0 || item.Id() != data)
                    item.Id(data);
            }
        );
    }
};

EventViewModelExtended.prototype.deleteShow = function (root, item) {
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(item);

        ajaxJsonRequest(ajdata, root.showDeleteUrl,
            function(data) {
                root.DeleteItem_(item);
            }
        );
    } else {
        root.DeleteItem_(item);
    }
};

EventViewModelExtended.prototype.deleteShows = function (root) {
    if (root.CheckedShows().length > 0) {
        var ajdata = ko.toJSON(root.CheckedShows);

        if (root.Id() != 0) {
            ajaxJsonRequest(ajdata, root.eventShowsDeleteUrl,
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
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.showEditView : bindingContext.$root.showView;
};

EventViewModelExtended.prototype.saveVenue = function (root, item) {
    if (root.Id() != 0) {
        if (root.selectedItem().Id() == 0)
            root.DeleteItem_(root.selectedItem());

        var venue = root.selectedVenue();

        //alert('item id = ' + vm.selectedVenue().Id() + ' selected item id = ' + vm.selectedItem().Id());
        venue.EventMetadataId(root.Id());
        var ajdata = ko.toJSON(venue);

        ajaxJsonRequest(ajdata, root.eventVenueSaveUrl,
            function(data) {
                if (data)
                    venue.AllShows.push(new ShowViewModel(data));
                root.AllVenues.push(venue);
                root.selectedItem(null);
                root.selectedVenue(null);
            }
        );
    } else {
        root.selectedItem(null);
        root.selectedVenue(null);
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

EventViewModelExtended.prototype.deleteVenues = function (root) {
    if (root.Id() != 0) {
        if (root.CheckedVenues().length > 0) {
            var ajdata = ko.toJSON(root.CheckedVenues);

            ajaxJsonRequest(ajdata, root.eventVenuesDeleteUrl,
                function(data) {
                    root.deleteVenuesClientSide();
                }
            );
        }
    } else {
        root.deleteVenuesClientSide();
    }
};

EventViewModelExtended.prototype.deleteVenue = function (root, item) {
    if (root.Id() != 0) {
        var ajdata = ko.toJSON(item.toJSON());

        ajaxJsonRequest(ajdata, root.eventVenueDeleteUrl,
            function(data) {
                if (data && data.length > 0) {
                    root.DeleteItem_(item);
                }
            }
        );
    } else {
        root.DeleteItem_(item);
    }
};

EventViewModelExtended.prototype.createVenue = function (root) {
    $(root.venueFormName).dialog("open");
};

EventViewModelExtended.prototype.getVenues = function (searchTerm, sourceArray) {
    var ajdata = JSON.stringify({ term: searchTerm, eventId: this.Id() });

    ajaxJsonRequest(ajdata, this.venueAutocompleteUrl,
        function (data) {
            if (data && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    sourceArray($.map(data, function (item) { return new EntityViewModel(item); }));
                }
            }
        }
    );
};

EventViewModelExtended.prototype.getVenueView = function (item, bindingContext) {
    return item === bindingContext.$root.selectedItem() ? bindingContext.$root.eventVenueEditView : bindingContext.$root.eventVenueView;
};

EventViewModelExtended.prototype.getHosts = function(searchTerm, sourceArray) {
    var ajdata = JSON.stringify({ term: searchTerm });
    ajaxJsonRequest(ajdata, this.hostAutocompleteUrl,
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
    $(this.hostFormName).dialog("open");
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