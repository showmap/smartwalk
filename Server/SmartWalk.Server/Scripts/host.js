function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.HostId = ko.observable(data.HostId);
    self.Type = ko.observable(data.Type);
    self.State = ko.observable(data.State);

    self.Title = ko.observable(data.Title);
    self.Contact = ko.observable(data.Contact);
}

function HostViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);

    self.Name = ko.observable(data.Name);
    self.Description = ko.observable(data.Description);
    self.Picture = ko.observable(data.Picture);

    self.AllContacts = ko.observableArray($.map(data.AllContacts, function (item) { return new ContactViewModel(item); }));

    self.Contacts = ko.computed(function () {        
        return ko.utils.arrayFilter(this.AllContacts(), function (item) {
            return item.State() != 2;
        });
    }, this);

    // Operations
    self.addContact = function () {
        self.AllContacts.push(new ContactViewModel({ Id: 0, HostId: 0, Type: 1, State: 1 }));
    };

    self.removeContact = function (contact) {
        contact.State(2);
    };
}