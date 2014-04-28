var myDate = "Aug 04, 2012";
ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var dateFormat = $.datepicker.regional[''].dateFormat;

        element.setAttribute("type", "date");
        if (element.type != "date") {
            var options = allBindingsAccessor().datepickerOptions || {};

            $(element).datepicker({
                showAnim: options.showAnim,
                onSelect: function(value) {
                    var observable = valueAccessor();
                    //observable($(element).datepicker('getDate'));
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
            ko.utils.registerEventHandler(element, "change", function() {
                var observable = valueAccessor();                
                var newDate = $(element).datepicker("getDate");
                observable($.datepicker.formatDate(dateFormat, newDate));
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                $(element).datepicker("destroy");
            });
        } else {
            ko.utils.registerEventHandler(element, "change", function () {
                var value = valueAccessor();
                var calcDate = new Date(element.valueAsDate.getFullYear(), element.valueAsDate.getMonth(), element.valueAsDate.getDate(), 12);
                value($.datepicker.formatDate(dateFormat, calcDate));
            });
        }
    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        var dateFormat = $.datepicker.regional[''].dateFormat;
        var valueDate = $.datepicker.parseDate(dateFormat, ko.utils.unwrapObservable(value));
        if (element.type != "date") {            
            //alert($.datepicker.parseDate(dateFormat, value));
            var current = $(element).datepicker("getDate");

            if (valueDate - current !== 0) {
                $(element).datepicker("setDate", valueDate);
            }
        } else {
            if(valueDate!= null)
                element.valueAsDate = new Date(valueDate.getFullYear(), valueDate.getMonth(), valueDate.getDate(), 12);
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
                onSelect: function(value) {
                    var observable = valueAccessor();
                    //observable($(element).datepicker('getDate'));
                    observable(value);
                }
            });

            //alert(simpleObjInspect(options));

            //alert($(element).timepicker("getTime"));
            //handle the field changing
            //                ko.utils.registerEventHandler(element, "change", function () {
            //                    var observable = valueAccessor();
            //                    observable($(element).datepicker('getDate'));
            //                });

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
    update: function (element, valueAccessor) {
        var observable = valueAccessor();
        if (!observable())
            observable("00:00");
        
        if (element.type != "time") {
            var value = ko.utils.unwrapObservable(observable);
            var valueDate = new Date(myDate + " " + value);
            var current = $(element).datepicker("getDate");
            //if (value.getTime() - current.getTime() !== 0) {
            if (valueDate - current !== 0) {
                $(element).timepicker("setDate", valueDate);
            }
        } else {
            element.value = observable();
        }
    }
};