ko.bindingHandlers.mapPicker = {
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

ko.mapUtil = {};
ko.mapUtil.ACCESSOR_NAME = "coordinatesVA";
ko.mapUtil.MAP_OBJECT = "mapPickerObject";

ko.mapUtil.getClassByType = function (type)
{
    return (!type || type == "leaflet") ? ko.leafLetMap : ko.googleMap;
};

ko.mapUtil.dispose = function (element, disposeHandler) {
    $(element).data(ko.mapUtil.MAP_OBJECT, null);
    if (disposeHandler)
        disposeHandler();
    $(element).data(ko.mapUtil.ACCESSOR_NAME, null);
};

ko.leafLetMap = {};

ko.leafLetMap.init = function (element, settings) {
    L.Icon.Default.imagePath = settings.imagePath;
    var center = $(element).data(ko.mapUtil.ACCESSOR_NAME)()();
    var map = L.map(element).setView(center, settings.zoomLevel);
    L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png').addTo(map);
    
    map.on('click', function (e) {
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