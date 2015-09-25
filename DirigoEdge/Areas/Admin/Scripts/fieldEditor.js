/*
 *  Schema / Field Editor integrated into Module Editor 
*/

fieldEditor_class = function () {
    this.moduleId = $("#Main > div.editContent").attr("data-page") || $("#Main > div.editContent").attr("data-id");
};

fieldEditor_class.prototype.initPageEvents = function () {
    var self = this;

    // Load Schema into Field editor on Schema Change
    $("#SchemaSelector").change(function () {
        var schemaId = $("#SchemaSelector option:selected").attr("data-id");

        // Check for No Schema
        if (schemaId == -1) {
            $("#FieldEntry").html("<p>No Schema assigned.</p>");
            return;
        }

        // If there is a schema, load it
        self.loadSchemaIntoFields(schemaId, self.moduleId);

        // Update the edit schema link
        $("#EditSchemaLink").attr("href", "/admin/editschema/" + schemaId);
    }).trigger("change");

    // Refresh the schema by simply triggering a change on the drop down
    $("#RefreshSchemaLink").click(function () {
        $("#SchemaSelector").trigger("change");
    });

    $('.js-sync-schema-view').click(self.syncSchemaView);

    this.initListEvents();
};

fieldEditor_class.prototype.loadSchemaIntoFields = function (schemaId, moduleId) {
    var self = this;

    var data = {
        schemaId: schemaId,
        moduleId: moduleId,
        isPage: $("#PageTitle").length > 0
    };

    // Set loading spinner
    var $container = $("#FieldEntry");
    common.showAjaxLoader($container);
    $("#RefreshSchemaLink i.fa-refresh").addClass("fa-spin");

    // Kick off ajax to get the schema and values
    $.ajax({
        url: "/admin/schemas/getschemahtml",
        type: "POST",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data, null, 2),
        success: function (data) {
            var obj = jQuery.parseJSON(data.data);

            // Clear the html
            $("#EditorTab").html("<div id='FieldEditorContainer'></div>");

            // Build out the fields into html and then place them on the page
            var $content = self.buildSchemaContent(obj.fields);
            $("#FieldEditorContainer").append($content);

            // Move any list items' children up to it's parent
            self.moveChildrenToList(obj.fields);

            // Set the user values into the newly placed fields
            var schemaData = jQuery.parseJSON(data.values);

            if (schemaData != null) {
                self.loadSchemaValues(schemaData);
            }

            self.initSpecialFields();

            // End Loading
            $("#RefreshSchemaLink i.fa-refresh").removeClass("fa-spin");
            common.hideAjaxLoader($container);
        },
        error: function (data) {
            alert("An Error Occurred. Please refresh and try again.");
        }
    });
};

fieldEditor_class.prototype.initSpecialFields = function ($parent) {

    $parent = $parent || $('#FieldEntry');

    // Set up Image Uploaders
    $('a.upload', $parent).fileBrowser({
        directory: "",
        isEditor: false
    }, function (object) {
        var path = object.src || object.href,
            $input = $(object.elem).parent().find("input[type=text]");

        $input.val(path);
        return false;
    });

    // Set up Date Pickers
    $('.date input', $parent).datepicker();

    // Setup CKEditors
    // Timeout needed for CK Bug
    setTimeout(function () {
        $('textarea.editor', $parent).each(function () {
            var id = $(this).attr("id");
            var $el = $("#" + id);

            if (!$el.hasClass("activated")) {
                CKEDITOR.replace(id);
                $el.addClass("activated");
            }
        });
    }, 300);
}

fieldEditor_class.prototype.loadSchemaValues = function (schemaValues) {

    $.each(schemaValues, function (key, value) {

        var label = key;
        var inputName = key.toLowerCase().replace(/ /g, '-');

        //text inputs
        $("#FieldEntry input[name='" + inputName + "']").val(value);

        // Text Area / Paragraphs
        $("#FieldEntry textarea[name='" + inputName + "']").val(value);

        // Select Boxes
        $("#FieldEntry select[name='" + inputName + "']").val(value);

        // Checkboxes
        $("#FieldEntry div.checkContainer[data-label='" + label + "']").each(function () {
            var nValues = value;
            var $this = $(this);

            $.each(nValues, function (key, val) {

                var $input = $this.find("input[data-label='" + val + "']");
                if ($input.length > 0) {
                    $input.prop("checked", true);
                }
                else {
                    // Other / Specify
                    $this.find("input.otherInput").prop("checked", true);
                    $this.find("input.otherSpecifyInput").val(val);
                }
            });
        });

        // Radio Buttons
        $("#FieldEntry div.radioContainer[data-label='" + label + "']").each(function () {
            var $input = $(this).find("input[data-label='" + value + "']");
            if ($input.length > 0) {
                $input.prop("checked", true);
            }
            else {
                // Other / Specify
                $(this).find("input.otherInput").prop("checked", true);
                $(this).find("input.otherSpecifyInput").val(value);
            }
        });


        $("#FieldEntry div.formContainer.list[data-label='" + label + "']").each(function () {

            var $this = $(this);

            var nValues = value[0].values;

            $.each(nValues, function (innerKey, innerValue) {

                var $listContainer = $this.find(" > li > .listContainer")[innerKey];

                // If it doesn't exist add it
                if (typeof $listContainer == "undefined" || $listContainer.length < 1) {
                    var $new = $this.find(" > li > .listContainer").last().clone(),
                        $newRadios = $new.find('.radioContainer, .checkboxContainer');

                    // Radio button/checkbox groups require unique names
                    $newRadios.each(function () {

                        var $input = $(this).find('input[type="radio"], input[type="checkbox"]'),
                            name = $input.attr("name") + Date.now();

                        $input.each($input, function () {
                            var id = $(this).attr("id") + Date.now();

                            $(this).attr("name", name);
                            $(this).attr("id", id);
                        });

                    });

                    $this.find(" > li > .listContainer").last().after($new);

                    $listContainer = $this.find(" > li > .listContainer")[innerKey];
                }

                $($listContainer).find("[data-label='" + innerValue[0] + "']").val(innerValue[1]);

                // Load up the values. This is where recursion should happen for lists within lists
                for (var propt in innerValue) {
                    $($listContainer).find("[data-label='" + propt + "']").val(innerValue[propt]);
                }
            });
        });
    });
};

fieldEditor_class.prototype.buildSchemaContent = function (fields) {
    var self = this;
    var $content = $("<form id='FieldEntry'>");

    // loop through the fields
    $.each(fields, function (index, value) {
        var oItem = value;
        var label = oItem.label;
        var inputName = label.toLowerCase().replace(/ /g, '-'); // lowercase, replace spaces with dashes
        var type = oItem.field_type;
        var bRequired = oItem.required;
        var sRequiredTokend = (bRequired) ? " <span class='requiredToken'>*</span>" : "";
        var cId = oItem.cid;
        var requiredClass = (bRequired) ? "required" : "";
        var options = oItem.field_options;
        var description = options.description || "";
        var sizeClass = options.size || "";
        var includeOther = options.include_other_option || false;
        var schemaId = options.schema_id || 0;

        // Text Fields
        if (type == "text") {

            var $formContainer = $("<div class='formContainer text " + sizeClass + "'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            $formContainer.append("<input name='" + inputName + "' data-label='" + label + "' type='text' class='form-control " + requiredClass + "' data-id='" + cId + "' autocomplete='off' />");

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // Paragraph / Text Area Fields
        if (type == "paragraph") {
            var $formContainer = $("<div class='formContainer paragraph " + sizeClass + "'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            $formContainer.append("<textarea name='" + inputName + "' data-label='" + label + "' type='text' class='form-control " + requiredClass + "' data-id='" + cId + "' autocomplete='off' ></textarea>");

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // Image Uploader Fields
        if (type == "image") {

            var $formContainer = $("<div class='formContainer image " + sizeClass + "'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            $formContainer.append("<input name='" + inputName + "' data-label='" + label + "' type='text' class='form-control " + requiredClass + "' data-id='" + cId + "' autocomplete='off' />");
            $formContainer.append("<a class='upload secondary btn btn-primary btn-sm'><i class='fa fa-picture-o'></i> Select Image...</a>");

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // WYSIWYG Editor 
        if (type == "wysiwyg") {
            var $formContainer = $("<div class='formContainer wysiwyg " + sizeClass + "'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            $formContainer.append("<textarea name='" + inputName + "' id='" + cId + "' data-label='" + label + "' type='text' class='editor " + requiredClass + "' data-id='" + cId + "' autocomplete='off' ></textarea>");

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // Multiple Choice / Radio Buttons
        if (type == "radio") {

            var $formContainer = $("<div class='formContainer radioContainer" + sizeClass + "' data-label='" + label + "'>");
            $formContainer.append("<label>" + label + "</label>");

            // Add Radio Options
            $.each(options.options, function (key, value) {
                var radioLabel = value.label;
                var radioId = value.label.toLowerCase().replace(/ /g, '-'); // lowercase, replace spaces with dashes
                var checked = value.checked == true ? "checked=checked" : "";
                var radioName = label.toLowerCase().replace(/ /g, '-');

                var $radioContainer = $("<div class='radioSingle' />");

                var inputHtml = "<input id='" + radioId + "'  name='" + radioName + "' type='radio' data-label='" + radioLabel + "' class='" + requiredClass + "' data-id='" + cId + "' autocomplete='off' " + checked + " />";
                $radioContainer.append("<label for='" + radioId + "' class='radioLabel'>" + inputHtml + " " + radioLabel + "</label>");

                $formContainer.append($radioContainer);
            });

            // Add Other choice if necessary
            if (includeOther) {
                var checkId = inputName + "OtherRadio";
                var checkName = label.toLowerCase().replace(/ /g, '-');
                var checkLabel = "Other";
                var checked = "";

                var $checkContainer = $("<div class='radioSingle' />");

                var inputHtml = "<input id='" + checkId + "'  name='" + checkName + "' type='radio' data-label='" + checkLabel + "' class='otherInput " + requiredClass + "' data-id='" + cId + "' autocomplete='off' " + checked + " />";
                $checkContainer.append("<label for='" + checkId + "' class='radioLabel'>" + inputHtml + " " + checkLabel + "</label>");
                $checkContainer.append('<input type="text" class="otherSpecifyInput" autocomplete="off" />');

                $formContainer.append($checkContainer);
            }

            $content.append($formContainer);
        }

        // CheckBoxes
        if (type == "checkboxes") {

            var $formContainer = $("<div class='formContainer checkContainer" + sizeClass + "' data-label='" + label + "'>");
            $formContainer.append("<label>" + label + "</label>");

            // Add CheckBox Options
            $.each(options.options, function (key, value) {
                var checkLabel = value.label;
                var checkId = value.label.toLowerCase().replace(/ /g, '-'); // lowercase, replace spaces with dashes
                var checked = value.checked == true ? "checked=checked" : "";
                var checkName = label.toLowerCase().replace(/ /g, '-');

                var $checkContainer = $("<div class='checkSingle' />");

                var inputHtml = "<input id='" + checkId + "'  name='" + checkName + "' type='checkbox' data-label='" + checkLabel + "' class='" + requiredClass + "' value='test' data-id='" + cId + "' autocomplete='off' " + checked + " />";
                $checkContainer.append("<label for='" + checkId + "' class='radioLabel'>" + inputHtml + " " + checkLabel + "</label>");

                $formContainer.append($checkContainer);
            });

            // Add Other choice if necessary
            if (includeOther) {
                var checkId = inputName + "OtherCheck";
                var checkName = inputName + "Check1";
                var checkLabel = "Other";
                var checked = "";

                var $checkContainer = $("<div class='checkSingle' />");

                var inputHtml = "<input id='" + checkId + "'  name='" + checkName + "' type='checkbox' data-label='" + checkLabel + "' class='otherInput " + requiredClass + "' value='test' data-id='" + cId + "' autocomplete='off' " + checked + " />";
                $checkContainer.append("<label for='" + checkId + "' class='radioLabel'>" + inputHtml + " " + checkLabel + "</label>");
                $checkContainer.append('<input type="text" class="otherSpecifyInput"/>');

                $formContainer.append($checkContainer);
            }

            $content.append($formContainer);
        }

        // Select Boxes / Dropdown
        if (type == "dropdown") {
            var $formContainer = $("<div class='formContainer dropdown'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            var $select = $("<select class='form-control' name='" + inputName + "' data-label='" + label + "' data-id='" + cId + "'></select>");

            $.each(options.options, function (key, value) {

                var label = value.label;
                var checked = value.checked;
                var selectedText = checked ? "selected=selected" : "";

                $select.append("<option " + selectedText + " >" + label + "</option>");
            });

            $formContainer.append($select);

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // Module list
        if (type == "module") {
            var $formContainer = $("<div class='formContainer module'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            var $select = $("<select class='form-control' name='" + inputName + "' data-label='" + label + "' data-id='" + cId + "'></select>");

            $.ajax({
                async: false,
                url: "/admin/schemas/getlistofmodulesbyschemaid/" + schemaId,
                type: "GET",
                success: function (data) {
                    $(data.moduleNames).each(function () {
                        $select.append("<option>" + this + "</option>");
                    });
                }
            });

            $formContainer.append($select);

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // Datepicker Fields
        if (type == "date") {

            var $formContainer = $("<div class='formContainer date small'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            $formContainer.append("<input name='" + inputName + "' data-label='" + label + "' type='text' class='form-control " + requiredClass + "' data-id='" + cId + "' autocomplete='off' />");
            $formContainer.append("<i class='fa fa-calendar'></i>");

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }

        // Listings
        if (type == "list") {

            var formControls = "<div class='listControls'><i class='fa fa-ellipsis-v'></i><i class='remove fa fa-trash-o'></i></div>";
            var $formContainer = $("<div class='formContainer list " + sizeClass + "' data-label='" + label + "'>");

            $formContainer.append("<label>" + label + sRequiredTokend + "</label>");
            $formContainer.append("<div class='listContainer' data-id='" + cId + "' data-label='" + label + "'> " + formControls + "</div>");

            if (description.length > 0) {
                $formContainer.append("<p class='labelDescription'>" + description + "</p>");
            }

            $content.append($formContainer);
        }
    });

    // Wrap each form container in list items for better viewing
    $content.find("div.formContainer").wrapAll("<ol></ol>").wrapInner("<li></li>");

    return $content;
};

fieldEditor_class.prototype.moveChildrenToList = function (fields) {

    $.each(fields, function (index, value) {
        if (value.field_type == "list") {
            var $listItem = $("[data-id=" + value.cid + "]");

            var nChildren = value.field_options.children;
            for (var x = 0; x < nChildren.length; x++) {

                var $child = $("[data-id=" + nChildren[x] + "]");

                // move it
                if ($child.length > 0) {
                    var $childContainer = $child.closest(".formContainer");

                    $listItem.append($childContainer);
                }
            }

            // Add an "Add" button to the list if it's not already there
            $listItem.after("<a class='small secondary btn btn-primary btn-sm newListItem ' href='#'>New " + value.label + " +</a>");
        }
    });

    // Make them sortable
    $(".formContainer.list ").sortable({
        items: ".listContainer",
        handle: ".listControls",
        revert: 100
    });
};

fieldEditor_class.prototype.syncSchemaView = function () {

    var self = this,
        $self = $(self),
        schemaId = $self.attr('data-schema'),
        itemId = $self.attr('data-item'),
        ispage = $self.attr('data-ispage'),
        itemList = [],
        updateContentItem = function (page, syncData, callback) {

            var parsedHtml = Mustache.to_html(syncData.html, JSON.parse(page.schemaValues)),
                data = {
                    itemId: page.id,
                    unparsedHtml: syncData.html,
                    parsedHtml: parsedHtml,
                    template: syncData.template,
                    parent: syncData.parent,
                    ispage: ispage
                };

            $self.siblings('.sync-counter').first().text('Syncing ' + (itemList.indexOf(page) + 1) + ' of ' + itemList.length);

            $.ajax({
                url: "/admin/schemas/syncschemaview/",
                type: "POST",
                dataType: 'json',
                data: JSON.stringify(data, null, 2),
                contentType: 'application/json; charset=utf-8',
                success: function (data) {

                    if (!data.success) {
                        noty({ text: data.error, type: 'error' });
                        return false;
                    }

                    if (itemList.indexOf(page) === itemList.length - 1 && typeof callback === 'function') {
                        callback();
                    } else {
                        updateContentItem(itemList[itemList.indexOf(page) + 1], syncData, callback);
                    }

                }
            });

        };

    $self.addClass('is-syncing');

    $.ajax({
        url: "/admin/schemas/getschemasyncdata/?schemaid=" + schemaId + "&itemid=" + itemId + "&ispage=" + ispage,
        type: "GET",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data) {

            itemList = data.items;
            $self.after('<div class="sync-counter"></div>');

            updateContentItem(itemList[0], data, function () {

                $self.removeClass('is-syncing');
                $self.siblings('.sync-counter').remove();
                noty({ text: "Items successfully synced.", type: 'success' });

            });

        }
    });

    return false;

};

fieldEditor_class.prototype.initListEvents = function () {

    var self = this;

    $("#EditorTab").on("click", ".newListItem", function (e) {
        e.preventDefault();
        var $listContainer = $(this).prev().clone();
        $listContainer.addClass("animated zoomIn");
        $listContainer.attr("data-id", self.generateCId());

        $(this).before($listContainer);

        // No longer need the class
        setTimeout(function () {
            $(".animated.zoomIn").removeClass("animated zoomIn");
        }, 400);

        console.log($(this).closest('.formContainer'));

        self.initSpecialFields($(this).closest('.formContainer'));
    });

    $("#EditorTab").on("click", ".listContainer .remove", function (e) {
        e.preventDefault();

        var $listContainer = $(this).closest(".listContainer");

        if ($listContainer.closest("li").find(".listContainer").length < 2) {
            alert("At least one list item must be present.");
            return false;
        }

        if (confirm("Delete this list item?")) {
            $listContainer.remove();
        }
    });
};

fieldEditor_class.prototype.generateCId = function () {

    // if duplicate, increment till we find a free spot
    var count = 0;
    var id = "c" + count;
    var $frame = $("#FieldEntry");
    while ($frame.find("[data-id=" + id + "]").length > 0) {
        id = "c" + count;
        count++;
    }
    return id;
};

// Keep at the bottom
$(document).ready(function () {
    fieldEditor = new fieldEditor_class();
    fieldEditor.initPageEvents();
});