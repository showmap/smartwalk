//EventViewModelBase class
EventViewModelBase = function (data) {
    EventViewModelBase.superClass_.constructor.call(this);
    
    this.Id = ko.observable();
    this.Title = ko.observable();
    this.StartTime = ko.observable();
    this.EndTime = ko.observable();
    this.IsPublic = ko.observable();
    this.Picture = ko.observable();
    this.DisplayDate = ko.observable();

    this.loadData(data);
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

//EventViewModel class
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

    this.selectedVenue = ko.observable();

    this.Venues = ko.computed(function () {
        return this.Items_(this.AllVenues());
    }, this);

    this.CheckedShows = ko.computed(function () {
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
    }, this);
    
    this.AllVenuesChecked = ko.computed({
        read: function () {
            return this.GetAllItemsChecked_(this.Venues());
        },
        write: function (value) {
            this.SetAllItemsChecked_(this.Venues(), value);
        }
    }, this);

    this.CalcVenues = ko.computed(function () {
        return ko.utils.arrayFilter(this.AllVenues(), function (item) {
            return item.Id() == 0;
        });
    }, this);

    this.loadDataEventViewModel(data);
};

inherits(EventViewModel, EventViewModelBase);

EventViewModel.prototype.loadDataEventViewModel = function (data) {
    this.Description(data.Description);
    this.Latitude(data.Latitude);
    this.Longitude(data.Longitude);

    this.CombineType(data.CombineType);
    this.AllVenues($.map(data.AllVenues, function (item) { return new EntityViewModel(item); }));
    
    if (data.Host != null) {
        var item = new EntityViewModelBase(data.Host);
        this.Host(item);
        this.AllHosts.push(item);
    }
};

EventViewModel.prototype.addVenue = function (root) {
    var newVenue = new EntityViewModel({ Id: 0, Type: 1, State: 1 });
    root.AllVenues.push(newVenue);
    root.selectedItem(newVenue);
    //root.selectedItem(root.AllVenues()[root.AllVenues().length-1]);
    //root.selectedVenue(newVenue);
};

EventViewModel.prototype.removeVenue = function (item) {
    this.DeleteItem_(item);
};

EventViewModel.prototype.cancelVenue = function (root) {
    if (root.selectedItem() && root.selectedItem().Id() == 0) {
        root.removeVenue(root.selectedItem());
    }
    root.selectedItem(null);
};

EventViewModel.prototype.toJSON = function () {
    var copy = ko.toJS(this); //just a quick way to get a clean copy
    delete copy.AllVenues;
    delete copy.Venues;
    delete copy.OtherVenues;
    return copy;
};