redirect_class = function () {

    $('[data-toggle="popover"]').popover();

    this.oTable = $("table.manageTable").DataTable({
        "iDisplayLength": 25,
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": ["actions"] } // No Sorting on actions
        ],
        "aaSorting": [[0, "desc"]] // Sort by id
    });

};

redirect_class.prototype.initRedirectEvents = function () {
    var self = this;

    $('.redirect-destination-toggle').click(function (e) {

        $('.redirect-destination-toggle').toggleClass('disabled');
        $('.redirect-destination').toggleClass('hidden');

        e.preventDefault();
    });
    
    $("body").on('click', ".deleteRedirectButton", function () {

        self.manageRedirectId = $(this).attr("data-id");
        self.$manageRedirectRow = $(this).closest('tr')[0];
        self.oTable = $("#DataTables_Table_0").dataTable();
        self.rowIndex = self.oTable.fnGetPosition(self.$manageRedirectRow);

        $("#DeleteModal").modal();
    });

    $("body").on('click', ".editRedirectButton", function () {

        var $modal = $("#EditRedirectModal");

        self.manageRedirectId = $(this).attr("data-id");
        self.$manageRedirectRow = $(this).closest('tr');
        self.oTable = $("#DataTables_Table_0").dataTable();
        self.rowIndex = self.oTable.fnGetPosition(self.$manageRedirectRow[0]);

        $modal.find('#EditSourceInput').val(self.$manageRedirectRow.find('.source').text());
        $modal.find('#EditDestinationInput').val(self.$manageRedirectRow.find('.destination').text());
        $modal.find('#EditPermanentInput').prop('checked', self.$manageRedirectRow.find('.permanent').text().toLowerCase() === 'true');
        $modal.find('#EditMatchingInput').prop('checked', self.$manageRedirectRow.find('.root-matching').text().toLowerCase() === 'true');

        $('.redirect-destination-toggle.existing').removeClass('disabled');
        $('.redirect-destination-toggle.custom').addClass('disabled');

        $('#EditDestinationSelect').addClass('hidden');
        $('#EditDestinationInput').removeClass('hidden');

        $modal.modal();
    });

    $('#AddRedirectModal').on('shown.bs.modal', function (e) {
        $('#SourceInput').focus();
    });

    $('#EditRedirectModal').on('shown.bs.modal', function (e) {
        $('#EditSourceInput').focus();
    });

    $('#EditRedirectModal').on('hidden.bs.modal', function (e) {
        self.manageRedirectId = null;
        self.$manageRedirectRow = null;
        self.oTable = null;
        self.rowIndex = null;

        $('.redirect-destination-toggle.existing').addClass('disabled');
        $('.redirect-destination-toggle.custom').removeClass('disabled');
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
                self.refreshTable();
                $('#DeleteModal').modal('hide');
            },
            error: function (data) {
                $('#DeleteModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });

    $('#SourceInput, #EditSourceInput').on('keyup', function (e) {
        var $input = $(this);
        var source = $input.val();

        if (source.indexOf("/") !== 0) {
            $input.val('/' + source);
        }
    });

    $('#DestinationInput, #EditDestinationInput').on('blur', function (e) {
        var $input = $(this);
        var destination = $input.val();

        // Enforce leading slash unless destination is external
        if (!destination.match(/^http/) && destination.indexOf("/") !== 0) {
            $input.val('/' + destination);
        }
    });

    $('#ConfirmAddRedirect').click(function (e) {
        var source = $("#SourceInput").val();
        var destination = $("#DestinationInput").hasClass('hidden') ? $("#DestinationSelect option:selected").text() : $("#DestinationInput").val();
        var isPermanent = $("#PermanentInput").is(':checked');
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

                // Reset form to defaults
                $("#SourceInput").val('');
                $("#DestinationInput").val('');
                $("#PermanentInput").prop('checked', false);
                $("#MatchingInput").prop('checked', false);
                self.refreshTable();
                $('#AddRedirectModal').modal('hide');
            },
            error: function (data) {
                // Close Modal
                common.hideAjaxLoader($container);
                $('#AddRedirectModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });

    $('#ConfirmEditRedirect').click(function (e) {
        var source = $("#EditSourceInput").val();
        var destination = $("#EditDestinationInput").hasClass('hidden') ? $("#EditDestinationSelect option:selected").text() : $("#EditDestinationInput").val();
        var isPermanent = $("#EditPermanentInput").is(':checked');
        var isRoot = $("#EditMatchingInput").is(':checked');

        if (source.length < 1 || destination.length < 1) {
            return false;
        }

        var $container = $("#EditRedirectModal").find("div.wrapper");
        common.showAjaxLoader($container);

        $.ajax({
            url: "/admin/redirect/editredirect",
            type: "POST",
            data: {
                id: self.manageRedirectId,
                source: source,
                destination: destination,
                isPermanent: isPermanent,
                rootMatching: isRoot
            },
            success: function (data) {
                // Hide loader
                common.hideAjaxLoader($container);

                // Reset form to defaults
                $("#EditSourceInput").val('');
                $("#EditDestinationInput").val('');
                $("#EditPermanentInput").prop('checked', false);
                $("#EditMatchingInput").prop('checked', false);
                self.refreshTable();
                $('#EditRedirectModal').modal('hide');
            },
            error: function (data) {
                // Close Modal
                common.hideAjaxLoader($container);
                $('#EditRedirectModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });
};

redirect_class.prototype.refreshTable = function() {
    var $container = $('.redirect-table-wrapper');
    common.showAjaxLoader($container);
    $container.load('/admin/redirect/redirects/ #redirect-table', function() {
        common.hideAjaxLoader($container);
        self.oTable = $("table.manageTable").DataTable({
            "iDisplayLength": 25,
            "aoColumnDefs": [
                { "bSortable": false, "aTargets": ["actions"] } // No Sorting on actions
            ],
            "aaSorting": [[0, "desc"]] // Sort by id
        });
    });
};

// Keep at the bottom
$(document).ready(function () {
    redirect = new redirect_class();
    redirect.initRedirectEvents();
});