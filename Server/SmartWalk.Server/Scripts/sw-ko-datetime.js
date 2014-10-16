ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data(ko.datetimeUtil.ACCESSOR_NAME, valueAccessor);

        var datetimeClass = ko.datetimeUtil.getClassByType(element);

        datetimeClass.initDefaultDate(element, settings);
        datetimeClass.initDate(element, settings);
        $(element).bind("change", datetimeClass.onChangeDate);
        
        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            datetimeClass.dispose(element, datetimeClass.onChangeDate);
        });        
    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var datetimeClass = ko.datetimeUtil.getClassByType(element);
        
        datetimeClass.updateDate(element, value);        
    }
};

ko.bindingHandlers.timepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data(ko.datetimeUtil.ACCESSOR_NAME, valueAccessor);

        var datetimeClass = ko.datetimeUtil.getClassByType(element);

        datetimeClass.initDefaultDate(element, settings);
        datetimeClass.initTime(element, settings);
        $(element).bind("change", datetimeClass.onChangeTime);

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            datetimeClass.dispose(element, datetimeClass.onChangeTime);
        });        
    },
    //update the control when the view model changes
    update: function(element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var datetimeClass = ko.datetimeUtil.getClassByType(element);

        datetimeClass.updateTime(element, value);
    }
};

// ###############    D a t e t i m e   U t i l     ####################

ko.datetimeUtil = {};
ko.datetimeUtil.ACCESSOR_NAME = "datepickerVA";
ko.datetimeUtil.DESTROY_REF = "datepickerDefDARef";

ko.datetimeUtil.getClassByType = function (element) {
    return (element.type == "time" || element.type == "date")
        ? ko.HTML5datetime : ko.datetime;
};

ko.datetimeUtil.initDefaultDate = function (element, settings, setDefaultCallback) {
    if (settings.defaultDateAccessor &&
        ko.isObservable(settings.defaultDateAccessor)) {
        // getting default date from observable
        setDefaultCallback(settings.defaultDateAccessor());

        // and subscribe to listen future changes
        var ref = settings.defaultDateAccessor.subscribe(function (date) {
            setDefaultCallback(date);
        });

        // saving the reference for disponsing
        $(element).data(ko.datetimeUtil.DESTROY_REF, ref);
    }
};

ko.datetimeUtil.dispose = function (element, onChangeHandler, disposeHandler) {
    $(element).unbind("change", onChangeHandler);
    $(element).data(ko.datetimeUtil.ACCESSOR_NAME, null);
    var subscribeRef = $(element).data(ko.datetimeUtil.DESTROY_REF);
    if (subscribeRef) {
        subscribeRef.dispose();
    }
    
    if (disposeHandler) {
        disposeHandler();
    }
};
    
ko.datetimeUtil.restoreDate = function (newTime, existingDate) {
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

ko.datetimeUtil.restoreTime = function (newDate, existingDate) {
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

// ###############    H T M L 5   d a t e t i m e   ####################

ko.HTML5datetime = { };

ko.HTML5datetime.initDefaultDate = function (element, settings) {
    ko.datetimeUtil.initDefaultDate(element, settings,
        function(date) {
            $(element).data("defaultDate", date);
        });
};

ko.HTML5datetime.initDate = function () { };
ko.HTML5datetime.initTime = function () { };

ko.HTML5datetime.onChangeDate = function (args) {
    var element = args.target;
    var newDate = sw.convertToLocal(element.valueAsDate);
    var observable = $(element).data(ko.datetimeUtil.ACCESSOR_NAME)();
    newDate = ko.datetimeUtil.restoreTime(newDate,
        observable() || $(element).data("defaultDate"));
    observable(newDate);
};

ko.HTML5datetime.onChangeTime = function (args) {
    var element = args.target;
    var newTime = sw.convertToLocal(element.valueAsDate);
    var observable = $(element).data(ko.datetimeUtil.ACCESSOR_NAME)();
    newTime = ko.datetimeUtil.restoreDate(newTime,
        observable() || $(element).data("defaultDate"));
    observable(newTime);
};

ko.HTML5datetime.updateDate = function (element, value) {
    $(element).unbind("change", ko.HTML5datetime.onChangeDate);
    element.valueAsDate = sw.convertToUTC(value);
    $(element).bind("change", ko.HTML5datetime.onChangeDate);
};

ko.HTML5datetime.updateTime = function (element, value) {
    $(element).unbind("change", ko.HTML5datetime.onChangeTime);
    element.valueAsDate = sw.convertToUTC(value);
    $(element).bind("change", ko.HTML5datetime.onChangeTime);
};

ko.HTML5datetime.dispose = function (element, onChangeHandler) {
    ko.datetimeUtil.dispose(element, onChangeHandler);
};

// ###############    d a t e t i m e   ####################

ko.datetime = {};

ko.datetime.initDefaultDate = function (element, settings) {
    ko.datetimeUtil.initDefaultDate(element, settings,
        function (date) {
            // ReSharper disable once UnknownCssClass
            if ($(element).hasClass("hasDatepicker")) {
                $(element).datepicker("option", "defaultDate", date);
            } else {
                settings.defaultDate = date;
            }
        });
};

ko.datetime.initDate = function (element, settings) {
    $(element).datepicker(settings);
};

ko.datetime.initTime = function (element, settings) {
    $(element).timepicker(settings);
};

ko.datetime.onChangeDate = function (args) {
    var element = args.target;
    var newDate = $(element).datepicker("getDate");
    var observable = $(element).data(ko.datetimeUtil.ACCESSOR_NAME)();
    newDate = ko.datetimeUtil.restoreTime(newDate, observable());
    observable(newDate);
};

ko.datetime.onChangeTime = function (args) {
    var element = args.target;
    var newTime = $(element).datepicker("getDate");
    var observable = $(element).data(ko.datetimeUtil.ACCESSOR_NAME)();
    newTime = ko.datetimeUtil.restoreDate(newTime, observable());
    observable(newTime);
};

ko.datetime.updateDate = function (element, value) {
    ko.datetime.updateDateTime(element, value);
};

ko.datetime.updateTime = function (element, value) {
    ko.datetime.updateDateTime(element, value, true);
};

ko.datetime.updateDateTime = function (element, value, restoreDate) {
    var current = $(element).datepicker("getDate");

    if (restoreDate) {
        current = ko.datetimeUtil.restoreDate(current, value);
    }

    if (current - value !== 0) {
        $(element).datepicker("disable");
        $(element).datepicker("setDate", value);
        $(element).datepicker("enable");
    }
};

ko.datetime.dispose = function (element, onChangeHandler) {
    ko.datetimeUtil.dispose(element, onChangeHandler,
        function () {
            $(element).datepicker("destroy");
        }
    );
};