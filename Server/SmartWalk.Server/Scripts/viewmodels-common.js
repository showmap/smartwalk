function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Type = ko.observable();
    self.State = ko.observable();
    self.IsChecked = ko.observable();

    self.Title = ko.observable();
    self.Contact = ko.observable();

    self.DisplayContact = ko.computed(function () {
        return (self.Title() ? self.Title() : "") + (self.Contact() ? ' [' + self.Contact() + ']' : "");
    }, self);

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EntityId(data.EntityId);
        self.Type(data.Type);
        self.State(data.State);
        self.IsChecked(false);

        self.Title(data.Title);
        self.Contact(data.Contact);
    };

    self.loadData(data);
}

function AddressViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Address = ko.observable();
    self.Tip = ko.observable();
    self.State = ko.observable();
    self.IsChecked = ko.observable();

    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.GetMapLink = function () {
        if (!self.Address())
            return "";
        var res = self.Address().replace(/&/g, "").replace(/,\s+/g, ",").replace(/\s+/g, "+");
        return "https://www.google.com/maps/embed/v1/place?q=" + res + "&key=AIzaSyAOwfPuE85Mkr-xoNghkIB7enlmL0llMgo";
    };

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EntityId(data.EntityId);
        self.Address(data.Address);
        self.Tip(data.Tip);
        self.State(data.State);
        self.IsChecked(false);

        self.Latitude(data.Latitude);
        self.Longitude(data.Longitude);
    };

    self.loadData(data);
}

function ShowViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EventMetadataId = ko.observable();
    self.VenueId = ko.observable();
    self.IsReference = ko.observable();
    self.Title = ko.observable();
    self.Description = ko.observable();
    self.StartDate = ko.observable();
    self.StartTime = ko.observable();
    self.EndDate = ko.observable();
    self.EndTime = ko.observable();
    self.Picture = ko.observable();
    self.DetailsUrl = ko.observable();
    self.State = ko.observable();

    self.IsChecked = ko.observable();


    self.TimeText = ko.computed(function () {
        if (self.EndTime()) {
            return self.StartTime() + '&nbsp-&nbsp' + self.EndTime();
        }

        return self.StartTime();
    }, this);

    self.loadData = function (data) {
        self.Id(data.Id);
        self.EventMetadataId(data.EventMetadataId);
        self.VenueId(data.VenueId);
        self.IsReference(data.IsReference);
        self.Title(data.Title);
        self.Description(data.Description);
        self.StartDate(data.StartDate ? data.StartDate : '');
        self.StartTime(data.StartTime ? data.StartTime : '');
        self.EndDate(data.EndDate ? data.EndDate : '');
        self.EndTime(data.EndTime ? data.EndTime : '');
        self.Picture(data.Picture);
        self.DetailsUrl(data.DetailsUrl);
        self.State(data.State);
        self.IsChecked(false);
    };

    self.loadData(data);
}