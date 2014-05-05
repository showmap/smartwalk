function inherits(child, parent) {
    var F = function () { };
    F.prototype = parent.prototype;
    child.prototype = new F();
    child.prototype.constructor = child;

    // `child` function is an object like all functions    
    child.superClass_ = parent.prototype;    
}

function addSingletonGetter(ctor) {
    ctor.getInstance = function () {
        if (!ctor.instance_) {
            ctor.instance_ = new ctor();
        }

        return ctor.instance_;
    };
}

function attachVerticalScroll(callback) {
    var z = this;
    $(window).scroll(function (evt) {
        evt.preventDefault();
        if ($(window).scrollTop() >= $(document).height() - $(window).height()) {
            callback.call(z);
        }
    });
}

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
        error: function (e) {
            if(onError)
                onError.call(z, e);
        },
        success: function (data) {
            if(onSuccess)
                onSuccess.call(z, data);
        }
    };

    $.ajax(config);
}

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