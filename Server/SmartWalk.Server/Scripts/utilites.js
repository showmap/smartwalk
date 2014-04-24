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

function ajaxJsonRequest(data, url, onSuccess, onError) {
    var config = {
        async: true,
        url: url,
        type: "POST",
        data: data,
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        error: function (e) {
            onError(e);
        },
        success: function (data) {
            onSuccess(data);
        }
    };

    $.ajax(config);
}

ListViewModel = function (parameters, url) {
    this.parameters_ = parameters;
    this.url_ = url;

    attachVerticalScroll.call(this, this.getNextPage);
};

ListViewModel.prototype = {
    Items: ko.observableArray(),
    currentPage: ko.observable(0),
    addItem: function(data) {
    },
    getData: function(pageNumber) {
        if (this.currentPage() != pageNumber) {
            var z = this;
            var ajData = JSON.stringify({ pageNumber: pageNumber, parameters: this.parameters_ });
            ajaxJsonRequest(ajData, this.url_, function (data) {
                if (data.length > 0) {
                    z.currentPage(z.currentPage() + 1);
                    for (var i = 0; i < data.length; i++) {
                        z.addItem(data[i]);
                    }
                }
            });
        }
    },
    getNextPage: function () {
        return this.getData(this.currentPage() + 1);
    },
};