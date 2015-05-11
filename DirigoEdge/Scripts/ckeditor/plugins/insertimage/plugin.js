CKEDITOR.plugins.add('insertimage', {
    icons: 'insertimage',
    init: function (editor) {

        editor.ui.addButton('InsertImage', {
            label: 'Insert Image',
            toolbar: 'insert',
            click: function (e) { }
        });

        function renderResponsiveTag(object, isPagebuilder) {

            if (!isPagebuilder) return '[responsive_image src="' + object.src + '" alt="' + object.alt + '" width="' + object.width + '" height="' + object.height + '"]';

            // This super hacky and prone to breaking if there are any backend changes.
            return '<div class="dynamicCodeInsert sortableModule responsive-image" data-name="responsive_image src=&quot;' + object.src + '&quot; alt=&quot' + object.alt + ';&quot; width=&quot;' + object.width + '&quot; height=&quot;' + object.height + '&quot;" data-tag="responsive_image src=&quot;' + object.src + '&quot; alt=&quot' + object.alt + ';&quot; width=&quot;' + object.width + '&quot; height=&quot;' + object.height + '&quot;"><style class="responsive-image">' +
                '#HUVSDINB { width: ' + object.width + 'px; max-width: 100%; background-size: cover; background-position: center; background-repeat: no-repeat }' +
                '#HUVSDINB:after { content: ""; display: block; padding-bottom: ' + (object.height / object.width * 100) + '%; }' +
                '#HUVSDINB { background-image: url("' + object.src + '"); }' +
                '@media (max-width: 2560px) { #HUVSDINB { background-image: url("/images/extreme' + object.src + '"); } }' +
                '@media (max-width: 1920px) { #HUVSDINB { background-image: url("/images/large' + object.src + '" } }' +
                '@media (max-width: 1024px) { #HUVSDINB { background-image: url("/images/medium' + object.src + '" } }' +
                '@media (max-width: 480px) { #HUVSDINB { background-image: url("/images/small' + object.src + '" } }</style>' +
                '<div id="HUVSDINB" class="responsive-image" data-bgimage="' + object.src + '" title=""><br></div><div class="dynamicModuleEditBar editBar editor-removable"><span class="title">responsive_image src="' + object.src + '" alt="' + object.alt + '" width="' + object.width + '" height="' + object.height + '"</span>  </div></div>';

        }

        // Detach the default click event on the Insert Image button
        // Click event will be replaced by Filebrowser events
        // Adding setTimeout because CKEditor is being a jerk and
        // button doesn't exist for a bit
        var buttonTimer = window.setTimeout(function () {
            var $button = $(".cke_button__insertimage");

            $button.off('click');

            $button.fileBrowser(function (object) {
                var tag;
                var responsiveShortcode = renderResponsiveTag(object, $('body').hasClass('pageBuilder'));

                if (object.type === 'image') {
                    tag = object.responsive
                            ? responsiveShortcode
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