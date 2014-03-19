var myDate = "Aug 04, 2012";
ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};

        $(element).datepicker({
            showAnim: options.showAnim,
            onSelect: function (value) {
                var observable = valueAccessor();
                //observable($(element).datepicker('getDate'));
                observable(value);
            }
        });

        var dateFormat = $(element).datepicker('option', 'dateFormat');
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
            var dateFormat = $(element).datepicker('option', 'dateFormat');
            var newDate = $(element).datepicker("getDate");
            observable($.datepicker.formatDate(dateFormat, newDate));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker("destroy");
        });

    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var dateFormat = $(element).datepicker('option', 'dateFormat');
        var valueDate = $.datepicker.parseDate(dateFormat, value);

        //alert($.datepicker.parseDate(dateFormat, value));
        var current = $(element).datepicker("getDate");

        if (valueDate - current !== 0) {
            $(element).datepicker("setDate", valueDate);
        }
    }
};

ko.bindingHandlers.timepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().timepickerOptions || {};

        $(element).timepicker({
            stepMinute: options.stepMinute,
            onSelect: function (value) {
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
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker("destroy");
        });

    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var valueDate = new Date(myDate + " " + value);
        var current = $(element).datepicker("getDate");
        //if (value.getTime() - current.getTime() !== 0) {
        if (valueDate - current !== 0) {
            $(element).timepicker("setDate", valueDate);
        }
    }
};