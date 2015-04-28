EDGE.prototype.ajaxPost = function (data, url, successCallback, errorCallback) {

    var payload = JSON.stringify(data);
    return $.ajax({
        url: url,
        type: 'POST',
        data: payload,
        dataType: 'JSON',
        contentType: 'application/json; charset=utf-8',
        success: successCallback,
        error: errorCallback
    });
};

EDGE.prototype.ajaxGet = function (data, url, successCallback, errorCallback) {

    if (data == null) {
        return $.ajax({
            url: url,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            success: successCallback,
            error: errorCallback
        });
    } else {
        var payload = JSON.stringify(data);

        return $.ajax({
            url: url,
            type: 'GET',
            data: payload,
            contentType: 'application/json; charset=utf-8',
            success: successCallback,
            error: errorCallback
        });
    }
};