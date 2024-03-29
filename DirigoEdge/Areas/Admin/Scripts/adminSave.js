﻿/// ===========================================================================================
/// Class allows a page's settings to be saved via AJAX based strictly on html5 data attributs
/// ===========================================================================================

save_class = function () {

};

save_class.prototype.saveAdminData = function ($this) {
    var saveUrl = $this.attr("data-url");
    var saveMessage = $this.attr("data-saveMessage") || "Save Successful.";
    var isListData = $this.data('list');
    var data = isListData ? this.getListData() : this.getData();
    $("#SaveIndicator").show();
    $.ajax({
        url: saveUrl,
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data, null, 2),
        success: function (data) {
            var noty_id = noty({ text: 'Changes saved successfully.', type: 'success', timeout: 3000 });
            $("#SaveIndicator").hide();
            $('form[data-list="true"] tbody tr.altered').removeClass('altered');
            $('.modal').modal('hide');
        },
        error: function (data) {
            var message = 'There was an error processing your request.';
            if (data && data.message);
            {
                message = $.parseJSON(data.responseText).message;
            }

            var noty_id = noty({ text: message, type: 'error', timeout: 3000 });
            $("#SaveIndicator").hide();
        }
    });
};

save_class.prototype.initPageEvents = function () {
    var self = this;

    $("a.savePageButton").click(function () {
        self.saveAdminData($(this));
        return false;
    });
};

save_class.prototype.getData = function () {
    var data = {
        entity: {

        }
    };

    // Text Fields
    $("input[type=text].saveField").each(function () {
        var field = $(this).attr("data-field");
        var value = $(this).val();

        data.entity[field] = value;
    });

    $("input[type=password].saveField").each(function () {
        var field = $(this).attr("data-field");
        var value = $(this).val();

        data.entity[field] = value;
    });


    $("input[type=number].saveField").each(function () {
        var field = $(this).attr("data-field");
        var value = $(this).val();

        data.entity[field] = value;
    });

    // Checkboxes (booleans)
    $("input[type=checkbox].saveField").each(function () {
        var field = $(this).attr("data-field");
        var value = $(this).is(":checked");

        data.entity[field] = value;
    });

    // Select Boxes (single value)
    $("select.saveField").each(function () {
        var field = $(this).attr("data-field");
        var value = $(this).find("option:selected").val();

        data.entity[field] = value;
    });

    // Textareas
    $("textarea.saveField").each(function () {
        var field = $(this).attr("data-field");
        var value = $(this).val();

        data.entity[field] = value;
    });

    // Radio Buttons
    $('input[type=radio].saveField').each(function () {
        if ($(this).is(':checked')) {
            var field = $(this).attr('name');
            var value = $(this).val();

            data.entity[field] = value;
        }
    });

    return data;
};

save_class.prototype.getListData = function () {
    var entities = {
        entity: {

        }
    };

    var data = [];

    $('form[data-list="true"] tbody tr.altered').each(function () {
        var $row = $(this);
        // Text Fields
        $row.find("input[type=text].saveField").each(function () {
            var field = $(this).attr("data-field");
            var value = $(this).val();

            entities.entity[field] = value;
        });

        // Checkboxes (booleans)
        $row.find("input[type=checkbox].saveField").each(function () {
            var field = $(this).attr("data-field");
            var value = $(this).is(":checked");

            entities.entity[field] = value;
        });

        // Select Boxes (single value)
        $row.find("select.saveField").each(function () {
            var field = $(this).attr("data-field");
            var value = $(this).find("option:selected").val();

            entities.entity[field] = value;
        });

        // Hidden Fields
        $row.find("input[type=hidden].saveField").each(function () {
            var field = $(this).attr("data-field");
            var value = $(this).val();

            entities.entity[field] = value;
        });
        data.push(entities.entity);
        entities.entity = {};
    });
    return data;
};

// Keep at the bottom
$(document).ready(function () {
    save = new save_class();
    save.initPageEvents();
});

(function () {
    var method;
    var noop = function () { };
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn', 'rainbow'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());

(function () {
    var log = console.log;

    console.rainbow = function (str) {
        var css = 'font-size:30px; text-shadow: -2px 0 black, 0 2px black, 2px 0 black, 0 -2px black; background: linear-gradient(to right, red, yellow, lime, aqua, blue, fuchsia); color: white; font-weight: bold; padding: 0 0.5em;';
        var args = Array.prototype.slice.call(arguments);
        args[0] = '%c' + args[0];
        args.splice(1, 0, css);
        return log.apply(console, args);
    };
})();