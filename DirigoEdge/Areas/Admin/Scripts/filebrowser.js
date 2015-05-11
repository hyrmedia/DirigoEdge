/*
 *  FileBrowser - v2.0.1
 */

; (function ($, window, document, undefined) {

    var pluginName = "fileBrowser",
        defaults = {
            directory: null,
            isEditor: true,
            createModal: true,
            newModalId: null
        };

    function Plugin(element, options, callback) {

        var self = this;

        if (!callback && typeof options === 'function') {
            callback = options;
            options = null;
        }

        this.element = element;
        this.callback = callback;
        this.settings = $.extend({}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;
        this.initialized = false;
        this.clickedElem = this.element;

        // Create objects for Ajax calls so we can abort them later
        this.browserAjax = null;
        this.folderAjax = null;

        this.$el = this.settings.createModal ? this.createModal() : $(this.element);

        // Each modal needs a unique Dropzone ID
        this.dropzoneId = this.$el.attr('id') + '_dropzone';

        // If createModal is set to false, it's assumed that this
        // plugin was called by a click event (i.e. CKEditor button)
        if (this.settings.createModal) {
            $(this.element).on('click', function () {
                self.clickedElem = this;
                self.init();
                return false;
            });
        } else {
            self.init();
        }
    }

    Plugin.prototype.init = function () {
        if (this.initialized) {
            this.show();
        } else {
            common.showAjaxLoader(this.$el);
            this.initFileBrowserData(this);
        }

        this.initialized = true;
    };

    Plugin.prototype.show = function () {
        this.$el.modal();
        this.events();
    };

    Plugin.prototype.events = function () {
        var self = this;

        // Click on  a folder in the folder list
        // Load list of files in that folder
        self.$el.on('click', '.directory', function () {

            var directory = $(this).attr('href');

            // Abort any existing folder AJAX calls
            if (self.folderAjax) {
                self.folderAjax.abort();
            }

            directory = directory.substr(directory.lastIndexOf('/') + 1);

            // Unhighlight all folders
            $('li', '.folders').removeClass('active');

            self.loadDirectoryFiles(directory);

            return false;

        });

        // Click on the delete icon on a folder in the folder list
        // Show Delete/Cancel div
        self.$el.on('click', '.folders .js-delete', function () {

            $(this).closest('li').find('.delete-actions').addClass('is-active');

            return false;

        });

        // Click on the cancel button on a folder in the folder list
        // Hide Delete/Cancel div
        self.$el.on('click', '.folders .delete-cancel', function () {

            $(this).closest('li').find('.delete-actions').removeClass('is-active');

            return false;

        });

        // Click on the delete confirm button on a folder in the folder list
        // Delete the folder
        self.$el.on('click', '.folders .delete-confirm', function () {

            var $this = $(this),
                isActive = $this.closest('li').hasClass('active'),
                data = {
                    folder: $this.closest('li').data('directory')
                };

            $.ajax({
                url: "/admin/media/deletefolder/",
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data, null, 2),
                success: function (res) {

                    if (res && res.success) {
                        $this.closest('li').fadeOut();
                        if (isActive) {
                            self.$el.find('.folders li').first().find('.directory').trigger('click');
                        }
                    } else {
                        noty({ text: res.error, type: 'error', timeout: 3000 });
                    }

                }
            });

            return false;

        });

        // Click Add Folder in the folder list
        // Show add folder dialog
        self.$el.on('click', '.add-folder-toggle', function () {

            var $this = $(this),
                $parent = $this.parent('.add-folder'),
                isActive = $parent.hasClass('is-active'),
                text = isActive ? 'Add Folder' : 'Cancel';

            $this.find('span').html(text);

            $parent.toggleClass('is-active');

            return false;

        });

        // Click on the create confirm button on a folder in the folder list
        // Add the folder
        self.$el.on('click', '.add-folder .create-confirm', function () {

            var $newEl,
                $this = $(this),
                data = {
                    folder: $this.siblings('.create-name').first().val()
                };

            $.ajax({
                url: "/admin/media/addfolder/",
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data, null, 2),
                success: function (res) {

                    if (res && res.success) {
                        $this.siblings('.create-name').first().val('');
                        $this.closest('.add-folder').find('.add-folder-toggle').trigger('click');

                        $newEl = self.$el
                            .find('.folders li')
                            .first()
                            .clone();

                        $newEl.appendTo(self.$el.find('.folders'));

                        $newEl.removeClass('hidden');
                        $newEl.removeClass('clone-directory');
                        $newEl.addClass('active');
                        $newEl.attr('data-directory', data.folder);
                        $newEl.find('.directory').attr('href', '/uploaded/' + data.folder);
                        $newEl.find('.folder-name').text(data.folder);
                        $newEl.find('.folder-count').text('0');

                        // Unhighlight all folders
                        $('li', '.folders').removeClass('active');

                        self.loadDirectoryFiles(data.folder);

                    } else {
                        noty({ text: res.error, type: 'error', timeout: 3000 });
                    }

                }
            });

            return false;

        });

        // Click on a file in the file list
        // Show thumbnail and settings, collapse for other files
        self.$el.on('click', '.files .files__list .file', function () {

            self.resetFileList();

            // Reveal thumbnail and settings toolbar
            $(this)
                .closest('li')
                .addClass('active')
                .find('.settings')
                .addClass('active');

            return false;

        });

        // Click on Insert on a file
        // Generate file object, hide modal, execute callback
        self.$el.on('click', '.insert', function () {

            var fileObject = {},
                $file = $(this).closest('.file-container'),
                isImage = $file.data('icon') === 'picture-o',
                media = {
                    path: $file.data('path'),
                    alt: $file.find('.alt-text').val(),
                    align: $('input[name=align]:checked', $file).val(),
                    linkText: $file.find('.link-text').val(),
                    width: $file.data('width'),
                    height: $file.data('height')
                };

            // Warn if user tries to insert a document without link text
            if (self.settings.isEditor && !isImage && !media.linkText) {
                alert('You didn\'t enter a link text');
                $file.find('.link-text').closest('label').addClass('input-error');
                return false;
            }

            // Create object to pass to callback
            if (isImage) {
                fileObject = {
                    type: 'image',
                    src: media.path,
                    alt: media.alt,
                    align: media.align,
                    width: media.width,
                    height: media.height,
                    elem: self.clickedElem,
                    // todo: This should be based on a checkbox
                    responsive: true
                };
            } else {
                fileObject = {
                    type: 'document',
                    href: media.path,
                    title: media.alt,
                    text: media.linkText,
                    elem: self.clickedElem
                };
            }

            // Unbind events, close modal
            self.terminateModal();

            if (self.callback && typeof self.callback === 'function') {
                self.callback(fileObject);
            }

        });

        // Click on Insert on a file
        // Generate file object, hide modal, execute callback
        self.$el.on('click', '.file-delete', function () {

            var $this = $(this),
                $container = $this.closest('.file-container'),
                data = {
                    filename: $container.data('path').replace('/uploaded/', '')
                };

            $.ajax({
                url: "/admin/media/removefile/",
                type: "POST",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(data, null, 2),
                success: function (res) {

                    if (res && res.success) {

                        $container.fadeOut(function () {
                            $container.remove();
                        });

                    } else {

                        noty({ text: res.response, type: 'error', timeout: 3000 });

                    }

                }
            });

            return false;

        });

        // Click on the close button
        // Unbind all events, close the modal
        self.$el.on('click', '.close-modal', function () {
            self.terminateModal();
        });

    };

    Plugin.prototype.createModal = function () {

        return $("<div/>", {
            "id": this.settings.newModalId || "Modal_" + Math.random().toString(36).substring(2, 12),
            "class": "file-browser modal"
        }).appendTo("body");

    };

    Plugin.prototype.initFileBrowserData = function () {

        var self = this;

        self.browserAjax = $.ajax({
            url: "/admin/media/filebrowser/",
            type: "GET",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (res) {
                if (res && res.success) {
                    self.showFileBrowser(res);
                }
            }
        });

    };

    Plugin.prototype.showFileBrowser = function (data) {

        this.$el
            .html(data.html);
        common.hideAjaxLoader(this.$el);

        if (this.settings.directory) {
            this.loadDirectoryFiles(this.settings.directory);
        } else {
            this.loadDirectoryFiles($('li', '.folders').eq(1).data('directory'));
        }

        this.show();

    };

    Plugin.prototype.loadDirectoryFiles = function (dir) {

        var self = this,
            $container = $(".file-browser.open > div.browser");

        if (!dir) return false;

        common.showAjaxLoader($container);
        self.folderAjax = $.ajax({
            url: "/admin/media/filebrowser/" + dir,
            type: "GET",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (res) {

                var $html, $noFiles;

                if (res && res.html) {
                    // Filebrowser partial
                    $html = $(res.html);
                    self.$files = $html.find('.file-container');
                    $noFiles = $html.find('.files');

                    $('.files', self.$el).html($noFiles.html());
                    
                    if (self.settings.isEditor) {
                        $('.files__list .file .insert').hide();
                    } else {
                        $('.files__list .settings .insert').hide();
                    }                    

                    // Add generated Dropzone ID
                    $('.files', self.$el).attr('id', self.dropzoneId);

                    // Reveal the list of files
                    $('.files__list', self.$el).addClass('active');

                    // Unhighlight all folders
                    $('li', '.folders').removeClass('active');

                    $('li[data-directory="' + dir + '"]', '.folders').addClass('active');

                    if (self.fileDropzone) {
                        self.fileDropzone.options.params.category = dir;
                    } else {
                        self.fileDropzone = new Dropzone('#' + self.dropzoneId, {
                            clickable : '.toolbar',
                            url : "/admin/media/uploadfile/",
                            params : {
                                category : dir
                            },
                            accept : function (file, done) {

                                if (file.size > 20971520) {
                                    done('Filesize cannot exceed 20mb');
                                } else {
                                    done();
                                }

                            },
                            init : function () {
                                this.on("success", function (file, data) {

                                    if (data && data.success) {
                                        self.loadDirectoryFiles(this.options.params.category);
                                    } else {
                                        noty({ text : data.error, type : 'error', timeout : 3000 });
                                    }
                                });

                                this.on("error", function (file, err, xhr) {

                                    if (file.size > 20971520) {
                                        noty({ text : 'Filesize cannot exceed 20mb', type : 'error', timeout : 3000 });
                                    } else {
                                        noty({ text : 'Something has gone wrong.', type : 'error', timeout : 3000 });
                                    }

                                });

                                this.on("complete", function(file) {

                                    this.removeFile(file);

                                });
                            }
                        });
                    }
                }
                common.hideAjaxLoader($container);
            }
        });

    };    

    Plugin.prototype.terminateModal = function () {

        this.$el.off('click');
        this.$el.modal('hide');

    };

    Plugin.prototype.resetFileList = function () {

        // Hide thumbnails and settings toolbar for
        // previously clicked files
        $('.settings', '.files__list').removeClass('active');
        $('li', '.files__list').removeClass('active');

        // Reset radio buttons
        $('input[name=align]').prop('checked', false);

        // Remove any error classes
        $('.input-error').removeClass('input-error');

    };

    $.fn[pluginName] = function (options, callback) {

        // This allows the plugin to reuse modals once they've
        // been instantiated. Mostly for CKEditor.
        if (!$.data(this, "plugin_" + pluginName)) {
            $.data(this, "plugin_" + pluginName, new Plugin(this, options, callback));
        } else {
            $.data(this, "plugin_" + pluginName).show();
        }

        return this;
    };

})(jQuery, window, document);