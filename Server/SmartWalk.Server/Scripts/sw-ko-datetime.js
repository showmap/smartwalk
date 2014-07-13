var myDate = "Aug 04, 2012";
ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var dateFormat = $.datepicker.regional[''].dateFormat;

        element.setAttribute("type", "date");
        if (element.type == "date") {
            ko.utils.registerEventHandler(element, "change", function () {
                var observable = valueAccessor();
                var calcDate = new Date(
                    element.valueAsDate.getFullYear(),
                    element.valueAsDate.getMonth(),
                    element.valueAsDate.getDate(),
                    12);
                observable($.datepicker.formatDate(dateFormat, calcDate));
            });
        } else {
            var options = allBindingsAccessor().datepickerOptions || {};

            $(element).datepicker({
                onSelect: function (value) {
                    var observable = valueAccessor();
                    observable(value);
                }
            });

            if (options.minDate) {
                var minDate = $.datepicker.parseDate(dateFormat, options.minDate);
                $(element).datepicker("option", "minDate", minDate);
            }

            if (options.maxDate) {
                var maxDate = $.datepicker.parseDate(dateFormat, options.maxDate);
                $(element).datepicker("option", "maxDate", maxDate);
            }

            //handle the field changing
            ko.utils.registerEventHandler(element, "change", function () {
                var observable = valueAccessor();
                var newDate = $(element).datepicker("getDate");
                observable(newDate ? $.datepicker.formatDate(dateFormat, newDate) : null);
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                $(element).datepicker("destroy");
            });
        }
    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var dateFormat = $.datepicker.regional[""].dateFormat;
        var value = ko.utils.unwrapObservable(valueAccessor());
        if (value) {
            var valueDate = $.datepicker.parseDate(dateFormat, value);
            
            if (element.type == "date") {
                // TODO: Why?
                element.valueAsDate = new Date(Date.UTC(
                    valueDate.getFullYear(),
                    valueDate.getMonth(),
                    valueDate.getDate(),
                    12));
            } else {
                var current = $(element).datepicker("getDate");
                
                // TODO: To figure this out, looks like WTF
                if (valueDate - current !== 0) {
                    $(element).datepicker("setDate", valueDate);
                }
            }
        } else {
            if (element.type == "date") {
                element.valueAsDate = null;
            } else {
                $(element).datepicker("setDate", null);
            }
        }
    }
};

ko.bindingHandlers.timepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        
        if (element.type != "time") {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().timepickerOptions || {};

            $(element).timepicker({
                stepMinute: options.stepMinute,
                controlType: options.controlType,
                onSelect: function(value) {
                    var observable = valueAccessor();
                    observable(value);
                }
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                $(element).datepicker("destroy");
            });
        } else {
            ko.utils.registerEventHandler(element, "change", function () {
                var observable = valueAccessor();
                observable(element.value);
            });
        }
    },
    //update the control when the view model changes
    update: function(element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());

        if (element.type == "time") {
            element.value = value;
        } else {
            if (value) {
                var valueDate = new Date(myDate + " " + value);
                var current = $(element).datepicker("getDate");
                if (valueDate - current !== 0) {
                    $(element).timepicker("setDate", valueDate);
                }
            } else {
                $(element).timepicker("setDate", null);
            }
        }
    }
};
