reset_password_class = function () {

};

reset_password_class.prototype.initPageEvents = function () {
    if ($('#ResetPasswordForm').length > 0) this.initResetPasswordEvents();


    if ($('#ChangePasswordForm').length > 0) this.initChangePasswordEvents();

};

reset_password_class.prototype.initResetPasswordEvents = function () {
    $('#ResetPasswordForm #ValidateEmail').click(function () {
        var email = $('#ResetPasswordForm #Email').val();

        common.showAjaxLoader($('#ValidateEmail'));

        $.ajax({
            type: 'post',
            url: '/account/getsecretquestion/',
            data: { email: email },
            success: function (data) {
                if (!data.error) {
                    if (data.secret) {
                        $('#SecretQuestion label').text(data.secret);
                        $('#SecretQuestion').show();
                    }
                    common.hideAjaxLoader($('#ValidateEmail'));
                } else {
                    $('#ErrorMessage span').text('Invalid email.');
                }
            }

        });

        return false;
    });

};

reset_password_class.prototype.initChangePasswordEvents = function () {

    $('#Password, #ConfirmPassword').keyup(function () {
        var $container = $(this).parent().next();
        var $msgContainer = $(this).parents(".row").find(".message");
        common.hideAjaxLoader($container);
        common.showAjaxLoader($container, true);
        var time = setTimeout(function () {

            var password = $('#ChangePasswordForm #Password').val();
            var cpassword = $('#ChangePasswordForm #ConfirmPassword').val();

            var isValid = common.isValidPassword(password) && common.isValidPassword(cpassword);

            if (password != '' && cpassword != '' && isValid && cpassword == cpassword) {

                clearTimeout(time);

                // Remove message
                $msgContainer.html('');

            } else {

                // Show message
                if (password != cpassword) {
                    $msgContainer.html('<div class="alert-box alert">Passwords must match</div>');
                } else {
                    $msgContainer.html('<div class="alert-box alert">Must be at least 12 characters, contain one uppercase letter, one lowercase letter, one number, and one special character (!&#64;#$&*)</div>');
                }
            }

            common.hideAjaxLoader($container);

        }, 1000);
    });

};


// Keep at the bottom
$(document).ready(function () {
    reset_password = new reset_password_class();
    reset_password.initPageEvents();
});