ko.bindingHandlers.mappicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        $(element).data(ko.mapUtil.ACCESSOR_NAME, valueAccessor);
        var mapClass = ko.mapUtil.getClassByType(settings.type);

        mapClass.init(element, settings);

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            mapClass.dispose(element);
        });
    },
    //update the control when the view model changes
    update: function (element, valueAccessor, allBindingsAccessor) {
        var settings = allBindingsAccessor().settings || {};
        var value = ko.utils.unwrapObservable(valueAccessor());
        var mapClass = ko.mapUtil.getClassByType(settings.type);

        mapClass.setData(element, value);
    }
};

// ###############    M a p   U t i l   ####################

ko.mapUtil = {};
ko.mapUtil.ACCESSOR_NAME = "coordinatesVA";
ko.mapUtil.MAP_OBJECT = "mapPickerObject";

ko.mapUtil.getClassByType = function (type)
{
    return (!type || type == "leaflet") ? ko.leafLetMap : ko.googleMap;
};

ko.mapUtil.dispose = function (element, disposeHandler) {
    if (disposeHandler) disposeHandler();
    $(element).data(ko.mapUtil.MAP_OBJECT, null);
    $(element).data(ko.mapUtil.ACCESSOR_NAME, null);
};

// ###############    L e a f L e t   M a p s   ####################

ko.leafLetMap = {};

ko.leafLetMap.init = function (element, settings) {
    L.Icon.Default.imagePath = settings.imagePath;
    
    var center = $(element).data(ko.mapUtil.ACCESSOR_NAME)()();
    var map = L.map(element).setView(center, settings.zoomLevel);
    L.tileLayer("http://{s}.tile.osm.org/{z}/{x}/{y}.png", { detectRetina: true }).addTo(map);
    
    map.on("click", function (e) {
        $(element).data(ko.mapUtil.ACCESSOR_NAME)()(e.latlng);
    });

    $(element).data(ko.mapUtil.MAP_OBJECT, map);
};

ko.leafLetMap.dispose = function (element) {
    ko.mapUtil.dispose(element,
        function () {
            $(element).data(ko.mapUtil.MAP_OBJECT).remove();
        }
    );
};

ko.leafLetMap.setData = function (element, value) {
    $(element).data(ko.mapUtil.MAP_OBJECT).setView(value);
};

// ###############    G o o g l e   M a p s   ####################

ko.googleMap = {};

ko.addMarker = function(latlng, title) {
    new google.maps.Marker({
        position: latlng,
        map: map,
        title: title,
        icon: "http://maps.google.com/mapfiles/marker.png" // TODO: to use our markers with two chars Abbreviation
    });
};

ko.googleMap.init = function (element, settings) {
    var center = $(element).data(ko.mapUtil.ACCESSOR_NAME)()();
    
    var map = new google.maps.Map(
            element,
            {
                backgroundColor: "#B4D1FF",
                zoom: settings.zoomLevel,
                center: center,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                streetViewControl: false
            });
    
    new google.maps.Marker({
        position: center,
        map: map,
        title: "Current address"
    });

    google.maps.event.addListener(map, 'click', function(e) {
        $(element).data(ko.mapUtil.ACCESSOR_NAME)()({ lat: e.latLng.lat(), lng: e.latLng.lng() });
    });
    
    $(element).data(ko.mapUtil.MAP_OBJECT, map);
};

ko.googleMap.dispose = function (element) {
    ko.mapUtil.dispose(element,
        function () {
            delete ($(element).data(ko.mapUtil.MAP_OBJECT));
        }
    );
};

ko.googleMap.setData = function (element, value) {
    $(element).data(ko.mapUtil.MAP_OBJECT).setCenter(value);
};