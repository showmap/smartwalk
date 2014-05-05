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

    this.CheckedVenues = ko.computed(function () {
        return this.CheckedItems_.call(this, this.Venues());
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

    this.loadDataEventViewModel(data);
    
    this.toJSON = ko.computed(function () {
        
        return {
            Id: this.Id(),
            Title: this.Title(),
            StartTime: this.StartTime(),
            EndTime: this.EndTime(),
            IsPublic: this.IsPublic(),
            Picture: this.Picture(),

            Description: this.Description(),
            Latitude: this.Latitude(),
            Longitude: this.Longitude(),

            CombineType: this.CombineType(),

            Host:  this.Host() ? this.Host().toJSON() : "",

            AllVenues:  ko.utils.arrayMap(this.Venues(), function (venue) {
                return venue.toJSON();
            }),
        };
    }, this);
};

inherits(EventViewModel, EventViewModelBase);

EventViewModel.prototype.loadDataEventViewModel = function (data) {
    this.Description(data.Description);
    this.Latitude(data.Latitude);
    this.Longitude(data.Longitude);

    this.CombineType(data.CombineType);
    this.AllVenues($.map(data.AllVenues, function (item) { return new EntityViewModel(item); }));
    
    if (data.Host != null) {        
        var item = new EntityViewModel(data.Host);
        this.Host(item);
        this.AllHosts.push(item);
    }
};