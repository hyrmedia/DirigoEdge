/// ===========================================================================================
/// Schema Editor
/// ===========================================================================================

schemaEditor_class = function () {
    this.id = $(".editContent").data("id");

    // No likey autosave
    Formbuilder.options.AUTOSAVE = false;
};


schemaEditor_class.prototype.initPageEvents = function () {
    var self = this;

    this.initFormBuilder();

    $('#SchemaName').change(function () {
        $('.js-save-form').removeAttr('disabled');
    });

    // Set theme to match Foundation
    $(".js-save-form").addClass("button").text("Save Schema");

    // Set up Save event
    self.schema.on('save', function (data) {

        // Sets children id's as attributes on list items
        self.updateListChildrenAttributes();

        // Run through the save data and set the children id's as field data
        var oData = JSON.parse(data);
        $.each(oData.fields, function (key, value) {

            if (value.field_type == "list") {
                var sChildren = $("[data-prevcid=" + value.cid + "]").attr("data-children") || "";
                var nChildren = sChildren.split(',');

                value.field_options.children = nChildren;
            }
        });

        data = JSON.stringify(oData);
        $.ajax({
            url: "/admin/schemas/saveschema",
            type: "POST",
            data: {
                name: $("#SchemaName").val(),
                data: data,
                id: self.id
            }
        });
    });

    // When adding a new field, make sure we update list items so they can be droppable
    $(".fb-response-fields").on("sortstop", function (event, ui) {
        setTimeout(function () {
            self.initListingItems();
        }, 200);
    });

    $(".fb-add-field-types a").click(function () {
        setTimeout(function () {
            self.initListingItems();
        }, 200);
    });

    // Sticky Edit Window
    var $sidebar = $("#FormBuilder .fb-left"),
        $window = $("#Main"),
        offset = $sidebar.offset(),
        topPadding = 30;

    $window.scroll(function () {

        if ($window.scrollTop() > offset.top - topPadding) {
            $sidebar.addClass('fixed');
        } else {
            $sidebar.removeClass('fixed');
        }
    });
};

schemaEditor_class.prototype.initFormBuilder = function () {

    var self = this;

    this.schema = new Formbuilder({
        selector: '#FormBuilder',
        autosave: false,
        bootstrapData: ODATA.fields
    });

    // Set up any previously saved cid's as attributes in case we need to reference the previous id's
    $("#FormBuilder .fb-field-wrapper").each(function (index, value) {

        if (ODATA.fields.length >= index) {
            var prevCid = ODATA.fields[index].cid;
            $(this).attr("data-prevcid", prevCid);
        }
    });

    // Run through and set up the list items
    if (ODATA && ODATA.hasOwnProperty("fields")) {
        $.each(ODATA.fields, function (key, value) {

            if (value.field_type == "list") {

                var $listItem = self.getElementByPrevCid(value.cid);
                var children = value.field_options.children;

                for (var x = 0; x < children.length; x++) {

                    var $childItem = children[x] ? self.getElementByPrevCid(children[x]) : null;

                    if ($childItem != null) {
                        $listItem.find(".listingContainer").append($childItem);
                    }
                }
            }
        });
    }

    setTimeout(function () {
        self.initListingItems();
        self.initListingSortables();
    }, 200);
};

// Returns a jQuery object based on units .data() "cid" attribute
schemaEditor_class.prototype.getElementByPrevCid = function (cid) {
    return $(".fb-field-wrapper[data-prevcid=" + cid + "]");
};

schemaEditor_class.prototype.initListingItems = function () {
    var self = this;
    //this.schema.mainView.setSortable();

    $(".response-field-list > .subtemplate-wrapper > .cover").remove();

    $(".listingContainer:not(.ui-droppable)").droppable({
        greedy: true,
        hoverClass: "drop-hover",
        accepts: ".fb-button",
        drop: function (event, ui) {

            self.$wrapper = $(this);

            setTimeout(function () {

                // Move the item the sortable just added inside the list item, but only if it isn't already inside
                if (self.$wrapper.find(".editing").length < 1) {

                    self.$wrapper.append($("#FormBuilder .fb-field-wrapper.editing"));
                }

                // Init Sortable on the listing container if hasn't already been initialized
                if (!self.$wrapper.hasClass(".ui-sortable")) {
                    self.initListingSortables(self.$wrapper);
                }

            }, 150);
        }
    });
};

schemaEditor_class.prototype.initListingSortables = function ($el) {

    if ($el == null) {
        $el = $(".listingContainer:not(.ui-sortable)");
    }

    $el.sortable({
        revert: 100,
        start: function () {
            $(this).addClass("sorting");
        },
        stop: function () {
            $(this).removeClass("sorting");
        }
    });
};

schemaEditor_class.prototype.updateListChildrenAttributes = function () {

    $("#FormBuilder .fb-field-wrapper").each(function () {
        // Store the current CID for later selection during save
        $(this).attr("data-current-id", $(this).data('cid'));
    });

    $("#FormBuilder .fb-field-wrapper.response-field-list").each(function () {
        // Grab CID's from children and add them to list attribute
        var nChildrenList = [];
        $(this).find("> .subtemplate-wrapper > .listingContainer > .fb-field-wrapper").each(function () {
            var childCid = $(this).data('cid');

            var newCid = $(".fb-field-wrapper[data-current-id=" + childCid + "]").attr("data-prevcid") || childCid;

            nChildrenList.push(newCid);
        });

        $(this).attr("data-children", nChildrenList.join());
    });
};

// Keep at the bottom
$(document).ready(function () {
    schemaEditor = new schemaEditor_class();
    schemaEditor.initPageEvents();
});