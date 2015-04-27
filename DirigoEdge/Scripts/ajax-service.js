function AjaxService() {
    var ajax = this;

    ajax.Post = function (data, url, successCallback, errorCallback) {

        var payload = JSON.stringify(data);
        $.ajax({
            url: url,
            type: 'POST',
            data: payload,
            dataType: 'JSON',
            contentType: 'application/json; charset=utf-8',
            success: successCallback,
            error: errorCallback
        });
    };

    ajax.Get = function (data, url, successCallback, errorCallback) {
        var payload = JSON.stringify(data)

        $.ajax({
            url: url,
            type: 'GET',
            data: payload,
            contentType: 'application/json; charset=utf-8',
            success: successCallback,
            error: errorCallback
        });
    };

    return ajax;
}