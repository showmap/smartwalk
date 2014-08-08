// #########    V a l i d a t i o n    R u l e s     ################

ko.validation.rules["dependencies"] = {
    validator: function (val, dependencies) {
        if (!dependencies) return true;
        ko.utils.arrayForEach(dependencies, function (dependency) {
            if (dependency.isValid) {
                dependency.notifySubscribers();
            }
        });

        return true;
    },
    message: "error.depencies"
};

ko.validation.rules["contactValidation"] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && !val) return true;

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
        var cmpDate = otherVal.compareVal();
        if (otherVal.allowEmpty && (!val || !cmpDate)) return true;

        if (otherVal.cmp == "GREATER_THAN") {
            return val >= cmpDate;
        } else if (otherVal.cmp == "LESS_THAN") {
            return val <= cmpDate;
        } else if (otherVal.cmp == "REGION") {
            var cmpDateTo = otherVal.compareValTo();
            if (!cmpDateTo) return true;
            return val <= cmpDateTo && val >= cmpDate;
        }

        return false;
    }
};

ko.validation.rules["urlValidation"] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && !val) return true;
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

            sw.ajaxJsonRequest(
                { propName: otherVal.propName, model: model },
                otherVal.validationUrl,
                function () {
                    callback(true);
                },
                function (response) {
                    if (response.responseJSON &&
                        response.responseJSON.ValidationErrors &&
                        response.responseJSON.ValidationErrors.length > 0) {
                        var message = $.map(response.responseJSON.ValidationErrors,
                            function(ve) {
                                return ve.Error;
                            }).join(" ");

                        callback({ isValid: false, message: message });
                    }

                    callback({ isValid: false });
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

// defines a simple foo binding to attach validation errors to an element
ko.bindingHandlers.validationTag = {
    init: function () {}
};

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

addValidationCoreToCustomBinding("validationTag");
addValidationCoreToCustomBinding("datepicker");
addValidationCoreToCustomBinding("timepicker");

// ###############    E d i t i n g    U t i l s     ####################

function VmItemsManager(allItems, createItemHandler, settings) {
    /// <signature>
    ///   <summary>The manager for CRUD operations with a list items.</summary>
    ///   <param name="allItems" type="Array">An array of all items to work with.
    ///     It's expected that each item has toJSON() and loadData() methods. The optional validation method
    ///     isValidating() and property errors() supported. 
    ///   </param>
    ///   <param name="createItemHandler" type="Function">A handler to create a new instance of an item.</param>
    ///   <param name="settings" type="PlainObject">A set of key/value pairs that configure the manager.
    ///   {
    ///     initItem: function(item) A handler to externally init an item state.
    ///     setEditingItem: function(item) A handler to override default logic of setting item's editing state.
    ///     beforeSave: function(item) A handler to run some logic before an item is saved.
    ///     afterSave: function(item) A handler to run some logic after an item was saved.
    ///     itemView: A string id of the item view template.
    ///     itemEditView: A string id of the item edit template.
    ///     filterItem: function(item) A handler to filter items array.
    ///   }
    ///   </param>
    /// </signature>
    
    var self = this;

    // private

    self._previousItemData = ko.observable(null);

    self._processIsEditingChange = function (item, isEditing) {
        if (isEditing) {
            if (item.id() && item.id() != 0) {
                self._previousItemData(item.toJSON());
            }
        } else {
            if (item.id() && item.id() != 0) {
                if (self._previousItemData() != null) {
                    item.loadData(self._previousItemData());
                }
            } else {
                self._allItems.remove(item);
            }

            self._previousItemData(null);
        }
    };
    
    self._initItem = function (item) {
        item.isEditing = ko.observable(false);
        item.isEditing.subscribe(function (isEditing) {
            self._processIsEditingChange(item, isEditing);
        });
        
        if (settings.initItem) {
            settings.initItem(item);
        }
    };

    self._allItems = allItems;
    
    if (self._allItems()) {
        self._allItems().forEach(function (item) {
            self._initItem(item);
        });
    }
    
    self._allItems.subscribe(function (items) {
        if (items) {
            items.forEach(function (item) {
                if (item.isEditing === undefined) {
                    self._initItem(item);
                }
            });
        }
    });
    
    // public
    
    self.items = settings.filterItem
        ? ko.computed(function() {
            return self._allItems() ? $.grep(self._allItems(), settings.filterItem) : undefined;
        })
        : allItems;
    
    self.setEditingItem = settings.setEditingItem || function (editingItem) {
        if (self._allItems()) {
            self._allItems().forEach(function (item) {
                item.isEditing(item == editingItem);
            });
        }
    };
    
    self.getItemView = function (item) {
        return item.isEditing() ? settings.itemEditView : settings.itemView;
    };

    self.addItem = function () {
        var item = createItemHandler();
        
        if (!self._allItems()) {
            self._allItems([]);
        }
        self._allItems.push(item);
        
        self.editItem(item);
    };

    self.editItem = function (item) {
        self.setEditingItem(item);
    };

    self.deleteItem = function (item) {
        if (item.isEditing()) {
            cancelItem(item);
        }

        if (item.id() && item.id() > 0) {
            self._allItems.destroy(item);
        } else {
            self._allItems.remove(item);
        }
    };

    self.cancelItem = function (item) {
        self.setEditingItem(null);
        
        if (item.errors) {
            item.errors.showAllMessages(false);
        }
    };

    self.saveItem = function (item) {
        if (settings.beforeSave) {
            settings.beforeSave(item);
        }
        
        if (item.isValidating && item.isValidating()) {
            setTimeout(function () { self.saveItem(item); }, 50);
            return false;
        }

        if (!item.errors || item.errors().length == 0) {
            if (!item.id() || item.id() == 0) {
                item.id(-1);
            }
            
            if (settings.afterSave) {
                settings.afterSave(item);
            }

            self._previousItemData(null);
            self.setEditingItem(null);
        } else {
            item.errors.showAllMessages();
        }

        return true;
    };
};

EditingViewModelBase = function () {
    var self = this;

    self.currentRequest = null;

    self.isBusy = ko.observable(false);
    self.isEnabled = ko.computed(function () { return !self.isBusy(); });

    self.isBusy.subscribe(function (isBusy) {
        if (!isBusy && self.currentRequest) {
            self.currentRequest.abort();
            self.currentRequest = undefined;
        }
    });
    
    self.serverValidErrors = ko.observableArray();
    self.serverError = ko.observable();

    self.handleServerError = function (errorResult) {
        self.serverValidErrors(
            errorResult.responseJSON ?
                errorResult.responseJSON.ValidationErrors
                : undefined);

        if (!errorResult.responseJSON) {
            if (errorResult.statusText) {
                if (errorResult.statusText.toLowerCase() != "abort") {
                    self.serverError(errorResult.statusText);
                }
            } else {
                self.serverError("There was an error. Please try again.");
            }
        }
    };

    self.resetServerErrors = function () {
        self.serverValidErrors(undefined);
        self.serverError(undefined);
    };
};

// ##########    3 r d    P a r t y    Ov e r r i d e s    ##############

// restyle with bootstrap
$.widget("ui.autocomplete", $.ui.autocomplete,
    {
        options: {
            create: function (event) {
                $(event.currentTarget)
                    .find(".ui-menu-item a")
                    .removeClass("ui-corner-all");
            }
        },

        _renderMenu: function (ul, items) {
            var self = this;
            
            $.each(items, function (index, item) {
                self._renderItemData(ul, item);
            });
            
            $(ul)
                .addClass("dropdown-menu")
                .removeClass("ui-menu ui-widget ui-widget-content ui-corner-all");
        },
    });

// restyle with bootstrap
$.widget("ui.dialog", $.ui.dialog,
    {
        options: {
            dialogClass: "panel panel-default",
            show: {
                effect: "fade",
                duration: 250
            },
            hide: {
                effect: "fade",
                duration: 100
            },
            create: function (event) {
                var dialog = $(event.target.parentElement);

                var title = dialog
                    .find(".ui-dialog-title");
                title
                    .replaceWith($("<h3></h3>")
                        .attr("id", title.attr("id"))
                        .addClass("panel-title")
                        .text(title.text()));
                
                dialog
                    .find(".ui-dialog-titlebar")
                    .removeClass("ui-widget-header ui-corner-all")
                    .addClass("panel-heading");
                
                dialog
                    .find(".ui-dialog-content")
                    .removeClass("ui-widget-content")
                    .addClass("panel-body");

                dialog
                    .find(".ui-dialog-titlebar-close")
                    .addClass("btn btn-default")
                    .append($("<span></span>").addClass("glyphicon glyphicon-remove"));

                dialog
                    .find(".ui-dialog-buttonpane")
                    .removeClass("ui-widget-content")
                    .addClass("panel-footer");

                dialog.removeClass("ui-widget-content ui-corner-all");
            },
            open: function () {
                $("body")
                    .addClass("stop-scrolling")
                    .bind("touchmove", function(e) { e.preventDefault(); }); // TODO: to allow scroll inside dialog
            },
            beforeClose: function () {
                $("body")
                    .removeClass("stop-scrolling")
                    .unbind("touchmove");
            }
        }
    });