EventViewModel = function (data) {
    var self = this;

    EventViewModel.superClass_.constructor.call(self);
    
    self.Id = ko.observable();
    self.Title = ko.observable();
    self.StartTime = ko.observable();
    self.EndTime = ko.observable();
    self.IsPublic = ko.observable();
    self.Picture = ko.observable();
    self.DisplayDate = ko.observable();
    self.Host = ko.observable();

    // TODO: Why we need this method at all?
    self.loadData = function (eventData) {
        self.Id(eventData.Id);
        self.Title(eventData.Title);
        self.StartTime(eventData.StartTime ? eventData.StartTime : '');
        self.EndTime(eventData.EndTime ? eventData.EndTime : '');
        self.IsPublic(eventData.IsPublic);
        self.Picture(eventData.Picture);
        self.DisplayDate(eventData.DisplayDate);
    };
        
    self.loadData(data);       
};

inherits(EventViewModel, ViewModelBase);