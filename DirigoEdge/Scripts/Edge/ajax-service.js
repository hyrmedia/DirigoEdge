EDGE.prototype.ajaxPost = function (data, url, success, error) {

    if (data && !url && typeof data === 'object') {
        url = data.url;
        success = data.success;
        error = data.error;
        data = data.data;
    }

    return $.ajax({
        url: url,
        type: 'POST',
        data: data ? JSON.stringify(data, null, 2) : null,
        dataType: 'JSON',
        contentType: 'application/json; charset=utf-8',
        success: success,
        error: error
    });
};

EDGE.prototype.ajaxGet = function (data, url, success, error) {

    if (data && !url && typeof data === 'object') {
        url = data.url;
        success = data.success;
        error = data.error;
        data = data.data;
    }

    return $.ajax({
        url: url,
        type: 'GET',
        data: data,
        contentType: 'application/json; charset=utf-8',
        success: success,
        error: error
    });
};
