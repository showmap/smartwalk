EventViewModelBase = function (data) {
    EventViewModelBase.superClass_.constructor.call(this);
    
    this.Id = ko.observable();
    this.Title = ko.observable();
    this.StartTime = ko.observable();
    this.EndTime = ko.observable();
    this.IsPublic = ko.observable();
    this.Picture = ko.observable();
    this.DisplayDate = ko.observable();

    EventViewModelBase.prototype.loadData.call(this, data);
};

inherits(EventViewModelBase, ViewModelBase);

EventViewModelBase.prototype.loadData = function(data) {
    this.Id(data.Id);
    this.Title(data.Title);
    this.StartTime(data.StartTime ? data.StartTime : '');
    this.EndTime(data.EndTime ? data.EndTime : '');
    this.IsPublic(data.IsPublic);
    this.Picture(data.Picture);
    this.DisplayDate(data.DisplayDate);
};

EventViewModel = function (data) {    
    EventViewModel.superClass_.constructor.call(this, data);
    
    this.Description = ko.observable();
    this.Latitude = ko.observable();
    this.Longitude = ko.observable();

    this.CombineType = ko.observable();

    this.Host = ko.observable();

    this.AllVenues = ko.observableArray();
    this.AllHosts = ko.observableArray();
    this.OtherVenues = ko.observableArray();

    EventViewModel.prototype.loadData.call(this, data);
};

inherits(EventViewModel, EventViewModelBase);

EventViewModel.prototype.selectedVenue = ko.observable();
EventViewModel.prototype.loadData = function (data) {
    this.Description(data.Description);
    this.Latitude(data.Latitude);
    this.Longitude(data.Longitude);

    this.CombineType(data.CombineType);

    this.AllVenues($.map(data.AllVenues, function (item) { return new EntityViewModelBase(item); }));

    if (data.Host != null) {
        var item = new EntityViewModelBase(data.Host);
        this.Host(item);
        this.AllHosts.push(item);
    }
};

EventViewModel.prototype.Venues = ko.computed(function () {
    return this.Items_ ? this.Items_(this.AllVenues()) : [];
});

EventViewModel.prototype.CheckedShows = ko.computed(function () {
    if (!this.Venues)
        return [];
    
    var venueShows = ko.utils.arrayMap(this.Venues(), function (venue) {
        return venue.CheckedShows();
    });

    var res = new Array();
    for (var i = 0; i < venueShows.length; i++) {
        for (var j = 0; j < venueShows[i].length; j++) {
            res.push(venueShows[i][j]);
        }
    }

    return res;
});
EventViewModel.prototype.AllVenuesChecked = ko.computed({
    read: function () {
        return this.GetAllItemsChecked_ ? this.GetAllItemsChecked_(this.Venues()) : false;
    },
    write: function (value) {
        if (this.SetAllItemsChecked_)
            this.SetAllItemsChecked_(this.Venues(), value);
    },
    owner: self
});

EventViewModel.prototype.CalcVenues = ko.computed(function () {
    if (!this.AllVenues)
        return [];
    return ko.utils.arrayFilter(this.AllVenues(), function (item) {
        return item.Id() == 0;
    });
});

EventViewModel.prototype.addVenue = function () {
    var newVenue = new EntityViewModel({ Id: 0, Type: 1, State: 1 });
    this.AllVenues.push(newVenue);
    this.selectedItem(newVenue);
};
EventViewModel.prototype.removeVenue = function (item) {
    this.DeleteItem_(item);
};

EventViewModel.prototype.cancelVenue = function (item) {
    if (this.selectedItem() && this.selectedItem().Id() == 0) {
        this.removeVenue(this.selectedItem());
    }
    this.selectedItem(null);
};

EventViewModel.prototype.toJSON = function () {
    var copy = ko.toJS(this); //just a quick way to get a clean copy
    delete copy.AllVenues;
    delete copy.Venues;
    delete copy.OtherVenues;
    return copy;
};