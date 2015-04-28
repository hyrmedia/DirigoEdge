/// ===========================================================================================
/// User Role Management
/// ===========================================================================================

role_class = function () {

};

role_class.prototype.initPageEvents = function () {
    // Save delegate resources by only triggering on the correct page
    if ($("#ManageUserRolesTable").length > 0) {

        this.manageUserRoleAdminEvents();

        this.initPermissionEvents();

        this.initEditUsersEvents();

        this.initRegistrationEvents();

        this.sortRoles();
    }
};

role_class.prototype.manageUserRoleAdminEvents = function () {
    var self = this;

    $("#RoleName").on('focus', function() {
        $('.role-error').addClass('hide').html('');
    });

    // Create User Role
    $("#CreateUserRoleButton").click(function() {
        $('.role-error').addClass('hide').html('');
        var roleName = $("#RoleName").val();
        if (roleName.length < 1) {
            $('.role-error').removeClass('hide').html("Please enter a Role Name.");
            return false;
        }
        
        var data = {
            role: {
                RoleName: roleName,
                Permissions: []
            }
        };

        // Add the Roles
        $("#NewUserRoleModal ul.rolePermissionsList input[type=checkbox]").each(function () {
            if ($(this).is(":checked")) {
                data.role.Permissions.push({ PermissionId: $(this).data("key"), PermissionName: $(this).data("name") });
            }
        });
        if (data.role.Permissions.length < 1) {
            noty({ text: 'Please Select a Permission.', type: 'error', timeout: 3000 });
            return false;
        }
        var $container = $("#NewUserRoleModal div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/roles/adduserrole",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                $('#NewUserRoleModal').modal('hide');

                common.hideAjaxLoader($container);

                //Refresh the inner content to show the new user
                self.refreshUserRoleTable(noty({ text: 'Role Successfully Created.', type: 'success', timeout: 3000 }));
            },
            error: function (data) {
                $('#NewUserModal').modal('hide');
                common.hideAjaxLoader($container);
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
    
    // Delete user role and confirmation
    $(".manageUsers").on("click", "a.deleteUserRole.btn", function () {
        var $row = $(this).parent().parent();
        var $el = $(this);
        self.EditUserRoleId = $(this).attr("data-id");
        self.EditUserRoleDisplayName = $row.find("td.roleName").text();
        $("#DelUserRole").text(self.EditUserRoleDisplayName);
        $("#DeleteUserRoleModal").modal();
    });

    // Submit Delete user
    $("#DeleteUserRoleButton").click(function () {
        var data = {
            role: {
                RoleId: self.EditUserRoleId
            }
        };

        var $container = $("#DeleteUserRoleModal > .content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/roles/deleterole",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                common.hideAjaxLoader($container);
                $('#DeleteUserRoleModal').modal('hide');

                if (data.success) {
                    self.refreshUserRoleTable(noty({ text: 'Role Successfully Deleted.', type: 'success', timeout: 3000 }));
                } else {
                    noty({ text: data.message, type: 'error' });
                }
            },
            error: function (data) {
                common.hideAjaxLoader($container);
                $('#DeleteUserRoleModal').modal('hide');
                noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });

    // Close Delete User Modal
    $("#CancelDeleteButton").click(function () {
        $('#DeleteUserRoleModal').modal('hide');
    });        
};

role_class.prototype.initRegistrationEvents = function () {
    var self = this;
    
    $(document).on("click", "a.regCodes", function () {
        var $row = $(this).parent().parent();
        self.EditUserRoleId = $(this).attr("data-id");
        self.EditUserRoleDisplayName = $row.find("td.roleName").text();

        $("#RegCodeInput").val($(this).attr("data-code"));

        $("#EditRoleRegistrationModal").modal();
    });

    // Save RegCode
    $("#ModifyRegCodeButton").click(function() {
        var data = {
            role: {
                RoleId : self.EditUserRoleId,
                RegistrationCode : $("#RegCodeInput").val()
            }
        };

        var $container = $("#EditRoleRegistrationModal > .content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/roles/updaterolecode",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                // Close the dialog box
                common.hideAjaxLoader($container);
                $('#EditRoleRegistrationModal').modal('hide');

                self.refreshUserRoleTable(noty({ text: 'Registration Code Updated.', type: 'success', timeout: 3000 }));
            },
            error: function (data) {
                common.hideAjaxLoader($container);
                $('#EditRoleRegistrationModal').modal('hide');
                noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};


role_class.prototype.initPermissionEvents = function () {
    var self = this;
    
    // Show user permissions
    $(document).on("click", "a.showPermissions", function () {
        var $row = $(this).parent().parent();
        self.EditUserRoleId = $(this).attr("data-id");
        self.EditUserRoleDisplayName = $row.find("td.roleName").text();
        $("#EditUserRole").text(self.EditUserRoleDisplayName);

        // Set the permissions to edit accordingly
        var nCurrentRolePermissions = $row.find("td.permsList").text().split(", ");

        $("#EditUserRolePermissionsModal ul.rolePermissionsList li").each(function () {
            var currentRole = $(this).find("span.key").text();

            if ($.inArray(currentRole, nCurrentRolePermissions) != -1) {
                $(this).find("input[type=checkbox]").prop('checked', true);
            }
            else {
                $(this).find("input[type=checkbox]").prop('checked', false);
            }
        });

        $("#EditUserRolePermissionsModal").modal();
    });

    // Edit Role Permissions
    $("#ModifyPermissionsButton").click(function () {

        var data = {
            role: {
                RoleName: self.EditUserRoleDisplayName,
                RoleId: self.EditUserRoleId,
                Permissions: []
            }
        };

        // Add the roles
        $("#EditUserRolePermissionsModal ul.rolePermissionsList input[type=checkbox]").each(function () {
            if ($(this).is(":checked")) {
                data.role.Permissions.push({ PermissionId: $(this).data("key"), PermissionName: $(this).data("name") });
            }
        });
        if (data.role.Permissions.length < 1) {
            noty({ text: 'Please Select a Permission.', type: 'error', timeout: 3000 });
            return false;
        }
        var $container = $("#EditUserRolePermissionsModal div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/roles/modifyrolepermissions",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                common.hideAjaxLoader($container);

                // Close the dialog box
                $('#EditUserRolePermissionsModal').modal('hide');

                //Refresh the inner content to show the new user
                self.refreshUserRoleTable(noty({ text: 'Permission(s) Successfully Modified.', type: 'success', timeout: 3000 }));
            },
            error: function () {
                common.hideAjaxLoader($container);
                $('#EditUserRolePermissionsModal').modal('hide');
                noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};


role_class.prototype.initEditUsersEvents = function () {
    var self = this;

    // Show modal
    $(document).on("click", "a.showUsers", function () {
        
        var $row = $(this).parent().parent();
        self.EditUserRoleId = $(this).attr("data-id");
        self.EditUserRoleDisplayName = $row.find("td.roleName").text();
        $("#EditUsersNRole").text(self.EditUserRoleDisplayName);
        
        // Hide how many users changed
        $("#UsersChangedContainer").hide();

        // Show modal
        $("#EditUsersInRoleModal").modal();
        
        // Ajax in the Users for the current role
        var $container = $("#EditUsersInRoleModal > div.content");
        common.showAjaxLoader($container);
        var data = { RoleName: self.EditUserRoleDisplayName };
        $("#UserListing").load("/admin/roles/getroleusers/", data, function () {
            
            // Success
            common.hideAjaxLoader($container);
        });
    });
    
    // Toggle checkboxes
    $("#UserListing ul.userList").on("click", "li", function (e) {

        // If we clicked on the checkbox, proceed as usual 
        if ($(e.target).attr("type") == "checkbox") {
            return;
        }

        // Otherise toggle the checkbox
        var $check = $(this).find("input[type=checkbox]");
        $check.prop("checked", !$check.prop("checked"));
        $check.trigger("change");
    });
    
    // Toggling checkboxes changes user changed count and marks for update
    $("#UserListing ul.userList li").on("change", "input[type=checkbox]", function (e) {
        e.preventDefault();
        
        // Update user count
        $(this).closest("li").addClass("changed");

        var changedCount = $("#UserListing ul.userList li.changed").length;

        $("#UsersChangedContainer").show();
        $("#UserModCount").text(changedCount);
    });

    // Add Remove User Submission
    $("#ModifyUserInRoleButton").click(function () {

        // compile the user count
        var addUsers = [];
        var removeUsers = [];

        $("#UserListing ul.userList li.changed").each(function () {

            var $checked = $(this).find("input[type=checkbox]").is(":checked");
            
            if ($checked) {
                addUsers.push($(this).attr("data-id"));
            }
            else {
                removeUsers.push($(this).attr("data-id"));
            }
        });

        var data = {
            RemoveUsers: removeUsers,
            AddUsers: addUsers,
            RoleID: self.EditUserRoleId
        };
        var $container = $("#EditUsersInRoleModal div.content");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/roles/modifyusersinrole",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                common.hideAjaxLoader($container);

                // Close the dialog box
                $('#EditUsersInRoleModal').modal('hide');

                //Refresh the inner content to show the new user
                self.refreshUserRoleTable(noty({ text: 'Users Successfully Modified.', type: 'success', timeout: 3000 }));
            },
            error: function () {
                common.hideAjaxLoader($container);
                $('#EditUsersInRoleModal').modal('hide');
                noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });

    });

};

role_class.prototype.sortRoles = function () {
    $("#ManageUserRolesTable").dataTable({
        "iDisplayLength": 25,
        "aoColumnDefs": [
            { "bSortable": false, "aTargets": ["actions"] } // No Sorting on actions
        ],
        "aaSorting": [[0, "asc"]] // Sort by Role Name date on load
    });
};

role_class.prototype.refreshUserRoleTable = function (fSuccess) {

    var $container = $("#ManageUserTableContainer");
    
    common.showAjaxLoader($container);
    //Refresh the inner content to show the new user
    $("#ManageUserTableContainer").load("/admin/roles/manageuserroles/ #ManageUserRolesTable", function (data) {
        var noty_id = fSuccess;

        // Sort the table again since the html has changed
        roleAdmin.sortRoles();
        
        common.hideAjaxLoader($container);
    });
};

// Keep at the bottom
$(document).ready(function () {
    roleAdmin = new role_class();
    roleAdmin.initPageEvents();
});