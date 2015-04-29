blog_authors_class = function () {

};

blog_authors_class.prototype.initPageEvents = function() {

    var self = this;

    this.sortAuthors();

    $("#CreateAuthorButton").click(function () {
        var data = {
            user: {
                DisplayName: $("#NewUserName").val(),
                UserName: $("#NewUserName").val(),
                UserImageLocation: $("#NewUserImage").val(),
                IsActive: $("#IsActiveBox").is(':checked'),
            }
        };

        var $container = $("#NewAuthorModal div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/blog/addbloguser",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                $('#NewAuthorModal').modal('hide');
                $("#NewUserName").val('');
                $("#NewUserImage").val('');

                common.hideAjaxLoader($container);

                //Refresh the inner content to show the new user
                self.refreshAuthorTable(noty({ text: 'Author Successfully Created.', type: 'success', timeout: 3000 }));
            },
            error: function (data) {
                $('#NewAuthorModal').modal('hide');
                common.hideAjaxLoader($container);
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });

    // Manage Users Edit User
    $("#Main div.manageAuthors").on("click", "a.editUser.btn", function () {
        var $row = $(this).parent().parent();
        var $el = $(this);
        self.EditUserId = $(this).attr("data-id");
        self.EditUserDisplayName = $row.find("td.displayName").text();
        self.EditUserIsActive = $row.find("td.isActive").text();
        self.EditUserImageLoc = $row.find("td.imageLocation img").attr("src");

        // Set the modal properties
        $("#ModUserName").val(self.EditUserDisplayName);
        $("#ModUserImageLocation").val(self.EditUserImageLoc);
        if ($row.find("td.isActive").text() == "True") {
            // Do Checkbox
            $("#ModUserIsActiveBox").prop('checked', true);
        }
        else {
            $("#ModUserIsActiveBox").prop('checked', false);
        }

        $("#ModifyAuthorModal").modal();
    });

    // Submit edit author
    $("#ModifyAuthorButton").click(function () {

        var data = {
            user: {
                DisplayName: $("#ModUserName").val(),
                UserImageLocation: $("#ModUserImageLocation").val(),
                IsActive: $("#ModUserIsActiveBox").is(":checked"),
                UserID: self.EditUserId,
            }
        };

        var $container = $("#ModifyAuthorModal > div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/blog/modifybloguser",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                common.hideAjaxLoader($container);

                // Close the dialog box
                $('#ModifyAuthorModal').modal('hide');

                //Refresh the inner content to show the new user
                self.refreshAuthorTable(noty({ text: 'User Successfully Modified.', type: 'success', timeout: 3000 }));
            },
            error: function (data) {
                common.hideAjaxLoader($container);
                $('#ModifyAuthorModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });

    // Delete author and confirmation
    $("#Main div.manageAuthors").on("click", "a.deleteUser.btn", function () {
        var $row = $(this).parent().parent();
        var $el = $(this);
        self.EditUserId = $(this).attr("data-id");
        self.EditUserDisplayName = $row.find("td.displayName").text();
        $("#DelUserName").text(self.EditUserDisplayName);
        $("#DeleteAuthorModal").modal();
    });

    // Submit Delete Author
    $("#DeleteAuthorButton").click(function () {
        var data = { userId: self.EditUserId };

        $.ajax({
            url: "/admin/blog/deletebloguser",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                $('#DeleteAuthorModal').modal('hide');

                blog_authors.refreshAuthorTable(noty({ text: 'Author Successfully Deleted.', type: 'success', timeout: 3000 }));
            },
            error: function (data) {
                $('#ModifyUserModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });

    // Close Delete User Modal
    $("#CancelDeleteButton").click(function () {
        $('#DeleteUserModal').modal('hide');
    });
};

blog_authors_class.prototype.refreshAuthorTable = function (fSuccess) {
    //Refresh the inner content to show the new user

    var $container = $("#ManageAuthorTableContainer");
    common.showAjaxLoader($container);

    $("#ManageAuthorTableContainer").load("/admin/blog/manageblogauthors/ #ManageAuthorTable", function (data) {
        var noty_id = fSuccess;

        // Sort the table again since the html has changed
        blog_authors.sortAuthors();

        common.hideAjaxLoader($container);
    });
};

blog_authors_class.prototype.sortAuthors = function () {
    $("#ManageAuthorTable").dataTable({
        "iDisplayLength": 25,
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": ["actions"] } // No Sorting on actions
        ],
        "aaSorting": [[1, "desc"]] // Sort by User Name date on load
    });
};

// Keep at the bottom
$(document).ready(function () {
    blog_authors = new blog_authors_class();

    if ($("#ManageAuthorTableContainer").length > 0) {
        blog_authors.initPageEvents();
    }
});