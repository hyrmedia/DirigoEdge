CKEDITOR.plugins.add('insertimage', {
    icons: 'insertimage',
    init: function (editor) {

        editor.ui.addButton('InsertImage', {
            label: 'Insert Image',
            toolbar: 'insert',
            click: function (e) { }
        });

        // Detach the default click event on the Insert Image button
        // Click event will be replaced by Filebrowser events
        // Adding setTimeout because CKEditor is being a jerk and
        // button doesn't exist for a bit
        var buttonTimer = window.setTimeout(function () {
            var $button = $(".cke_button__insertimage");

            $button.off('click');

            $button.fileBrowser(function (object) {
                var tag;

                if (object.type === 'image') {
                    tag = object.responsive
                            ? '[responsive_image src="' + object.src + '" alt="' + object.alt + '" width="' + object.width + '" height="' + object.height + '"]'
                            : '<img src="' + object.src + '" alt="' + object.alt + '" />';
                } else {
                    tag = '<a href="' + object.href + '" title="' + object.title + '" >' + object.text + '</a>';
                }

                // Insert the tag into the editor
                editor.insertHtml(tag);
            });
        }, 200);

    }
});