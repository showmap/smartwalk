(function ($) {

    $.foursquareAutocomplete = function (element, options) {
        this.options = {};

        element.data('foursquareAutocomplete', this);

        this.init = function (element, options) {
            this.options = $.extend({}, $.foursquareAutocomplete.defaultOptions, options);
            this.options = $.metadata ? $.extend({}, this.options, element.metadata()) : this.options;
            updateElement(element, this.options);
        };
        this.init(element, options);
        this.select = function (event, ui) {
        };
        
    };
    $.fn.foursquareAutocomplete = function (options) {
        return this.each(function () {
            (new $.foursquareAutocomplete($(this), options));
        });
    };

    function updateElement(element, options) {
        element.autocomplete({
            source: function (request, response) {
                var ajData = null;                
                if (typeof options.oauth_token === "undefined" || options.oauth_token === "") {                    
                    ajData = {
                        ll: options.latitude + "," + options.longitude,
                        v: "20140214",
                        client_id: options.client_id,
                        client_secret: options.client_secret,
                        query: request.term
                    };
                } else {
                    ajData = {
                        ll: options.latitude + "," + options.longitude,
                        v: "20140214",
                        oauth_token: options.oauth_token,
                        query: request.term
                    };
                }

                $.ajax({
                    url: "https://api.foursquare.com/v2/venues/search",
                    dataType: "jsonp",
                    data: ajData,
                    success: function (data) {
                    		// Check to see if there was success
                    		if (data.meta.code != 200) {
                    		    element.removeClass("ui-autocomplete-loading");
                    			options.onError(data.meta.code, data.meta.errorType, data.meta.errorDetail);
                    			return false;
                    		}
                        
                    		//$("#gLog").text(JSON.stringify(data.response));

                            var mapData = data.response.minivenues === undefined ? data.response.venues : data.response.minivenues;
                            $("#gLog").text("");
                            response($.map(mapData, function (item) {
                                $("#gLog").text(JSON.stringify(item));
                    		    //options.onError(0, 0, JSON.stringify(data.response));
                            return {                                
                                name: item.name,
                                id: item.id,
                                address: (item.location.address == undefined ? "" : item.location.address),
                                cityLine: (item.location.city == undefined ? "" : item.location.city + ", ") + (item.location.state == undefined ? "" : item.location.state + " ") + (item.location.postalCode == undefined ? "" : item.location.postalCode),
                                photo: (item.categories == undefined || item.categories.length == 0 ? "" : item.categories[0].icon.prefix + "bg_32" + item.categories[0].icon.suffix),
                                location: {lat:item.location.lat,lng:item.location.lng},
                                full: item
                            };
                            }));
                    },
                    error: function (header, status, errorThrown) {
                    	  options.onAjaxError(header, status, errorThrown);
                    }
                });
            },
            minLength: options.minLength,
            select: function (event, ui) {
                element.val(ui.item.name);
                options.search(event, ui);
                return false;
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        })
            .data("ui-autocomplete")._renderItem = function (ul, item) {
                return $("<li></li>")
                    .data("item.autocomplete", item)
                    .append("<a>" + getAutocompleteText(item) + "</a>")
                    .appendTo(ul);
            };

    };

    $.foursquareAutocomplete.defaultOptions = {
        'latitude': 47.22,
        'longitude': -122.2,
        'oauth_token': "",
        'minLength': 3,
        'select': function (event, ui) {},
        'onError': function (errorCode, errorType, errorDetail) {},
        'onAjaxError' : function (header, status, errorThrown) {}
    };
    

    /// Builds out the <select> portion of autocomplete control
    function getAutocompleteText(item) {
        var text = "<div>";
        text += "<div class='categoryIconContainer'><img src='" + (item.photo == "" ? "" : item.photo) + "' /></div>";
        text += "<div class='autocomplete-name'>" + item.name + "</div>";
        if (item.address == "" && item.cityLine == "")
            text += "<div class='autocomplete-detail'>&nbsp;</div>";
        if (item.address != "")
            text += "<div class='autocomplete-detail'>" + item.address + "</div>";
        if (item.cityLine != "")
            text += "<div class='autocomplete-detail'>" + item.cityLine + "</div>";
        text += "</div>";
        return text;
    }
})(jQuery);
