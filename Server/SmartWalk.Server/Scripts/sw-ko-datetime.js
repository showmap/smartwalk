ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data("datepickerVA", valueAccessor);
        
        if (element.type == "date") {
            // HTML 5
            ko.datetime.initHTML5DefaultDate(element, settings);
            $(element).bind("change", ko.datetime.onHTML5ChangeDate);
            
            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                ko.datetime.dispose(element, ko.datetime.onHTML5ChangeDate);
            });
        } else {
            ko.datetime.initDefaultDate(element, settings);

            //initialize datepicker with some optional options
            $(element).datepicker(settings);
            $(element).bind("change", ko.datetime.onChangeDate);

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                ko.datetime.dispose(element, ko.datetime.onChangeDate);
            });
        }
    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());

        if (element.type == "date") {
            // HTML 5
            $(element).unbind("change", ko.datetime.onHTML5ChangeDate);
            element.valueAsDate = convertToUTC(value);
            $(element).bind("change", ko.datetime.onHTML5ChangeDate);
        } else {
            var current = $(element).datepicker("getDate");
            if (current - value !== 0) {
                $(element).unbind("change", ko.datetime.onChangeDate);
                $(element).datepicker("setDate", value);
                $(element).bind("change", ko.datetime.onChangeDate);
            }
        }
    }
};

ko.bindingHandlers.timepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data("datepickerVA", valueAccessor);

        if (element.type == "time") {
            // HTML 5
            ko.datetime.initHTML5DefaultDate(element, settings);
            $(element).bind("change", ko.datetime.onHTML5ChangeTime);
            
            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                ko.datetime.dispose(element, ko.datetime.onHTML5ChangeTime);
            });
        } else {
            ko.datetime.initDefaultDate(element, settings);

            //initialize datepicker with some optional options
            $(element).timepicker(settings);
            $(element).bind("change", ko.datetime.onChangeTime);

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                ko.datetime.dispose(element, ko.datetime.onChangeTime);
            });
        }
    },
    //update the control when the view model changes
    update: function(element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());

        if (element.type == "time") {
            // HTML 5
            $(element).unbind("change", ko.datetime.onHTML5ChangeTime);
            element.valueAsDate = convertToUTC(value);
            $(element).bind("change", ko.datetime.onHTML5ChangeTime);
        } else {
            var current = $(element).datepicker("getDate");
            current = ko.datetime.restoreDate(current, value);
            if (current - value !== 0) {
                $(element).unbind("change", ko.datetime.onChangeTime);
                $(element).datepicker("setDate", value);
                $(element).bind("change", ko.datetime.onChangeTime);
            }
        }
    }
};

ko.datetime = {};

ko.datetime.initHTML5DefaultDate = function (element, settings) {
    if (settings.defaultDateAccessor &&
        ko.isObservable(settings.defaultDateAccessor)) {
        // getting default date from observable
        $(element).data("defaultDate", settings.defaultDateAccessor());

        // and subscribe to listen future changes
        var ref = settings.defaultDateAccessor.subscribe(function (date) {
            $(element).data("defaultDate", date);
        });

        // saving the reference for disponsing
        $(element).data("datepickerDefDARef", ref);
    }
};

ko.datetime.initDefaultDate = function (element, settings) {
    if (settings.defaultDateAccessor &&
        ko.isObservable(settings.defaultDateAccessor)) {
        // getting default date from observable
        settings.defaultDate = settings.defaultDateAccessor();
        
        // and subscribe to listen future changes
        var ref = settings.defaultDateAccessor.subscribe(function (date) {
            $(element).datepicker("option", "defaultDate", date);
        });
        
        // saving the reference for disponsing
        $(element).data("datepickerDefDARef", ref);
    }
};

ko.datetime.dispose = function (element, onChangeHandler) {
    $(element).unbind("change", onChangeHandler);
    $(element).datepicker("destroy");
    $(element).data("datepickerVA", null);
    var subscribeRef = $(element).data("datepickerDefDARef");
    if (subscribeRef) {
        subscribeRef.dispose();
    }
};

ko.datetime.onHTML5ChangeDate = function (args) {
    var element = args.target;
    var newDate = convertToLocal(element.valueAsDate);
    var observable = $(element).data("datepickerVA")();
    newDate = ko.datetime.restoreTime(newDate,
        observable() || $(element).data("defaultDate"));
    observable(newDate);
};

ko.datetime.onHTML5ChangeTime = function (args) {
    var element = args.target;
    var newTime = convertToLocal(element.valueAsDate);
    var observable = $(element).data("datepickerVA")();
    newTime = ko.datetime.restoreDate(newTime,
        observable() || $(element).data("defaultDate"));
    observable(newTime);
};

ko.datetime.onChangeDate = function (args) {
    var element = args.target;
    var newDate = $(element).datepicker("getDate");
    var observable = $(element).data("datepickerVA")();
    newDate = ko.datetime.restoreTime(newDate, observable());
    observable(newDate);
};

ko.datetime.onChangeTime = function (args) {
    var element = args.target;
    var newTime = $(element).datepicker("getDate");
    var observable = $(element).data("datepickerVA")();
    newTime = ko.datetime.restoreDate(newTime, observable());
    observable(newTime);
};

ko.datetime.restoreDate = function (newTime, existingDate) {
    if (!newTime || !existingDate) return newTime;

    if (newTime.toDateString() != existingDate.toDateString()) {
        newTime = new Date(
            existingDate.getFullYear(),
            existingDate.getMonth(),
            existingDate.getDate(),
            newTime.getHours(),
            newTime.getMinutes());
    }

    return newTime;
};

ko.datetime.restoreTime = function (newDate, existingDate) {
    if (!newDate || !existingDate) return newDate;

    if (newDate.toTimeString() != existingDate.toTimeString()) {
        newDate = new Date(
            newDate.getFullYear(),
            newDate.getMonth(),
            newDate.getDate(),
            existingDate.getHours(),
            existingDate.getMinutes());
    }

    return newDate;
};