BookmarkClass = function () {

    this.el = '.manageBookmarks';
    this.$el = $(this.el);
    parentClass = this;

};

BookmarkClass.prototype.initPageEvents = function () {
    if (this.$el.length > 0) {
        this.initPageEvents();
    }
};

BookmarkClass.prototype.initPageEvents = function () {

    var self = this;

    // Events to trigger delete folder
    // confirmation and call method to
    // send to server
    $('body').on('click', '.delete-bookmark', function () {
        var bookmark = $(this).closest('li, tr').attr('data-bookmark');
        $("#ConfirmBookmarkDelete").attr('data-bookmark', bookmark);
        $("#DeleteBookmarkModal").modal();
        return false;
    });

    $('#ConfirmBookmarkDelete').on('click', this.methods.deleteBookmark);

    $('body').on('click', '.edit-bookmark', function () {
        var $modal = $("#EditBookmarkModal"),
            $row = $(this).closest('li, tr');

        $("#ConfirmBookmarkEdit").attr('data-bookmark', $row.attr('data-bookmark'));

        $modal.find('#bookmarkName').val($row.attr('data-title'));
        $modal.find('#bookmarkUrl').val($row.attr('data-url'));

        $("#EditBookmarkModal").modal();

        return false;
    });

    $('#ConfirmBookmarkEdit').on('click', this.methods.editBookmark);

};

BookmarkClass.prototype.methods = {

    editBookmark: function () {

        var $this = $(this),
            $modal = $this.closest('.modal'),
            data = {
                id: parseInt($this.attr('data-bookmark')),
                title: $modal.find('#bookmarkName').val(),
                url: $modal.find('#bookmarkUrl').val()
            };

        $.ajax({
            url : "/admin/bookmark/edit/",
            type : "POST",
            dataType : 'json',
            contentType : 'application/json; charset=utf-8',
            data : JSON.stringify(data, null, 2),
            success : function (res) {

                $modal.modal('hide');

                // Reset values to prevent bad things
                $this.attr('data-bookmark', '');
                $modal.find('#bookmarkName').val('');
                $modal.find('#bookmarkUrl').val('');

                if (res && res.response === 'success') {
                    parentClass.methods.refreshTable();
                    noty({ text: 'Bookmark successfully edited.', type: 'success', timeout: 3000 });
                } else {
                    noty({ text : res.error, type : 'error', timeout : 3000 });
                }

            }
        });

        return false;

    },

    deleteBookmark: function () {

        var $this = $(this),
            bookmark = $this.attr('data-bookmark'),
            data = {
                id: parseInt(bookmark)
            };

        $.ajax({
            url: "/admin/bookmark/delete/",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (res) {

                $('#DeleteBookmarkModal').modal('hide');

                if (res && res.response === "success") {
                    parentClass.methods.refreshTable();
                    noty({ text: 'Bookmark successfully deleted.', type: 'success', timeout: 3000 });
                } else {
                    noty({ text: res.error, type: 'error', timeout: 3000 });
                }

            }
        });

        return false;

    },

    refreshTable: function () {
        //Refresh the inner content to show the new user
        var $container = $(".manageTable");

        common.showAjaxLoader($container);
        $container.load("/admin/bookmark/ .manageTable", function (data) {
            common.hideAjaxLoader($container);
            if (!$.fn.dataTable.isDataTable('.manageTable')) {
                window.oTable = $("table.manageTable").dataTable({
                    "iDisplayLength": 25,
                    "aoColumnDefs": [
                        {
                            "bSortable": false,
                            "aTargets": ["thumbnail", "actions", "location"]
                        }
                    ],
                    "aaSorting": [[0, "asc"]]
                });
            }
        });
    }
};

// Keep at the bottom
$(document).ready(function () {
    var bookmarks = new BookmarkClass();
    bookmarks.initPageEvents();
});
