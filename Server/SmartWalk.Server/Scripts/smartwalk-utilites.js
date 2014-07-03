function inherits(child, parent) {
    var f = function () { };
    f.prototype = parent.prototype;
    child.prototype = new f();
    child.prototype.constructor = child;

    // `child` function is an object like all functions    
    child.superClass_ = parent.prototype;
};

function attachVerticalScroll(callback) {
    $(window).scroll(function (evt) {
        evt.preventDefault();

        if ($(window).scrollTop() >=
            $(document).height() - $(window).height()) {
            callback();
        }
    });
};

function ajaxJsonRequest(ajData, url, onSuccess, onError) {
    var self = this;

    var config = {
        async: true,
        url: url,
        type: "POST",
        data: JSON.stringify(ajData),
        dataType: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
    };

    $.ajax(config)
        .done(function(response, statusText, xhr) {
            if (onSuccess) onSuccess.call(self, response, statusText, xhr);
        })
        .fail(function (response, statusText, xhr) {
            if (onError) onError.call(self, response, statusText, xhr);
        });
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

VmItemState =
{
    Normal: 0,
    Added: 1,
    Deleted: 2,
    Hidden: 3
};

// static
function VmItemUtil() {
};

VmItemUtil.DeleteItem = function (item) {
    item.state(VmItemState.Deleted);
};

VmItemUtil.AvailableItems = function (items) {
    return items
        ? $.grep(items, function (item) {
                return item.state() != VmItemState.Deleted &&
                    item.state() != VmItemState.Hidden;
            })
        : undefined;
};

VmItemUtil.DeletedItems = function (items) {
    return items
        ? $.grep(items, function (item) { return item.state() == VmItemState.Deleted; })
        : undefined;
};