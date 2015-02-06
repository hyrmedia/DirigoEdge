CKEDITOR.plugins.add('sourceswitch', {
    //icons: 'insertimage',
    init: function (editor) {

        // Detach the default click event on the Source button
        // Click event will be replaced by a link to the code editor
        var buttonTimer = window.setTimeout(function () {
            var $button = $(".cke_button__source");

            // only do this if the editcontent page exists, otherwise keep default behavior
            if ($('.editContent').length) {
                $button.removeAttr('onclick');
                $button.on('click', function (e) {
                    window.open('/admin/pages/editcontent/' + $('.editContent').attr('data-id'), '_blank');
                });
            }
        }, 200);

    }
});