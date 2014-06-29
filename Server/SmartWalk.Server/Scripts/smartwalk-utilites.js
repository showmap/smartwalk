function addValidationCodeToCustomBinding(binding) {
    var init = ko.bindingHandlers[binding].init;
    ko.bindingHandlers[binding].init = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
        return ko.bindingHandlers['validationCore'].init(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
    };
};

function inherits(child, parent) {
    var F = function () { };
    F.prototype = parent.prototype;
    child.prototype = new F();
    child.prototype.constructor = child;

    // `child` function is an object like all functions    
    child.superClass_ = parent.prototype;
};

function addSingletonGetter(ctor) {
    ctor.getInstance = function () {
        if (!ctor.instance_) {
            ctor.instance_ = new ctor();
        }

        return ctor.instance_;
    };
};

function attachVerticalScroll(callback) {
    var z = this;
    $(window).scroll(function (evt) {
        evt.preventDefault();
        if ($(window).scrollTop() >= $(document).height() - $(window).height()) {
            callback.call(z);
        }
    });
};

function ajaxJsonRequest(ajData, url, onSuccess, onError) {
    var z = this;

    var config = {
        async: true,
        url: url,
        type: "POST",
        data: ajData,
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
    };

    $.ajax(config)
        .done(function(response, statusText, xhr) {
            onSuccess.call(z, response, statusText, xhr);
        })
        .fail(function (response, statusText, xhr) {
            onError.call(z, response, statusText, xhr);
        });
};

//ListViewModel class
ListViewModel = function (parameters, url) {
    this.parameters_ = parameters;
    this.url_ = url;
    this.query = ko.observable();

    attachVerticalScroll.call(this, this.getNextPage);
};

ListViewModel.prototype.Items = ko.observableArray();
ListViewModel.prototype.currentPage = ko.observable(0);
ListViewModel.prototype.getData = function(pageNumber) {
    if (this.currentPage() != pageNumber) {
        var ajData = JSON.stringify({ pageNumber: pageNumber, query: this.query(), parameters: this.parameters_ });
        ajaxJsonRequest.call(this, ajData, this.url_,
            function (data) {
                if (data.length > 0) {
                    this.currentPage(this.currentPage() + 1);
                    for (var i = 0; i < data.length; i++) {
                        this.addItem(data[i]);
                    }
                }
            }
        );
    }
};
ListViewModel.prototype.getNextPage = function() {
    return this.getData(this.currentPage() + 1);
};
ListViewModel.prototype.search = function (data) {
    $("a").remove(".default-rows");
    this.Items.removeAll();
    this.currentPage(-1);
    this.getNextPage();
};

//ViewModelBase Class
ViewModelBase = function() {
    this.selectedItem = ko.observable();
};

ViewModelBase.prototype.DeleteItem_ = function(item) {
    item.State(2);
};

ViewModelBase.prototype.Items_ = function(itemCollection) {
    return ko.utils.arrayFilter(itemCollection, function(item) {
        return item.State() != 2 && item.State() != 3;
    });
};

ViewModelBase.prototype.DeletedItems_ = function(itemCollection) {
    return ko.utils.arrayFilter(itemCollection, function(item) {
        return item.State() == 2;
    });
};

ViewModelBase.prototype.CheckedItems_ = function (itemCollection) {
    var selectedItem = this.selectedItem();
    return ko.utils.arrayFilter(itemCollection, function(item) {
        return item.IsChecked() && item != selectedItem;
    });
};

ViewModelBase.prototype.IsAnyItemSelected_ = function(itemCollection) {
    if (itemCollection.length == 0)
        return false;

    for (var i = 0; i < itemCollection.length; i++) {
        if (itemCollection[i].IsChecked()) {
            return true;
        }
    }
    return false;
};

ViewModelBase.prototype.GetAllItemsChecked_ = function(itemCollection) {
    if (itemCollection.length == 0)
        return false;

    for (var i = 0; i < itemCollection.length; i++) {
        if (!itemCollection[i].IsChecked()) {
            return false;
        }
    }
    return true;
};

ViewModelBase.prototype.SetAllItemsChecked_ = function(itemsCollection, value) {
    for (var i = 0; i < itemsCollection.length; i++) {
        if (itemsCollection[i].IsChecked() != value) {
            itemsCollection[i].IsChecked(value);
        }
    }
};

ko.validation.rules['dependencies'] = {
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

ko.validation.rules['contactValidation'] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && !val)
            return true;
        
        var regex = "";
        switch (otherVal.contactType()) {
            case 0:
                this.message = otherVal.messages.contactEmailValidationMessage;
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(val);
            case 1:
                this.message = otherVal.messages.contactWebValidationMessage;
                regex = new RegExp("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|(www\\.)?){1}([0-9A-Za-z-\\.@:%_\‌​+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?");
                return regex.test(val);
            case 2:
                this.message = otherVal.messages.contactPhoneValidationMessage;
                regex = new RegExp("^[\s()+-]*([0-9][\s()+-]*){6,20}$");
                return regex.test(val);
            default:
                return false;
        }        
    }
};

ko.validation.rules['dateCompareValidation'] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && (!val || !otherVal.compareVal()))
            return true;
        
        var dateFormat = $.datepicker.regional[''].dateFormat;
        
        var curDate = $.datepicker.parseDate(dateFormat, val);
        var cmpDate = $.datepicker.parseDate(dateFormat, otherVal.compareVal()); 
        
        if (otherVal.cmp == 'GREATER_THAN') {
            return curDate >= cmpDate.setDate(cmpDate.getDate() - 1);
        } else if (otherVal.cmp == 'LESS_THAN') {
            return curDate <= cmpDate.setDate(cmpDate.getDate() + 1);
        } else if (otherVal.cmp == 'REGION') {
            if (!otherVal.compareValTo())
                return true;
            var cmpDateTo = $.datepicker.parseDate(dateFormat, otherVal.compareValTo());
            return curDate <= cmpDateTo.setDate(cmpDateTo.getDate() + 1) && curDate >= cmpDate.setDate(cmpDate.getDate() - 1);
        }

        return false;
    }
};



ko.validation.rules['urlValidation'] = {
    validator: function (val, otherVal) {
        if (otherVal.allowEmpty && !val)
            return true;
        var regex = new RegExp("^(http[s]?:\\/\\/(www\\.)?|ftp:\\/\\/(www\\.)?|(www\\.)?){1}([0-9A-Za-z-\\.@:%_\‌​+~#=]+)+((\\.[a-zA-Z]{2,3})+)(/(.)*)?(\\?(.)*)?");
        return regex.test(val);
    }
};

ko.validation.rules['asyncValidation'] = {
    async: true,
    validator: function (val, otherVal, callback) {
        otherVal.model[otherVal.propName] = val;
        var ajdata = ko.toJSON({ propName: otherVal.propName, model: otherVal.model });

        ajaxJsonRequest(ajdata, otherVal.validationUrl,
            function (response, statusText, xhr) {
                callback(true);
            },
            function (response, statusText, xhr) {
                callback({ isValid: false, message: $.parseJSON(response.responseText).Message });
            }
        );
    }
};

ko.bindingHandlers.fadeInVisible = {
    init: function (element, valueAccessor) {
        var duration = ko.utils.unwrapObservable(valueAccessor());
        $(element).hide().fadeIn(duration);
    }
};

ko.bindingHandlers.scrollVisible = {
    init: function (element, valueAccessor) {
        if (!$(element).visible(false, false, "vertical")) {
            var duration = ko.utils.unwrapObservable(valueAccessor());
            $("html, body").animate({
                scrollTop: $(element).offset().top - 80 // minus small top margin
            }, duration);
        }
    }
};

ko.validation.registerExtenders();

ko.validation.init({
    errorElementClass: 'has-error',
    errorMessageClass: 'help-block',
    decorateElement: true,
    messageOnModified: true
});