/// ===========================================================================================
/// User Management
/// ===========================================================================================

user_class = function () {

};

user_class.prototype.initPageEvents = function () {
    // Save delegate resources by only triggering on the correct page
    if ($("#ManageUserTable").length > 0) {
        this.manageUserAdminEvents();
        this.manageChangePasswordEvents();
        this.sortUsers();
    }
};

user_class.prototype.sortUsers = function () {
    $("#ManageUserTable").dataTable({
        "iDisplayLength": 25,
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": ["actions"] } // No Sorting on actions
        ],
        "aaSorting": [[1, "desc"]] // Sort by User Name date on load
    });
};

user_class.prototype.manageUserAdminEvents = function () {
    var self = this;

    $('.user-image-btn.new').fileBrowser(function (object) {
        if (object.type === 'image' && object.src) {
            $('#NewUserImage').val(object.src);
        }
    });

    $('.user-image-btn.mod').fileBrowser(function (object) {
        if (object.type === 'image' && object.src) {
            $('#ModUserImageLocation').val(object.src);
        }
    });

    $('#NewUser').click(function () {
        $("#NewUserName").val('');
        $("#NewUserFirstName").val('');
        $("#NewUserLastName").val('');
        $('#NewUserEmail').val('');
        $("#NewUserImage").val("/areas/admin/css/images/user.png");
        $("#NewUserPassword").val('');
        $("#NewUserModal div.roleListing input[type=checkbox]").each(function () {
            $(this).prop('checked', false).next().removeClass("checked");
        });
    });

    $("#NewUserName, #NewUserPassword").on('focus', function () {
        $('.user-error').addClass('hide').html('');
    });

    // Add User click event
    $("#CreateUserButton").click(function () {
        $('.user-error').addClass('hide').html('');
        if (!$("#NewUserName").val().length || !$("#NewUserPassword").val().length) {
            $('.user-error').removeClass('hide').html('Please fill in all fields.');
            return false;
        }
        var data = {
            user: {
                DisplayName: $("#NewUserName").val(),
                UserName: $("#NewUserName").val(),
                UserImageLocation: $("#NewUserImage").val(),
                IsApproved: $("#IsActiveBox").is(':checked'),
                Password: $("#NewUserPassword").val(),
                FirstName: $("#NewUserFirstName").val(),
                LastName: $("#NewUserLastName").val(),
                Email: $("#NewUserEmail").val(),
                Roles: []
            }
        };

        var $roles = $("#NewUserModal div.roleListing input[type=checkbox]:checked");
        if (!$roles.length) {
            $('#NewUserModal .user-error').text('Please choose at least one role.').removeClass('hide');
            return false;
        }
        $roles.each(function () {
            var oRole = {
                RoleName: $(this).data('role'),
            };
            data.user.Roles.push(oRole);
        });

        var $container = $("#NewUserModal div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/users/adduser",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                $('#NewUserModal').modal('hide');
                $('.user-error').addClass('hide').html('');

                common.hideAjaxLoader($container);

                //Refresh the inner content to show the new user
                noty({ text: 'User Successfully Created.', type: 'success', timeout: 3000 });
                self.refreshUserTable();
            },
            error: function (data) {
                $('#NewUserModal').modal('hide');
                common.hideAjaxLoader($container);
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });

    // Manage Users Edit User
    $("#Main div.manageUsers").on("click", "a.editUser.btn", function () {
        var $row = $(this).closest('tr');
        var $el = $(this);
        self.EditUserId = $(this).attr("data-id");
        self.EditUserDisplayName = $row.find("td.displayName").text().trim();
        self.EditUserDisplayFirstName = $row.find("td.displayFirstName").text().trim();
        self.EditUserDisplayLastName = $row.find("td.displayLastName").text().trim();
        self.EditUserDisplayEmail = $row.find("td.displayEmail").text().trim();
        self.EditUserIsActive = $row.find("td.isActive").text().trim();
        self.EditUserImageLoc = $row.find("td.imageLocation img").attr("src");
        $('#ModifyUserModal .user-error').text('').addClass('hide');

        // Set the modal properties
        $("#ModUserName").val(self.EditUserDisplayName);
        $("#ModUserFirstName").val(self.EditUserDisplayFirstName);
        $("#ModUserLastName").val(self.EditUserDisplayLastName);
        $("#ModUserEmail").val(self.EditUserDisplayEmail);
        $("#ModUserImageLocation").val(self.EditUserImageLoc);
        if ($row.find("td.isActive").text() == "true") {
            $("#ModUserIsActiveBox").prop("checked", true);
        }
        else {
            $("#ModUserIsActiveBox").prop("checked", false);
        }

        // Set the Roles
        $("#ModifyUserModal div.roleListing input[type=checkbox]").each(function () {
            var role = $(this).data("role");

            if ($row.find("td.roles:contains(" + role + ")").length > 0) {
                $(this).prop('checked', true).next().addClass("checked");
            }
            else {
                $(this).prop('checked', false).next().removeClass("checked");
            }
        });

        $("#ModifyUserModal").modal();
    });

    // Submit edit user
    $("#ModifyUserButton").click(function () {
        $('#ModifyUserModal .user-error').text('').addClass('hide');
        // populate roles
        var roles = [];
        var $roles = $('#ModifyUserModal .roleListing input[type="checkbox"]:checked');
        if (!$roles.length) {
            $('#ModifyUserModal .user-error').text('Please choose at least one role.').removeClass('hide');
            return false;
        }
        $roles.each(function () {
            var oRole = {
                RoleName: $(this).data("role"),
            };

            roles.push(oRole);
        });


        var data = {
            user: {
                UserName: $("#ModUserName").val(),
                UserImageLocation: $("#ModUserImageLocation").val(),
                IsApproved: $("#ModUserIsActiveBox").is(":checked"),
                UserID: self.EditUserId,
                FirstName: $("#ModUserFirstName").val(),
                LastName: $("#ModUserLastName").val(),
                Email: $("#ModUserEmail").val(),
                Roles: roles
            }
        };

        var $container = $("#ModifyUserModal > div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/users/modifyuser",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                common.hideAjaxLoader($container);
                noty({ text: data.message, type: data.success ? 'success' : 'error', timeout: 3000 });

                //Refresh the inner content to show the new user
                if (data.success) {
                    // Close the dialog box
                    $('#ModifyUserModal').modal('hide');
                    self.refreshUserTable();
                }
            },
            error: function (data) {
                common.hideAjaxLoader($container);
                $('#ModifyUserModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });

    // Delete user and confirmation
    $("#Main div.manageUsers").on("click", "a.deleteUser.btn", function () {
        var $row = $(this).parent().parent();
        var $el = $(this);
        self.EditUserId = $(this).attr("data-id");
        self.EditUserDisplayName = $row.find("td.displayName").text();
        $("#DelUserName").text(self.EditUserDisplayName);
        $("#DeleteUserModal").modal();
    });

    // Submit Delete user
    $("#DeleteUserButton").click(function () {
        var data = {
            user: {
                UserID: self.EditUserId
            }
        };

        var $container = $("#DeleteUserModal > div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/users/deleteuser",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                common.hideAjaxLoader($container);

                // Close the dialog box
                $('#DeleteUserModal').modal('hide');
                noty({ text: 'User Successfully Deleted.', type: 'success', timeout: 3000 });
                self.refreshUserTable();
            },
            error: function (data) {
                common.hideAjaxLoader($container);
                $('#ModifyUserModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });

    // Close Delete User Modal
    $("#CancelDeleteButton").click(function () {
        $('#DeleteUserModal').modal('hide');
    });
};

user_class.prototype.manageChangePasswordEvents = function () {
    var self = this;

    $("#ChangeUserPassword").click(function () {
        // set up modal info before showing modal
        $("#ChngPasswdUname").text(self.EditUserDisplayName);
        $('.password-error').addClass('hide').html('');

        //
        $("#ChangePasswordModal").modal();
    });

    // Change Password Submit
    $("#ChangeUserPasswordButton").click(function () {
        $('.password-error').addClass('hide').html('');
        //self.EditUserId
        var newPassword = $("#NewUserChangePassword").val();
        var newPasswordRepeated = $("#RepeatNewUserChangePassword").val();

        // Passwords must match
        if (newPassword != newPasswordRepeated) {
            $('.password-error').removeClass('hide').html("Passwords must match");
            return;
        }

        // Min lenfth on password
        if (newPassword.length < 1) {
            $('.password-error').removeClass('hide').html("Passwords much be at least one character long.");
            return;
        }

        // We're good - send off the ajax call
        var $container = $("#ChangePasswordModal .content");
        common.showAjaxLoader($container);
        var data = { user: { UserID: self.EditUserId }, newPassword: newPassword };
        $.ajax({
            url: "/admin/users/changeuserpassword",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                common.hideAjaxLoader($container);
                $('#ChangePasswordModal').modal('hide');
                $('.password-error').addClass('hide').html('');
                noty({ text: 'Password Successfully Updated.', type: 'success', timeout: 3000 });
                self.refreshUserTable();
            },
            error: function (data) {
                common.hideAjaxLoader($container);
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });
};

user_class.prototype.refreshUserTable = function () {
    //Refresh the inner content to show the new user
    var $container = $("#ManageUserTableContainer");

    common.showAjaxLoader($container);
    $container.load("/admin/users/manageusers/ #ManageUserTable", function (data) {
        // Sort the table again since the html has changed
        user.sortUsers();
        common.hideAjaxLoader($container);
    });
};

// Keep at the bottom
$(document).ready(function () {
    user = new user_class();
    user.initPageEvents();
});