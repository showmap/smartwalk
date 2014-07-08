
// #########    V a l i d a t i o n    R u l e s     ################

ko.validation.rules["dependencies"] = {
    validator: function (val, dependencies) {
        if (!dependencies) return true;
        ko.utils.arrayForEach(dependencies, function (dependency) {
            if (dependency.isValid) {
                //ko.validation.validateObservable.call(dependency, dependency);
                dependency.notifySubscribers();
            }
        });

        return true;
    },
    message: 'error.depencies'
};

ko.validation.rules["contactValidation"] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && !val)
            return true;

        switch (otherVal.contactType()) {
            case 0:
                this.message = otherVal.messages.contactEmailValidationMessage;
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(val);

            case 1:
                this.message = otherVal.messages.contactWebValidationMessage;
                var webRegex = new RegExp("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|(www\\.)?){1}([0-9A-Za-z-\\.@:%_\‌​+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?");
                return webRegex.test(val);

            case 2:
                this.message = otherVal.messages.contactPhoneValidationMessage;
                var phoneRegex = new RegExp("^[\s()+-]*([0-9][\s()+-]*){6,20}$");
                return phoneRegex.test(val);

            default:
                return false;
        }
    }
};

ko.validation.rules["dateCompareValidation"] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && (!val || !otherVal.compareVal()))
            return true;

        var dateFormat = $.datepicker.regional[""].dateFormat;

        var curDate = $.datepicker.parseDate(dateFormat, val);
        var cmpDate = $.datepicker.parseDate(dateFormat, otherVal.compareVal());

        if (otherVal.cmp == "GREATER_THAN") {
            return curDate >= cmpDate.setDate(cmpDate.getDate() - 1);
        } else if (otherVal.cmp == "LESS_THAN") {
            return curDate <= cmpDate.setDate(cmpDate.getDate() + 1);
        } else if (otherVal.cmp == "REGION") {
            if (!otherVal.compareValTo())
                return true;
            var cmpDateTo = $.datepicker.parseDate(dateFormat, otherVal.compareValTo());
            return curDate <= cmpDateTo.setDate(cmpDateTo.getDate() + 1) && curDate >= cmpDate.setDate(cmpDate.getDate() - 1);
        }

        return false;
    }
};

ko.validation.rules["urlValidation"] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && !val)
            return true;
        var regex = new RegExp("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|(www\\.)?){1}([0-9A-Za-z-\\.@:%_\‌​+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?");
        return regex.test(val);
    }
};

ko.validation.rules["asyncValidation"] = {
    async: true,
    validator: function (val, otherVal, callback) {
        var model = otherVal.model || otherVal.modelHandler();
        if (model) {
            model[otherVal.propName] = val;

            ajaxJsonRequest(
                { propName: otherVal.propName, model: model },
                otherVal.validationUrl,
                function (response, statusText, xhr) {
                    callback(true);
                },
                function (response, statusText, xhr) {
                    callback({ isValid: false, message: $.parseJSON(response.responseText).Message });
                }
            );
        }
    }
};

ko.validation.registerExtenders();

ko.validation.init({
    errorElementClass: "has-error",
    errorMessageClass: "help-block",
    decorateElement: true,
    messageOnModified: true
});

function addValidationCoreToCustomBinding(binding) {
    if (ko.bindingHandlers[binding]) {
        var init = ko.bindingHandlers[binding].init;
        ko.bindingHandlers[binding].init =
            function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
                init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
                return ko.bindingHandlers["validationCore"].init(
                    element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
            };
    }
};

addValidationCoreToCustomBinding("jqAuto");
addValidationCoreToCustomBinding("datepicker");
addValidationCoreToCustomBinding("timepicker");

// ###############    E d i t i n g    U t i l s     ####################

// static
function VmItemUtil() {
};

VmItemUtil.deleteItem = function (item) {
    item.state(VmItemState.Deleted);
};

VmItemUtil.availableItems = function (items) {
    return items
        ? $.grep(items, function (item) {
            return item.state() != VmItemState.Deleted &&
                item.state() != VmItemState.Hidden;
        })
        : undefined;
};

VmItemUtil.deletedItems = function (items) {
    return items
        ? $.grep(items, function (item) { return item.state() == VmItemState.Deleted; })
        : undefined;
};

// ##########    3 r d    P a r t y    Ov e r r i d e s    ##############

// restyle with bootstrap
$.widget("ui.autocomplete", $.ui.autocomplete,
    {
        options: {
            focus: function (event) {
                $(event.currentTarget)
                    .find(".ui-menu-item a")
                    .removeClass("ui-corner-all ui-state-focus");
            }
        },

        _renderMenu: function (ul, items) {
            var that = this;
            $.each(items, function (index, item) {
                that._renderItemData(ul, item);
            });
            $(ul)
                .addClass("dropdown-menu")
                .removeClass("ui-autocomplete ui-front ui-menu ui-widget ui-widget-content ui-corner-all");
        },
    });