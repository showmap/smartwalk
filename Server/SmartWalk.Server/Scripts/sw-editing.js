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
    validator: function (val, params) {
        if (params.allowEmpty && !val) return true;

        switch (params.contactType()) {
            case 0:
                this.message = params.messages.contactEmailValidationMessage;
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(val);

            case 1:
                this.message = params.messages.contactWebValidationMessage;
                var webRegex = new RegExp("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|(www\\.)?){1}([0-9A-Za-z-\\.@:%_\‌​+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?");
                return webRegex.test(val);

            case 2:
                this.message = params.messages.contactPhoneValidationMessage;
                var phoneRegex = new RegExp("^[\s()+-]*([0-9][\s()+-]*){6,20}$");
                return phoneRegex.test(val);

            default:
                return false;
        }
    }
};

ko.validation.rules["dateCompareValidation"] = {
    validator: function (val, params) {
        var cmpDate = params.compareVal();
        if (params.allowEmpty && !val) return true;

        if (params.cmp == "GREATER_THAN") {
            return cmpDate ? cmpDate <= val : true;
        } else if (params.cmp == "LESS_THAN") {
            return cmpDate ? val <= cmpDate : true;
        } else if (params.cmp == "REGION") {
            var cmpDateTo = params.compareValTo();
            return (cmpDate ? cmpDate <= val : true) &&
                (cmpDateTo ? val <= cmpDateTo : true);
        }

        return false;
    }
};

ko.validation.rules["urlValidation"] = {
    validator: function (val, params) {
        if (params.allowEmpty && !val) return true;
        var regex = new RegExp("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|(www\\.)?){1}([0-9A-Za-z-\\.@:%_\‌​+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?");
        return regex.test(val);
    }
};

ko.validation.rules["asyncValidation"] = {
    async: true,
    validator: function (val, params, callback) {
        var model = params.model || params.modelHandler();
        if (model) {
            model[params.propName] = val;

            sw.ajaxJsonRequest(
                { propName: params.propName, model: model },
                params.validationUrl,
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
    ///     beforeEdit: function(item) A handler to run some logic before an item is edited.
    ///     beforeSave: function(item) A handler to run some logic before an item is saved.
    ///     itemView: A string id of the item view template.
    ///     itemEditView: A string id of the item edit template.
    ///     filterItem: function(item) A handler to filter items array.
    ///   }
    ///   </param>
    /// </signature>
    
    var self = this;

    // private

    self._allItems = allItems;
    self._previousItemData = new Hashtable();
    self._editingItems = new HashSet();
    
    self._initItem = function (item) {
        item.isEditing = ko.observable(false);
        
        if (settings.initItem) {
            settings.initItem(item);
        }
    };

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
    
    self.getItemView = function (item) {
        return item.isEditing() ? settings.itemEditView : settings.itemView;
    };

    self.addItem = function () {
        var item = createItemHandler();
        
        if (!self._allItems()) {
            self._allItems([]);
        }

        self._allItems.push(item);
        self._editItem(item, false);
    };

    self._editItem = function (item, savePreviousData) {
        if (self._allItems.indexOf(item) < 0 ||
            self._editingItems.contains(item)) return;

        self.cancelAll();

        if (settings.beforeEdit) {
            settings.beforeEdit(item);
        }

        self._editingItems.add(item);

        if (savePreviousData) {
            self._previousItemData.put(item, item.toJSON.apply(item));
        }

        item.isEditing(true);
    };

    self.editItem = function (item) {
        self._editItem(item, true);
    };

    self.deleteItem = function (item) {
        if (item.isEditing()) {
            cancelItem(item);
        }

        self._allItems.destroy(item);
    };

    self.cancelItem = function (item) {
        if (self._allItems.indexOf(item) < 0 ||
            !self._editingItems.contains(item)) return;

        if (self._previousItemData.containsKey(item)) {
            item.loadData.apply(item, [self._previousItemData.get(item)]);
            self._previousItemData.remove(item);
        } else {
            self._allItems.remove(item);
        }

        self._editingItems.remove(item);

        if (item.errors) {
            item.errors.showAllMessages(false);
        }

        item.isEditing(false);
    };

    self.cancelAll = function() {
        self._editingItems.values().forEach(function (editingItem) {
            self.cancelItem(editingItem);
        });
    };

    self.saveItem = function (item) {
        if (self._allItems.indexOf(item) < 0 ||
            !self._editingItems.contains(item)) return false;

        if (settings.beforeSave) {
            settings.beforeSave(item);
        }
        
        if (item.isValidating && item.isValidating()) {
            setTimeout(function () { self.saveItem(item); }, 50);
            return false;
        }

        if (!item.errors || item.errors().length == 0) {
            self._previousItemData.remove(item);
            self._editingItems.remove(item);
        } else {
            item.errors.showAllMessages();
        }

        item.isEditing(false);

        return true;
    };
};

// ##########    3 r d    P a r t y    Ov e r r i d e s    ##############

sw.widgets = {};

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
                    .addClass("stop-scrolling");
            },
            beforeClose: function () {
                $("body")
                    .removeClass("stop-scrolling");
            }
        }
    });