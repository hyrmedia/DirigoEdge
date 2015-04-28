category_class = function () {
    this.ajax = new AjaxService();
};

category_class.prototype.initPageEvents = function () {
    this.initAddCategoryEvent();
    this.initDeleteCategoryEvent();
    this.initConfirmCatDeleteEvent();
};

category_class.prototype.initConfirmCatDeleteEvent = function () {
    var self = this;

    // Confirm deleteion ofself category from cat table
    $("#ConfirmDeleteCategory").click(function () {
        var catIdToDelete = self.$catRowToDelete.attr("data-id");

        var $container = $("#DeleteCategoryModal").find("div.wrapper");
        common.showAjaxLoader($container);

        var newId = $('#allCategories option:selected').val();
        var newCat = $($('tr[data-id=' + newId + '] .total'));

        var success = function (data) {
            newCat.text(data.newCount);
            noty({ text: 'Category Successfully Deleted.', type: 'success', timeout: 2000 });
            self.$catRowToDelete.remove();
            common.hideAjaxLoader($container);
            $('#DeleteCategoryModal').modal('hide');
        };

        var error = function () {
            common.hideAjaxLoader($container);
            $('#DeleteCategoryModal').modal('hide');
            noty({ text: 'There was an error processing your request.', type: 'error' });
        };

        var data =
        {
            id: catIdToDelete,
            newId: newId
        };

        self.ajax.Post(data, "/admin/category/deletecategory", success, error);
    });
};

category_class.prototype.initDeleteCategoryEvent = function () {
    var self = this;

    $('body').on("click", "a.deleteCategoryButton", function () {
        // Store the row to be removed so the dialog box can access is
        self.$catRowToDelete = $(this).closest("tr");
        var catIdToDelete = self.$catRowToDelete.attr("data-id");

        $("#popTitle").text("'" + self.$catRowToDelete.find("td.name").text() + "'");

        var success = function (data) {

            $('#allCategories').empty();
            $.each($.parseJSON(data), function (index, value) {
                console.log(value);
                if (value.Id != catIdToDelete) {
                    var option = $('<option></option>').attr("value", value.Id).text(value.Name);
                    $('#allCategories').append(option);
                }
            });
        }

        $("#DeleteCategoryModal").modal();
        self.ajax.Get({}, "/admin/category/all", success);

        return false;
    });
};

category_class.prototype.initAddCategoryEvent = function () {
    var self = this;

    // Add Category Button
    $("#ConfirmAddCategory").click(function () {

        var name = $("#CategoryNameInput").val();
        if (name.length < 1) {
            return false;
        }

        var $container = $("#AddCategoryModal").find("div.wrapper");

        var success = function (data) {

            noty({ text: 'Category Successfully Created.', type: 'success', timeout: 2000 });
            $("#CategoryNameInput").val('');
            $("#CategoriesTable").append('<tr><td class="name">' + name + '</td><td>0</td><td><a data-id="' + data.id + '" href="javascript:void(0);" class="deleteCategoryButton btn btn-danger btn-sm">Delete</a></td></tr>');
            common.hideAjaxLoader($container);
            $('#AddCategoryModal').modal('hide');
        };

        var error = function () {
            common.hideAjaxLoader($container);
            $('#AddCategoryModal').modal('hide');
            noty({ text: 'There was an error processing your request.', type: 'error' });
        };

        common.showAjaxLoader($container);
        self.ajax.Post({ name: name }, "/admin/category/addcategory", success, error);

        return false;
    });
};

// Keep at the bottom
$(document).ready(function () {
    categoryAdmin = new category_class();
    categoryAdmin.initPageEvents();
});