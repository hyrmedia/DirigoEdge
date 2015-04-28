common_class = function() {

};

common_class.prototype.initPageEvents = function() {

    // Setup console.log if it does not exist
    if (typeof console == "undefined") {
        window.console = {
            log: function () { }
        };
    }

};

// Shows an ajax spinner over the specified element
// Ex: common.showAjaxLoader($("#myElement"));
common_class.prototype.showAjaxLoader = function($el) {
    var width = $el.width();
    var height = $el.height();

    var $html = $('<div class="ajaxContainer"><div class="ajaxLoader"></div></div>');
    $html.css({ "height": height, "width": width });

    // Element must be relative for the graphic to overlay properly
    // TODO: Check for position and maybe do a thing
    if ($el.css("position") === "static") {
        $el.css("position", "relative");
    }

    $el.append($html);
};

common_class.prototype.convertFromUtc = function(time, timeZone){
    moment.tz(time, "UTC").clone().tz(timeZone);
};

common_class.prototype.hideAjaxLoader = function($el) {
    $el.find(".ajaxContainer").remove();
};

common_class.prototype.insertParam = function(url, key, value) {
    return url + (url.indexOf("?") < 0 ? "?" : "&") + key + "=" + value;
};

common_class.prototype.executeFunctionByName = function(functionName, context /*, args */) {
    var args = Array.prototype.slice.call(arguments).splice(2);
    var namespaces = functionName.split(".");
    var func = namespaces.pop();
    for (var i = 0; i < namespaces.length; i++) {
        context = context[namespaces[i]];
    }
    return context[func].apply(this, args);
};

common_class.prototype.isValidPassword = function (str) {
    /*
            (?=.*[A-Z].*[A-Z])        Ensure string has at least one uppercase letter.
            (?=.*[!@#$&*])            Ensure string has at least one special case letter.
            (?=.*[0-9])        Ensure string has at least one digit.
            (?=.*[a-z]) Ensure string has at least one lowercase letter.
            .{8,}                     Ensure string is at least of length 8.
        */
    return /(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{12,}/.test(str);
};

/**
 * http://stackoverflow.com/a/10997390/11236
 */
common_class.prototype.updateURLParameter = function(url, param, paramVal) {
    var newAdditionalURL = "";
    var tempArray = url.split("?");
    var baseURL = tempArray[0];
    var additionalURL = tempArray[1];
    var temp = "";
    if (additionalURL) {
        tempArray = additionalURL.split("&");
        for (i = 0; i < tempArray.length; i++) {
            if (tempArray[i].split('=')[0] != param) {
                newAdditionalURL += temp + tempArray[i];
                temp = "&";
            }
        }
    }

    var rows_txt = temp + "" + param + "=" + paramVal;
    return baseURL + "?" + newAdditionalURL + rows_txt;
};

// Keep at the bottom
$(document).ready(function () {
    common = new common_class();
    common.initPageEvents();
    $('[timezone][date][format]').each(function () {
        var self = $(this);
        var timezone = self.attr('timezone');
        var publishdate = self.attr('date');
        var format = self.attr('format');
        var time = common.convertFromUtc(publishdate, timezone);
        var formatTime = time.format(format);
        self.text(formatTime);
    });
    EDGE = new EDGE();
});