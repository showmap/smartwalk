function ContactViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Type = ko.observable();
    self.State = ko.observable();

    self.Title = ko.observable();
    self.Contact = ko.observable();

    self.IsEditing = ko.observable(false);

    self.DisplayContact = ko.computed(function () {
        return (self.Title() ? self.Title() : "") + (self.Contact() ? ' [' + self.Contact() + ']' : "");
    }, self);    

    self.loadData = function (contactData) {
        self.Id(contactData.Id);
        self.EntityId(contactData.EntityId);
        self.Type(contactData.Type);
        self.State(contactData.State);

        self.Title(contactData.Title);
        self.Contact(contactData.Contact);
    };

    self.loadData(data);

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EntityId: self.EntityId(),
            Type: self.Type(),
            State: self.State(),
            Title: self.Title(),
            Contact: self.Contact()
        };
    }, self);
    
    // TODO: To refactor initializing of validation like ShowViewModel
    if (data.validationUrl) {
        self.Contact.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Contact', model: $.parseJSON(ko.toJSON(self.toJSON())) } });
        self.Title.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Title', model: $.parseJSON(ko.toJSON(self.toJSON())) } });

        self.isValidating = ko.computed(function () {
            return self.Contact.isValidating() || self.Title.isValidating();
        }, self);
    };

    if (data.messages) {
        self.Type.extend({ dependencies: [self.Contact] });

        self.Contact
            .extend({ required: { params: true, message: data.messages.contactRequiredValidationMessage } })
            .extend({ maxLength: { params: 255, message: data.messages.contactLengthValidationMessage } })
            .extend({ contactValidation: { allowEmpty: true, contactType: self.Type, messages: data.messages } });
        
        self.Title
            .extend({ maxLength: { params: 255, message: data.messages.contactTitleValidationMessage } });
    };

    self.errors = ko.validation.group(self);
}

function AddressViewModel(data) {
    var self = this;

    self.Id = ko.observable();
    self.EntityId = ko.observable();
    self.Address = ko.observable();
    self.Tip = ko.observable();
    self.State = ko.observable();

    self.Latitude = ko.observable();
    self.Longitude = ko.observable();

    self.IsEditing = ko.observable(false);

    self.GetMapLink = function () {
        if (!self.Address())
            return "";
        var res = self.Address().replace(/&/g, "").replace(/,\s+/g, ",").replace(/\s+/g, "+");
        return "https://www.google.com/maps/embed/v1/place?q=" + res + "&key=AIzaSyAOwfPuE85Mkr-xoNghkIB7enlmL0llMgo";
    };    

    self.loadData = function (addressData) {
        self.Id(addressData.Id);
        self.EntityId(addressData.EntityId);
        self.Address(addressData.Address);
        self.Tip(addressData.Tip);
        self.State(addressData.State);

        self.Latitude(addressData.Latitude);
        self.Longitude(addressData.Longitude);
    };

    self.loadData(data);

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EntityId: self.EntityId(),
            State: self.State(),
            Address: self.Address(),
            Tip: self.Tip(),
            Latitude: self.Latitude(),
            Longitude: self.Longitude()
        };
    }, self);
    
    // TODO: To refactor initializing of validation like ShowViewModel
    if (data.validationUrl) {
        self.Address.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Address', model: $.parseJSON(ko.toJSON(self.toJSON())) } });
        self.Tip.extend({ asyncValidation: { validationUrl: data.validationUrl, propName: 'Tip', model: $.parseJSON(ko.toJSON(self.toJSON())) } });

        self.isValidating = ko.computed(function () {
            return self.Address.isValidating() || self.Tip.isValidating();
        }, self);
    };
    
    if (data.messages) {
        self.Address
            .extend({ required: { params: true, message: data.messages.addressRequiredValidationMessage } })
            .extend({ maxLength: { params: 255, message: data.messages.addressLengthValidationMessage } });

        self.Tip
            .extend({ maxLength: { params: 255, message: data.messages.addressTipValidationMessage } });
    };

    self.errors = ko.validation.group(self);
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

    self.IsEditing = ko.observable(false);

    self.TimeText = ko.computed(function () {
        if (self.EndTime()) {
            return self.StartTime() + '&nbsp-&nbsp' + self.EndTime();
        }

        return self.StartTime();
    }, self);

    self.loadData = function (showData) {
        self.Id(showData.Id);
        self.EventMetadataId(showData.EventMetadataId);
        self.VenueId(showData.VenueId);
        self.IsReference(showData.IsReference);
        self.Title(showData.Title);
        self.Description(showData.Description);
        self.StartDate(showData.StartDate ? showData.StartDate : '');
        self.StartTime(showData.StartTime ? showData.StartTime : '');
        self.EndDate(showData.EndDate ? showData.EndDate : '');
        self.EndTime(showData.EndTime ? showData.EndTime : '');
        self.Picture(showData.Picture);
        self.DetailsUrl(showData.DetailsUrl);
        self.State(showData.State);
    };

    self.loadData(data);

    // TODO: Should not be computed, 'cause it's slows down shit
    self.toJSON = ko.computed(function () {
        return {
            Id: self.Id(),
            EventMetadataId: self.EventMetadataId(),
            VenueId: self.VenueId(),
            IsReference: self.IsReference(),
            Title: self.Title(),
            Description: self.Description(),
            StartDate: self.StartDate(),
            StartTime: self.StartTime(),
            EndDate: self.EndDate(),
            EndTime: self.EndTime(),
            Picture: self.Picture(),
            DetailsUrl: self.DetailsUrl(),
            State: self.State()
        };
    }, self);

    self.extendValidation = function (validationData) {
        self.Title
            .extend({ required: { params: true, message: validationData.messages.titleRequiredValidationMessage } })
            .extend({ maxLength: { params: 255, message: validationData.messages.titleLengthValidationMessage } });

        self.Picture
            .extend({ maxLength: { params: 255, message: validationData.messages.pictureLengthValidationMessage } })
            .extend({ urlValidation: { params: { allowEmpty: true }, message: validationData.messages.pictureValidationMessage } });

        self.DetailsUrl
            .extend({ maxLength: { params: 255, message: validationData.messages.detailsLengthValidationMessage } })
            .extend({ urlValidation: { params: { allowEmpty: true }, message: validationData.messages.detailsValidationMessage } });

        self.StartDate
            .extend({ dateCompareValidation: { params: {
                allowEmpty: true,
                cmp: 'LESS_THAN',
                compareVal: self.EndDate
            }, message: validationData.messages.startDateValidationMessage } })
            .extend({ dateCompareValidation: { params: {
                allowEmpty: true,
                cmp: 'REGION',
                compareVal: validationData.eventDtFrom,
                compareValTo: validationData.eventDtTo
            }, message: validationData.messages.startTimeValidationMessage } });

        self.EndDate
            .extend({ dateCompareValidation: { params: {
                allowEmpty: true,
                cmp: 'GREATER_THAN',
                compareVal: self.EndDate
            }, message: validationData.messages.endDateValidationMessage } })
            .extend({ dateCompareValidation: { params: {
                allowEmpty: true,
                cmp: 'REGION',
                compareVal: validationData.eventDtFrom,
                compareValTo: validationData.eventDtTo
            }, message: validationData.messages.endTimeValidationMessage } });
    };
    
    if (data.messages) {
        self.extendValidation(data);
    };

    self.errors = ko.validation.group(self);
}