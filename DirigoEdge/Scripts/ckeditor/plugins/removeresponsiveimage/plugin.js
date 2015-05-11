CKEDITOR.plugins.add('removeresponsiveimage', {
    init: function (editor) {

        editor.on('instanceReady', function () {
            this.container.on('click', function (event) {
                var $el = $(event.data.$.target);
                var $parent = $el.closest('.dynamicCodeInsert');

                $('.responsive-image').closest('.dynamicCodeInsert').removeClass('is-selected');

                if ($el.hasClass('responsive-image')) {
                    $parent.addClass('is-selected');
                }
            });
        });

        editor.on('key', function (event) {
            var $image;

            // If DELETE or BACKSPACE keys are pressed, remove image
            if (event.data.keyCode === 46 || event.data.keyCode === 8) {
                $image = $('.dynamicCodeInsert.is-selected');
                if ($image.length) {
                    $image.remove();
                }
            }
        });

        editor.on('blur', function () {
            $('.dynamicCodeInsert.is-selected').removeClass('is-selected');
        });

    }
});