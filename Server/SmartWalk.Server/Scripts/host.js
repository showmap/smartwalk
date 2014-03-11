function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.HostId = ko.observable(data.HostId);
    self.Type = ko.observable(data.Type);

    self.Title = ko.observable(data.Title);
    self.Contact = ko.observable(data.Contact);
}

function HostViewModel(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    
    self.Name = ko.observable(data.Name);
    self.Description = ko.observable(data.Desctiption);
    self.Picture = ko.observable(data.Picture);

    self.Contacts = ko.observableArray($.map(data.Contacts, function (item) { return new ContactViewModel(item); }));

    // Operations
    self.addContact = function () {
        self.Contacts.push(new ContactViewModel({ Id: 0, HostId: 0, Type: 1, Title: "Title", Contact: "Contact" }));
    };

    self.removeContact = function (contact) { self.Contacts.remove(contact); };

    self.add = function () {
        var token = '@Html.AntiForgeryTokenValueOrchard()';
        var headers = {};
        // other headers omitted
        headers['__RequestVerificationToken'] = token;

        var ajdata = self ? ko.toJSON(self) : null;

        var urlUpdate = $("#add-host-form").attr("add-host-url");

        var config = {
            async: true,
            url: urlUpdate,
            type: "POST",
            headers: headers,
            data: ajdata,
            dataType: "json",
            cache: false,
            contentType: "application/json; charset=utf-8",
            __RequestVerificationToken: token,
            error: function (e) {
            },
            success: function (data) {
            }
        };
        $.ajax(config);
    };
}