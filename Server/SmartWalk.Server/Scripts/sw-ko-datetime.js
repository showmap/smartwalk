ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data(datetimeBase.prototype.accessorName, valueAccessor);

        var datetimeClass = (element.type == "date") ? HTML5datetime : datetime;

        datetimeClass.prototype.initDate(element, settings);
        $(element).bind("change", datetimeClass.prototype.onChangeDate);
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
        $(element).data(datetimeBase.prototype.accessorName, valueAccessor);

        var datetimeObject = (element.type == "time") ? ko.HTML5datetime : ko.datetime;

        datetimeObject.initTime(element, settings);
        $(element).bind("change", datetimeObject.onChangeTime);

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            datetimeObject.dispose(element, datetimeObject.onChangeTime);
        });        
    },
    //update the control when the view model changes
    update: function(element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var datetimeObject = (element.type == "time") ? ko.HTML5datetime : ko.datetime;
        
        datetimeObject.updateTime(element, value);
    }
};

datetimeBase = function() { };
datetimeBase.prototype.accessorName = 'datepickerVA';
datetimeBase.prototype.destroyRef = 'datepickerDefDARef';
    
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

datetimeBase.prototype.InitBase = function (element, settings, setDefaultCallback) {
    if (settings.defaultDateAccessor &&
        ko.isObservable(settings.defaultDateAccessor)) {
        // getting default date from observable
        if (setDefaultCallback)
            setDefaultCallback(settings.defaultDateAccessor());        

        // and subscribe to listen future changes
        var ref = settings.defaultDateAccessor.subscribe(function (date) {
            $(element).data("defaultDate", date);
        });

        // saving the reference for disponsing
        $(element).data(datetimeBase.prototype.destroyRef, ref);
    }
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

datetimeBase.prototype.dispose = function (element, onChangeHandler) {
    $(element).unbind("change", onChangeHandler);
    $(element).datepicker("destroy");
    $(element).data(datetimeBase.prototype.accessorName, null);
    var subscribeRef = $(element).data(datetimeBase.prototype.destroyRef);
    if (subscribeRef) {
        subscribeRef.dispose();
    }
};

HTML5datetime = function () { };
inherits(HTML5datetime, datetimeBase);

HTML5datetime.prototype.initDate = function (element, settings) {
    datetimeBase.prototype.InitBase(element, settings, function (defaultDate) {
        $(element).data("defaultDate", defaultDate);
    });
};

HTML5datetime.prototype.initTime = HTML5datetime.prototype.initDate;

HTML5datetime.prototype.onChangeDate = function (args) {
    var element = args.target;
    var newDate = convertToLocal(element.valueAsDate);
    var observable = $(element).data(datetimeBase.prototype.accessorName)();
    newDate = ko.HTML5datetime.restoreTime(newDate,
        observable() || $(element).data("defaultDate"));
    observable(newDate);
};

HTML5datetime.prototype.onChangeTime = function (args) {
    var element = args.target;
    var newTime = convertToLocal(element.valueAsDate);
    var observable = $(element).data(datetimeBase.prototype.accessorName)();
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

datetime.prototype.initDate_ = function (element, settings) {
    datetimeBase.prototype.InitBase(element, settings, function (defaultDate) {
        settings.defaultDate = defaultDate;
    });
};

datetime.prototype.initDate = function (element, settings) {
    datetime.prototype.initDate_(element, settings);
    $(element).datepicker(settings);
};

datetime.prototype.initTime = function (element, settings) {
    datetime.prototype.initDate_(element, settings);
    $(element).timepicker(settings);
};

datetime.prototype.onChangeDate = function (args) {
    var element = args.target;
    var newDate = $(element).datepicker("getDate");
    var observable = $(element).data(datetimeBase.prototype.accessorName)();
    newDate = datetime.prototype.restoreTime(newDate, observable());
    observable(newDate);
};

datetime.prototype.onChangeTime = function (args) {
    var element = args.target;
    var newTime = $(element).datepicker("getDate");
    var observable = $(element).data(datetimeBase.prototype.accessorName)();
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