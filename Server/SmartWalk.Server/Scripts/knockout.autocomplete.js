//jqAuto -- main binding (should contain additional options to pass to autocomplete)
//jqAutoSource -- the array to populate with choices (needs to be an observableArray)
//jqAutoQuery -- function to return choices (if you need to return via AJAX)
//jqAutoValue -- where to write the selected value
//jqAutoSourceLabel -- the property that should be displayed in the possible choices
//jqAutoSourceImage -- the image property to use for the value
//jqAutoSourceInputValue -- the property that should be displayed in the input box
//jqAutoSourceValue -- the property to use for the value
ko.bindingHandlers.jqAuto = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var allBindings = allBindingsAccessor(),
            options = allBindings.jqOptions || {},
            unwrap = ko.utils.unwrapObservable,
            modelValue = valueAccessor(),
            source = allBindings.jqAutoSource,
            query = allBindings.jqAutoQuery,
            autocompleteItemText = allBindings.jqAutoItemText,
            valueProp = allBindings.jqAutoSourceValue,
            inputValueProp = allBindings.jqAutoSourceInputValue || valueProp,
            labelProp = allBindings.jqAutoSourceLabel || inputValueProp;

        //function that is shared by both select and change event handlers
        function writeValueToModel(valueToWrite) {
            if (ko.isWriteableObservable(modelValue)) {
                modelValue(valueToWrite);
            } else {  //write to non-observable
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['jqAutoValue'])
                    allBindings['_ko_property_writers']['jqAutoValue'](valueToWrite);
            }
        }

        $(element).change(function () {
            if($(element).val() == '')
                writeValueToModel(null);
        });

        //on a selection write the proper value to the model
        options.select = function (event, ui) {
            writeValueToModel(ui.item ? ui.item.item : null);
        };

        //on a change, make sure that it is a valid value or clear out the model value
        options.change = function (event, ui) {
            var currentValue = $(element).val();
            
            if (currentValue == "") {
                writeValueToModel(null);
            } else {
                var matchingItem = ko.utils.arrayFirst(unwrap(source), function(item) {
                    return unwrap(item[inputValueProp]) === currentValue;
                });

                if (!matchingItem) {
                    writeValueToModel(null);
                }
            }
        };

        //hold the autocomplete current response
        var currentResponse = null;

        //handle the choices being updated in a DO, to decouple value updates from source (options) updates
        var mappedSource = ko.computed({
            read: function () {
                var mapped = ko.utils.arrayMap(unwrap(source), function (item) {
                    var result = {};
                    result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
                    result.value = inputValueProp ? unwrap(item[inputValueProp]) : unwrap(item).toString();  //show in input box
                    result.actualValue = valueProp ? unwrap(item[valueProp]) : item;  //store in model
                    result.item = item;
                    return result;
                });
                return mapped;
            },
            write: function (newValue) {
                source(newValue);  //update the source observableArray, so our mapped value (above) is correct
                if (currentResponse) {
                    currentResponse(mappedSource());
                }
            }
        });

        if (query) {
            options.source = function(request, response) {
                currentResponse = response;
                query.call(bindingContext.$root, request.term, mappedSource);
            };
        } else {
            //whenever the items that make up the source are updated, make sure that autocomplete knows it
            mappedSource.subscribe(function (newValue) {
                $(element).autocomplete("option", "source", newValue);
            });

            options.source = mappedSource();
        }

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).autocomplete("destroy");
        });
      
        //initialize autocomplete
        if (autocompleteItemText)
            $(element).autocomplete(options).data("ui-autocomplete")._renderItem = function (ul, item) {
                return $("<li></li>")
                    .data("item.autocomplete", item)
                    .append("<a>" + autocompleteItemText.call(this, item.item) + "</a>")
                    .appendTo(ul);
            };
        else
            $(element).autocomplete(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var allBindings = allBindingsAccessor(),
            unwrap = ko.utils.unwrapObservable,
            modelValue = unwrap(valueAccessor()) || '',
            valueProp = allBindings.jqAutoSourceValue,
            inputValueProp = allBindings.jqAutoSourceInputValue || valueProp;

        //if we are writing a different property to the input than we are writing to the model, then locate the object
        if (valueProp && inputValueProp !== valueProp) {
            var source = unwrap(allBindings.jqAutoSource) || [];
            modelValue = ko.utils.arrayFirst(source, function (item) {
                return unwrap(item[valueProp]) === unwrap(modelValue[valueProp]);
            }) || {};
        }

        //update the element with the value that should be shown in the input
        $(element).val(modelValue && inputValueProp !== valueProp ? unwrap(modelValue[inputValueProp]) : modelValue.toString());
    }
};

(
function () {
    addValidationCodeToCustomBinding("jqAuto");
}());