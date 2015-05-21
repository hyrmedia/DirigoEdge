CKEDITOR.plugins.add('insertimage', {
    icons: 'insertimage',
    init: function (editor) {

        editor.ui.addButton('InsertImage', {
            label: 'Insert Image',
            toolbar: 'insert',
            click: function (e) { }
        });

        function renderResponsiveTag(object, isPagebuilder, cb) {

            var shortcodeString = 'responsive_image src=&quot;' + object.src + '&quot; alt=&quot' + object.alt + ';&quot; width=&quot;' + object.width + '&quot; height=&quot;' + object.height + '&quot;" data-tag="responsive_image src=&quot;' + object.src + '&quot; alt=&quot' + object.alt + ';&quot; width=&quot;' + object.width + '&quot; height=&quot;' + object.height + '&quot;';

            var dataObject = {
                imageObject : {
                    ClassName : object.align,
                    ImagePath : object.src,
                    AltText : object.alt,
                    Width : object.width,
                    Height : object.height
                }
            };

            cb = typeof cb === 'function' ? cb : function () { };

            if (!isPagebuilder) {
                cb('[responsive_image src="' + object.src + '" alt="' + object.alt + '" width="' + object.width + '" height="' + object.height + '"]');
            } else {
                EDGE.ajaxPost({
                    data: dataObject,
                    url: '/content/getresponsiveimage/',
                    success: function (data) {
                        console.log(data);
                        cb('<div class="dynamicCodeInsert sortableModule responsive-image" data-name="' + shortcodeString + '">' + data + '<div class="dynamicModuleEditBar editBar editor-removable"><span class="title">' + shortcodeString + '</span></div></div>');
                    }
                });
            }

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

                renderResponsiveTag(object, $('body').hasClass('pageBuilder'), function (responsiveShortcode) {
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
                
            });
        }, 200);

    }
});