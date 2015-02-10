media_class = function () {

    this.el = '.manageMedia';
    this.$el = $(this.el);

};

media_class.prototype.initPageEvents = function() {
    if (this.$el.length > 0) {
        this.initPageEvents();
        this.initDropzone();
    }
};

media_class.prototype.initPageEvents = function () {

    var self = this;

    $('#toggle-thumbnails', this.el).on('change', this.methods.toggleThumbnails).change();

    // Events to trigger add folder modal
    // and call method to send to server
    $('.create-media-folder').click(function () {
        $("#AddFolderModal").modal();
        $('input, #AddFolderModal').focus();
        return false;
    });

    $('input', '#AddFolderModal').on('keyup', function (e) {
        var key = e.keyCode || e.which,
            $this = $(this);

        if (key === 13 && $this.val().indexOf('+') === -1) {
            self.methods.addMediaFolder();
        } else {
            if ($this.val().indexOf('+') > -1) {
                $this
                    .parent('.form-group')
                    .addClass('has-error')
                    .find('small.control-label')
                    .removeClass('hidden');
                $('#ConfirmFolderAdd').addClass('disabled');
            } else {
                $this
                    .parent('.form-group')
                    .removeClass('has-error')
                    .find('small.control-label')
                    .addClass('hidden');
                $('#ConfirmFolderAdd').removeClass('disabled');
            }
        }
    });

    $('#ConfirmFolderAdd').on('click', function () {
        self.addMediaFolder(this);
    });
    
    // Events to trigger delete folder
    // confirmation and call method to
    // send to server
    $('body').on('click', '.delete-media-folder', function () {
        var folder = $(this).closest('li, tr').attr('data-folder');
        $("#ConfirmFolderDelete").attr('data-folder', folder);
        $("#DeleteFolderModal").modal();
        return false;
    });
    
    $('#ConfirmFolderDelete').on('click', function () {
        self.deleteMediaFolder(this);
    });
    
    // Events to trigger delete file
    // confirmation and call method to
    // send to server
    self.$el.on('click', '.delete:not(.disabled)', function () {

        var file = $(this).attr('data-src'),
            $modal = $('#DeleteMediaModal');

        $modal.find('#ConfirmMediaDelete').attr('data-src', file);

        $modal.find('.file').text(file.split('/').pop());
        
        $modal.modal();

    });
    
    $('#ConfirmMediaDelete').on('click', function () {

        var $button = $(this),
            data = {
                filename : $button.attr("data-src")
            };

        var $container = $('#DeleteMediaModal .content');
        common.showAjaxLoader($container);
        $.ajax({
            url : "/admin/media/removefile/",
            type : "POST",
            dataType : 'json',
            contentType : 'application/json; charset=utf-8',
            data : JSON.stringify(data, null, 2),
            success : function (res) {
                if (res && res.success) {
                    noty({ text : 'File successfully deleted.', type : 'success', timeout : 3000 });

                    // If DataTables are missing, remove HTML from table
                    // otherwise use DataTables API
                    if (!window.oTable) {
                        $('.manageTable .delete[data-src="' + data.filename + '"]').closest('tr')[0].remove();
                    } else {
                        parentClass.methods.refreshTable();
                    }
                } else {
                    noty({ text : res.response, type : 'error', timeout : 3000 });
                }
                common.hideAjaxLoader($container);
                $('#DeleteMediaModal').modal('hide');
            }
        });

        return false;
    });

};


media_class.prototype.initZClip = function ($element) {
    return $element.zclip({
        path: '/scripts/jquery/plugins/zclip/ZeroClipboard.swf',
        copy: function () { return $(this).parent().find("input").val() },
        afterCopy: function () {
            $(this).parent().find("input").select();
        }
    });

};

media_class.prototype.addMediaFolder = function (el) {

    var _this      = this,
        $input     = $('#AddFolderModal').find('input').first(),
        folderName = $input.val(),
        data       = {
            folder : folderName
        };

    var $container = $('#AddFolderModal div.content');
    common.showAjaxLoader($container);
    $.ajax({
        url : "/admin/media/addfolder/",
        type : "POST",
        dataType : 'json',
        contentType : 'application/json; charset=utf-8',
        data : JSON.stringify(data, null, 2),
        success : function (res) {

            $input.val('');

            if (res && res.success) {
                _this.refreshTable();
                $('#AddFolderModal').modal('hide');
            } else {
                noty({ text : res.error, type : 'error', timeout : 3000 });
            }

            common.hideAjaxLoader($container);
        }
    });

    return false;

};

media_class.prototype.deleteMediaFolder = function (el) {

    var _this  = this,
        $this  = $(el),
        folder = $this.attr('data-folder'),
        data   = {
            folder : folder
        };

    $.ajax({
        url         : "/admin/media/deletefolder/",
        type        : "POST",
        dataType    : 'json',
        contentType : 'application/json; charset=utf-8',
        data        : JSON.stringify(data, null, 2),
        success     : function (res) {
                
            $('#DeleteFolderModal').modal('hide');

            if (res && res.success) {
                    
                // If user is in folder that just got deleted, redirect
                // them to the dashboard. Otherwise just remove
                // the folder.
                var rgx = new RegExp('^(?:/admin)\/managemedia\/(?:' + folder + '/?)$', 'ig');
                if (location.pathname.match(rgx)) {
                    window.location = '/admin';
                } else {
                    _this.refreshTable();
                    //$('.dropdown [data-folder="' + folder + '"], tr[data-folder="' + folder + '"]').remove();
                }

            } else {
                noty({ text: res.error, type: 'error', timeout: 3000 });
            }

        }
    });

    return false;
        
};

media_class.prototype.refreshTable = function() {
    //Refresh the inner content to show the new user
    var $container = $(".category-listing");

    common.showAjaxLoader($container);
    $container.load("/admin/media/managemedia/ .category-listing", function (data) {
        common.hideAjaxLoader($container);
        var zclip = undefined;
        if (!$.fn.dataTable.isDataTable('.manageTable')) {
            window.oTable = $("table.manageTable").dataTable({
                "iDisplayLength": 25,
                "fnDrawCallback": function () {
                    $('.zclip').remove();
                    media.initZClip($(".copy"));
                },
                "aoColumnDefs": [
                    {
                        "bSortable": false,
                        "aTargets": ["thumbnail", "actions", "location"]
                    }
                ],
                //"aaSorting": [[0, "asc"]]
            });
        }
            
        zclip = media.initZClip(oTable.$(".copy"));
    });
};

media_class.prototype.methods = {
    toggleThumbnails: function () {

        var _this,
            $image = $('.thumb .image');

        if ($(this).prop('checked')) {
            $image.each(function () {
                _this = $(this);
                _this
                    .attr('style', 'background-image:url("' + _this.attr('data-thumb') + '")');
            });
        } else {
            $image
                .attr('style', '');
        }

    }
};

// Keep at the bottom
$(document).ready(function () {
    media = new media_class();
    media.initPageEvents();
});
