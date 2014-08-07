ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data(datetimeBase.prototype.ACCESSOR_NAME, valueAccessor);

        var datetimeClass = (element.type == "date") ? HTML5datetime : datetime;

        datetimeClass.prototype.initDefaultDate(element, settings);
        datetimeClass.prototype.initDate(element, settings);
        $(element).bind("change", datetimeClass.prototype.onChangeDate);
        
        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            datetimeClass.prototype.dispose(element, datetimeClass.prototype.onChangeDate);
        });        
    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var datetimeClass = (element.type == "date") ? HTML5datetime : datetime;
        
        datetimeClass.prototype.updateDate(element, value);        
    }
};

ko.bindingHandlers.timepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data(datetimeBase.prototype.ACCESSOR_NAME, valueAccessor);

        var datetimeClass = (element.type == "time") ? HTML5datetime : datetime;

        datetimeClass.prototype.initDefaultDate(element, settings);
        datetimeClass.prototype.initTime(element, settings);
        $(element).bind("change", datetimeClass.prototype.onChangeTime);

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            datetimeClass.prototype.dispose(element, datetimeClass.prototype.onChangeTime);
        });        
    },
    //update the control when the view model changes
    update: function(element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var datetimeClass = (element.type == "date") ? HTML5datetime : datetime;

        datetimeClass.prototype.updateTime(element, value);
    }
};

datetimeBase = function() { };
datetimeBase.prototype.ACCESSOR_NAME = "datepickerVA";
datetimeBase.prototype.DESTROY_REF = "datepickerDefDARef";

datetimeBase.prototype.initDefaultDate = function (element, settings, setDefaultCallback) {
    if (settings.defaultDateAccessor &&
        ko.isObservable(settings.defaultDateAccessor)) {
        // getting default date from observable
        setDefaultCallback(settings.defaultDateAccessor());

        // and subscribe to listen future changes
        var ref = settings.defaultDateAccessor.subscribe(function (date) {
            setDefaultCallback(date);
        });

        // saving the reference for disponsing
        $(element).data(datetimeBase.prototype.DESTROY_REF, ref);
    }
};

datetimeBase.prototype.dispose = function (element, onChangeHandler) {
    $(element).unbind("change", onChangeHandler);
    $(element).datepicker("destroy");
    $(element).data(datetimeBase.prototype.ACCESSOR_NAME, null);
    var subscribeRef = $(element).data(datetimeBase.prototype.DESTROY_REF);
    if (subscribeRef) {
        subscribeRef.dispose();
    }
};
    
datetimeBase.prototype.restoreDate = function (newTime, existingDate) {
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

datetimeBase.prototype.restoreTime = function (newDate, existingDate) {
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

HTML5datetime = function () { };
inherits(HTML5datetime, datetimeBase);

HTML5datetime.prototype.initDefaultDate = function (element, settings) {
    datetimeBase.prototype.initDefaultDate(element, settings,
        function(date) {
            $(element).data("defaultDate", date);
        });
};

HTML5datetime.prototype.initDate = function () {};
HTML5datetime.prototype.initTime = function () {};

HTML5datetime.prototype.onChangeDate = function (args) {
    var element = args.target;
    var newDate = convertToLocal(element.valueAsDate);
    var observable = $(element).data(datetimeBase.prototype.ACCESSOR_NAME)();
    newDate = HTML5datetime.prototype.restoreTime(newDate,
        observable() || $(element).data("defaultDate"));
    observable(newDate);
};

HTML5datetime.prototype.onChangeTime = function (args) {
    var element = args.target;
    var newTime = convertToLocal(element.valueAsDate);
    var observable = $(element).data(datetimeBase.prototype.ACCESSOR_NAME)();
    newTime = HTML5datetime.prototype.restoreDate(newTime,
        observable() || $(element).data("defaultDate"));
    observable(newTime);
};

HTML5datetime.prototype.updateDate = function (element, value) {
    $(element).unbind("change", HTML5datetime.prototype.onChangeDate);
    element.valueAsDate = convertToUTC(value);
    $(element).bind("change", HTML5datetime.prototype.onChangeDate);
};

HTML5datetime.prototype.updateTime = function (element, value) {
    $(element).unbind("change", HTML5datetime.prototype.onChangeTime);
    element.valueAsDate = convertToUTC(value);
    $(element).bind("change", HTML5datetime.prototype.onChangeTime);
};

datetime = function() {};
inherits(datetime, datetimeBase);

datetime.prototype.initDefaultDate = function (element, settings) {
    datetimeBase.prototype.initDefaultDate(element, settings,
        function (date) {
            if ($(element).hasClass("hasDatepicker")) {
                $(element).datepicker("option", "defaultDate", date);
            } else {
                settings.defaultDate = date;
            }
        });
};

datetime.prototype.initDate = function (element, settings) {
    $(element).datepicker(settings);
};

datetime.prototype.initTime = function (element, settings) {
    $(element).timepicker(settings);
};

datetime.prototype.onChangeDate = function (args) {
    var element = args.target;
    var newDate = $(element).datepicker("getDate");
    var observable = $(element).data(datetimeBase.prototype.ACCESSOR_NAME)();
    newDate = datetime.prototype.restoreTime(newDate, observable());
    observable(newDate);
};

datetime.prototype.onChangeTime = function (args) {
    var element = args.target;
    var newTime = $(element).datepicker("getDate");
    var observable = $(element).data(datetimeBase.prototype.ACCESSOR_NAME)();
    newTime = datetime.prototype.restoreDate(newTime, observable());
    observable(newTime);
};

datetime.prototype.updateDate = function (element, value) {
    var current = $(element).datepicker("getDate");
    if (current - value !== 0) {
        $(element).unbind("change", datetime.prototype.onChangeDate);
        $(element).datepicker("setDate", value);
        $(element).bind("change", datetime.prototype.onChangeDate);
    }
};

datetime.prototype.updateTime = function (element, value) {
    var current = $(element).datepicker("getDate");
    current = datetime.prototype.restoreDate(current, value);
    if (current - value !== 0) {
        $(element).unbind("change", datetime.prototype.onChangeTime);
        $(element).datepicker("setDate", value);
        $(element).bind("change", datetime.prototype.onChangeTime);
    }
};