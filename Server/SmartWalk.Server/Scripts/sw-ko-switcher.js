ko.bindingHandlers.switcher = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().switcherOptions || {};
        var value = valueAccessor();
        if (options && options.values) {
            for (var i = 0; i < options.values.length; i++) {
                var opt = options.values[i];
                var btn = $("<button></button>")
                    .prop("disabled", opt.enable === false)
                    .addClass("btn " + ((i == 0) ? "btn-primary active" : "btn-default"))
                    .data("val", opt.val)
                    .text(opt.disp);
                
                $(element).append(btn);

                btn.click(function () {
                    if (value() != $(this).data("val")) {
                        $(element).find(" > button").each(function () {
                            $(this).toggleClass("btn-primary active");
                            $(this).toggleClass("btn-default");
                        });

                        value($(this).data("val"));
                    }
                });
            }
        }
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();

        $(element).find(" > button").each(function () {
            if ($(this).data("val") == value() && $(this).hasClass("btn-default")) {
                $(this).toggleClass("btn-primary active");
                $(this).toggleClass("btn-default");
            }

            if ($(this).data("val") != value() && $(this).hasClass("btn-primary active")) {
                $(this).toggleClass("btn-primary active");
                $(this).toggleClass("btn-default");
            }
        });
    }
};