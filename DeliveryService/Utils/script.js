var map;
var courierMarker = null;
var routes = [];
var currentRoute = null;
var routeRequestId = 0;


ymaps.ready(function () {
    map = new ymaps.Map("map", {
        center: [55.0415, 82.9346],
        zoom: 12
    });

    map.events.add('click', function (e) {
        var coords = e.get('coords');

        ymaps.geocode(coords).then(function (res) {
            var firstGeoObject = res.geoObjects.get(0);
            var address = firstGeoObject.getAddressLine();

            window.chrome.webview.postMessage({
                type: "mapClick",
                lat: coords[0],
                lon: coords[1],
                address: address
            });
        });
    });

});
//Очистка карты
function clearMapOverlays() {
    map.geoObjects.removeAll();
}
//Постройка маршрута
function DrawRoute(startLat, startLon, endLat, endLon, addCourierMark) {
    console.log("Draw Route",startLat,endLon);
    clearMapOverlays();
    routeRequestId++;
    var thisRequestId = routeRequestId;
    if (courierMarker != null) {
        map.geoObjects.remove(courierMarker);
        courierMarker = null;
    }
    ymaps.route([
        [startLat, startLon],
        [endLat, endLon]
    ]).then(function (route) {
        if (thisRequestId !== routeRequestId) return;
        currentRoute = route;
        map.geoObjects.add(route);
        if (addCourierMark) {
            //AddMark(startLat, startLon);


            route.getPaths().each(function (path) {
                var coordinates = path.geometry.getCoordinates();
                window.chrome.webview.postMessage({
                    type: "routeCoordinates",
                    coordinates: coordinates
                });
            });
        }


    }).catch(function (err) {
        console.log("Ошибка:", err);
    });
}
//Добавление метки
function AddMark(lat, lon) {
    console.log("AddMark вызван, текущий courierMarker:", courierMarker);

    if (courierMarker != null) {
        map.geoObjects.remove(courierMarker);
        courierMarker = null;
    }
    courierMarker = new ymaps.Placemark(
        [lat, lon],
        {
            balloonContent: 'Курьер'
        },
        {
            iconLayout: 'default#imageWithContent',
            iconImageHref: 'https://cdn-icons-png.flaticon.com/512/684/684908.png',
            iconImageSize: [36, 36],
            iconImageOffset: [-18, -36],
            iconContentLayout: ymaps.templateLayoutFactory.createClass(
                '<div style="color:white;background:#2563EB;border-radius:50%;width:22px;height:22px;text-align:center;line-height:22px;font-weight:bold;border:2px solid white;">К</div>'
            ),
            zIndex: 9999,
            zIndexActive: 10000
        }
    );
    map.geoObjects.add(courierMarker);
}


function MoveCourier(lat, lon) {

    if (courierMarker != null) {
        courierMarker.geometry.setCoordinates([lat, lon]);
    }
}
