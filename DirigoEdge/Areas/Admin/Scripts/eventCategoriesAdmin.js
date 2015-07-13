event_category_class = function() {

};

event_category_class.prototype.initPageEvents = function () {
    this.initAddCategoryEvent();
    this.initDeleteCategoryEvent();
    this.initConfirmCatDeleteEvent();
};

event_category_class.prototype.initConfirmCatDeleteEvent = function () {
    var self = this;

    // Confirm deleteion of category from cat table
    $("#ConfirmDeleteEventCategory").click(function() {
        var catIdToDelete = self.$catRowToDelete.find("td.id").text();
        
        var $container = $("#DeleteEventCategoryModal").find("div.wrapper");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/eventcategory/deletecategory",
            type: "POST",
            data: {
                id: catIdToDelete
            },
            success: function (data) {

                // Notify user of success
                var noty_id = noty({ text: 'Category Successfully Deleted.', type: 'success', timeout: 2000 });

                // Remove the row
                self.$catRowToDelete.remove();

                // Hide loader
                common.hideAjaxLoader($container);

                // Close Modal
                $('#DeleteEventCategoryModal').modal('hide');
            },
            error: function (data) {
                // Close Modal
                common.hideAjaxLoader($container);
                $('#DeleteEventCategoryModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });
};

event_category_class.prototype.initDeleteCategoryEvent = function () {
    var self = this;
    self.$catRowToDelete;
    

    // Delete Category from category listing table
    $("#EventCategoriesTable td").on("click", "a.deleteCategoryButton", function () {
        var catId = $(this).attr("data-id");

        // Store the row to be removed so the dialog box can access is
        self.$catRowToDelete = $(this).closest("tr");
        
        // Set the dialog's box's text to give the user some context
        $("#popTitle").text("'" + self.$catRowToDelete.find("td.name").text() + "'");

        // Show confirmation pop up
        $("#DeleteEventCategoryModal").modal();
    });
};

event_category_class.prototype.initAddCategoryEvent = function () {

    // Add Category Button
    $("#ConfirmAddEventCategory").click(function () {
        var name = $("#EventCategoryNameInput").val();
        if (name.length < 1) {
            return false;
        }

        var $container = $("#AddEventCategoryModal").find("div.wrapper");
        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/eventcategory/addcategory",
            type: "POST",
            data: {
                name: name
            },
            success: function (data) {

                // Notify user of success
                var noty_id = noty({ text: 'Category Successfully Created.', type: 'success', timeout: 2000 });

                var $wrapper = $('#event-categories-wrapper');
                common.showAjaxLoader($wrapper);
                $wrapper.load('/admin/eventcategory/manageeventcategories/ #event-categories-wrapper', function () {
                    // Hide loader
                    common.hideAjaxLoader($container);
                    common.hideAjaxLoader($wrapper);
                });

                // Close Modal
                $('#AddEventCategoryModal').modal('hide');
            },
            error: function (data) {
                // Close Modal
                common.hideAjaxLoader($container);
                $('#AddEventCategoryModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });
    });
};


// Keep at the bottom
$(document).ready(function () {
    eventCategoryAdmin = new event_category_class();
    eventCategoryAdmin.initPageEvents();
});