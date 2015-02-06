redirect_class = function () {

};

redirect_class.prototype.initRedirectEvents = function () {
    this.initDeleteRedirectEvent();
};

redirect_class.prototype.initDeleteRedirectEvent = function () {
    var self = this;
    $("body").on('click', ".deleteRedirectButton", function () {

        self.manageRedirectId = $(this).attr("data-id");
        self.$manageRedirectRow = $(this).closest('tr')[0];
        self.oTable = $("#DataTables_Table_0").dataTable();
        self.rowIndex = self.oTable.fnGetPosition(self.$manageRedirectRow);

        $("#DeleteModal").modal();
    });

    // Confirm Delete Content
    $("#ConfirmRedirectDelete").click(function () {
        var id = self.manageRedirectId;
        $.ajax({
            url: "/admin/redirect/deleteredirect",
            type: "POST",
            data: {
                id: id
            },
            success: function () {
                var noty_id = noty({ text: 'Redirect Successfully Deleted.', type: 'success', timeout: 2000 });
                self.oTable.fnDeleteRow(self.rowIndex);
                $('#DeleteModal').modal('hide');
            },
            error: function (data) {
                $('#DeleteModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });

    $('#SourceInput').on('keyup', function (e) {
        var $val = $(this);
        var key = e.keyCode || e.which;

        if (key === 32) {
            $val.val($val.val().replace(' ', '-'));
        }

        if ($val.val().indexOf("/") != 0) {
            $val.val('/' + $val.val());
        }
    });

    $('#ConfirmAddRedirect').click(function (e) {
        var source = $("#SourceInput").val();
        var destination = $("#DestinationInput").val();
        var isPermanent = $("#ToggleBookmark").is(':checked');
        var isRoot = $("#MatchingInput").is(':checked');

        if (source.length < 1 || destination.length < 1) {
            return false;
        }

        var $container = $("#AddRedirectModal").find("div.wrapper");
        common.showAjaxLoader($container);

        $.ajax({
            url: "/admin/redirect/addredirect",
            type: "POST",
            data: {
                source: source,
                destination: destination,
                isPermanent: isPermanent,
                rootMatching: isRoot
            },
            success: function (data) {
                // Hide loader
                common.hideAjaxLoader($container);
                // Close Modal

                $("#redirectBody").append('<tr><td>' + source + '</td><td>' + destination + '</td><td>' + isPermanent + '</td><td>' + isRoot + '</td><td><a class="deleteRedirectButton btn btn-danger btn-sm" href="#" data-id="' + data.id + '">Delete</a></td></tr>');
                $('#AddRedirectModal').modal('hide');
            },
            error: function (data) {
                // Close Modal
                common.hideAjaxLoader($container);
                $('#AddRedirectModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};

// Keep at the bottom
$(document).ready(function () {
    redirect = new redirect_class();
    redirect.initRedirectEvents();
});