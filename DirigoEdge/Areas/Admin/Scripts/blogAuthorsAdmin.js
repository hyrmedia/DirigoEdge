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

        var success = function () {
            $('#NewAuthorModal').modal('hide');
            $("#NewUserName").val('');
            $("#NewUserImage").val('');

            common.hideAjaxLoader($container);

            //Refresh the inner content to show the new user
            self.refreshAuthorTable(noty({ text: 'Author Successfully Created.', type: 'success', timeout: 3000 }));
        };

        var error = function() {
            $('#NewAuthorModal').modal('hide');
            common.hideAjaxLoader($container);
            noty({ text: 'There was an error processing your request.', type: 'error' });
        };

        EDGE.ajaxPost(data, "/admin/blog/addbloguser", success, error);
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

        var success = function() {
            common.hideAjaxLoader($container);
            $('#ModifyAuthorModal').modal('hide');
            self.refreshAuthorTable(noty({ text: 'User Successfully Modified.', type: 'success', timeout: 3000 }));
        };

        var error = function() {
            common.hideAjaxLoader($container);
            $('#ModifyAuthorModal').modal('hide');
            noty({ text: 'There was an error processing your request.', type: 'error' });
        };

        EDGE.ajaxPost(data, "/admin/blog/modifybloguser", success, error);
    });

    // Delete author and confirmation
    $("#Main div.manageAuthors").on("click", "a.deleteUser.btn", function () {
        var $row = $(this).parent().parent();
        var $el = $(this);
        self.EditUserId = $(this).attr("data-id");
        self.EditUserDisplayName = $row.find("td.displayName").text();
        $("#DelUserName").text(self.EditUserDisplayName);

        var success = function (data) {

            $('#allAuthors').empty();
            $.each($.parseJSON(data), function (index, value) {
                console.log(value);
                if (value.Id != self.EditUserId) {
                    var option = $('<option></option>').attr("value", value.Id).text(value.DisplayName);
                    $('#allAuthors').append(option);
                }
            });
        }

        $("#DeleteCategoryModal").modal();
        EDGE.ajaxGet({}, "/admin/blog/GetAllBlogAuthors", success);

        $("#DeleteAuthorModal").modal();
    });

    // Submit Delete Author
    $("#DeleteAuthorButton").click(function () {
        var newId = $('#allAuthors option:selected').val();
        var data = { userId : self.EditUserId, newUserId: newId } ;
        var success = function() {
            $('#DeleteAuthorModal').modal('hide');
            blog_authors.refreshAuthorTable(noty({ text: 'Author Successfully Deleted.', type: 'success', timeout: 3000 }));
        };
        var error = function() {
            $('#ModifyUserModal').modal('hide');
            noty({ text: 'There was an error processing your request.', type: 'error' });
        };

        EDGE.ajaxPost(data, "/admin/blog/deletebloguser", success, error);
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