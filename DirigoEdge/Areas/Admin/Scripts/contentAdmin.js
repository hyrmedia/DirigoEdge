/// ===========================================================================================
/// Content Admin Class. In charge of manage content, edit content for Modules and Pages
/// ===========================================================================================

content_class = function () {

};

content_class.prototype.initPageEvents = function () {

    this.manageContentAdminEvents();

    if ($("div.editContent").length > 0 && typeof (ace) != "undefined") {
        this.initCodeEditorEvents();
        this.initWordWrapEvents();
        this.initContentImageUploadEvents();
        this.initModuleUploadEvents();
    }

    if ($("div.manageContent").length > 0) {
        this.initDeleteContentEvent();
        this.initNewPageModalEvents();
    }

    if ($("div.manageModule").length > 0) {
        this.initDeleteModuleEvent();
        this.initNewPageModalEvents();
    }

    if ($("div.manageSchemas").length > 0) {
        this.initDeleteSchemaEvent();
    }

    // View / Act on Revisions
    this.initRevisionEvents();

    // Generate Url Structure
    this.initParentCategoryEvents();
};

content_class.prototype.initWordWrapEvents = function () {
    var self = this;

    $("#WordWrap").change(function () {
        var wrapWords = $(this).is(":checked");

        self.htmlEditor.getSession().setUseWrapMode(wrapWords);

        // Let's help out our fellow coders and save their settings so they don't check/uncheck every time
        $.ajax({
            url: "/admin/pages/setwordwrap",
            type: "POST",
            data: {
                wordWrap: wrapWords
            }
        });
    });
};

content_class.prototype.initDeleteModuleEvent = function () {
    var self = this;

    // Delete Module
    $("div.manageModule table.manageTable td a.delete").click(function () {
        self.managePageId = $(this).attr("data-id");

        self.$managePageRow = $(this).closest('tr');
        var title = '"' + self.$managePageRow.find("td.title a").text() + '"';
        $("#popTitle").text(title);
        $("#DeleteModal").modal();
    });

    // Confirm Delete Content
    $("#ConfirmModuleDelete").click(function () {
        var id = self.managePageId;
        $.ajax({
            url: "/admin/modules/deletemodule",
            type: "POST",
            data: {
                id: self.managePageId
            },
            success: function (data) {
                var noty_id = noty({ text: 'Module Successfully Deleted.', type: 'success', timeout: 2000 });
                self.$managePageRow.remove();
                $('#DeleteModal').modal('hide');
            },
            error: function (data) {
                $('#DeleteModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};

content_class.prototype.refreshTable = function (containerClass, url) {
    //Refresh the inner content to show the new user
    var $container = $(containerClass);

    common.showAjaxLoader($container);
    $container.load(url + ' ' + containerClass, function (data) {
        common.hideAjaxLoader($container);
        if (!$.fn.dataTable.isDataTable('.manageTable')) {
            window.oTable = $("table.manageTable").dataTable({
                "iDisplayLength": 25,
                "aoColumnDefs": [
                    {
                        "bSortable": false,
                        "aTargets": ["actions"]
                    } // No Sorting on actions
                ],
                "aaSorting": [[0, "asc"]] // Sort by module name on load
            });
        }
    });
}

content_class.prototype.initDeleteContentEvent = function () {
    var self = this;
    $("div.manageContent table.manageTable td a.delete").click(function () {
        self.managePageId = $(this).attr("data-id");

        self.$managePageRow = $(this).parent().parent();
        var title = '"' + self.$managePageRow.find("td.title a").text() + '"';
        $("#popTitle").text(title);
        $("#DeleteModal").modal();
    });

    // Confirm Delete Content
    $("#ConfirmContentDelete").click(function () {
        var id = self.managePageId;
        $.ajax({
            url: "/admin/pages/deletecontent",
            type: "POST",
            data: {
                id: self.managePageId
            },
            success: function (data) {
                var noty_id = noty({ text: 'Content Page Successfully Deleted.', type: 'success', timeout: 2000 });
                self.$managePageRow.remove();
                $('#DeleteModal').modal('hide');
            },
            error: function (data) {
                $('#DeleteModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};

content_class.prototype.initDeleteSchemaEvent = function () {
    var self = this;
    $('body').on('click', '.delete-schema', function () {
        self.managePageId = $(this).attr("data-id");

        self.$managePageRow = $(this).closest('tr');
        var title = '"' + self.$managePageRow.find("td.title a").text() + '"';
        $("#popTitle").text(title);
        $("#DeleteModal").modal();
    });

    // Confirm Delete Content
    $("#ConfirmSchemaDelete").click(function () {
        var id = self.managePageId;
        $.ajax({
            url: "/admin/schemas/deleteschema",
            type: "POST",
            data: {
                id: self.managePageId
            },
            success: function (data) {
                var noty_id = noty({ text: 'Schema Successfully Deleted.', type: 'success', timeout: 2000 });
                self.$managePageRow.remove();
                $('#DeleteModal').modal('hide');
            },
            error: function (data) {
                $('#DeleteModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};

content_class.prototype.initNewPageModalEvents = function () {
    var self = this;

    // Pop Modal in Admin for New page if page builder enabled
    $("#NewContentPage").click(function (e) {
        if ($(this).hasClass("useTemplateSelector")) {
            e.preventDefault();
            $('#NewPageModal .modal-body .error').addClass('hide').html('');
            $("#NewPageModal").modal();
        }
    });

    // Click on template to highlight / select it
    $("#NewPageModal div.newPageTemplateListing ul li a").click(function () {
        $("#NewPageModal div.newPageTemplateListing ul li a.active").removeClass("active");
        $(this).addClass("active");
    });

    // set permalink based on title keyup
    $("#PageTitle").keyup(function () {
        var val = self.formatContentLink($(this).val());
        $("#PagePermalink").val(val);
    });

    // Click create in modal to create new page and then be brought to new content to start modifying
    $("#ConfirmCreatePageButton").click(function () {

        var title = $("#PageTitle").val(),
            permalink = $("#PagePermalink").val(),
            parent = parseInt($("#ParentPage option:selected").attr('data-id'));

        if (title.length < 1 || permalink.length < 1) {
            $('#NewPageModal .modal-body .user-error').removeClass('hide').html('Page Title and Permalink (the url) are required.');
            return false;
        }

        var viewTemplate = $("#NewPageModal div.newPageTemplateListing a.active").attr("data-view-template"),
            templatePath = $("#NewPageModal div.newPageTemplateListing a.active").attr("data-template-path"),
            $container = $("#NewPageModal div.content");


        common.showAjaxLoader($container);
        $.ajax({
            url: "/admin/pages/addnewpagefromtemplate",
            type: "POST",
            data: {
                viewTemplate: viewTemplate,
                templatePath: templatePath,
                permalink: permalink,
                title: title,
                parent: parent
            },
            success: function (data) {
                if (data.success) {
                    self.afterNewPageCreate(data);
                } else {
                    $('#NewPageModal .modal-body .user-error')
                        .removeClass('hide')
                        .html(data.message);
                    common.hideAjaxLoader($container);
                    return false;
                }
            },
            error: function (data) {
                $('#NewUserModal').modal('hide');
                common.hideAjaxLoader($container);
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};

content_class.prototype.afterNewPageCreate = function (data) {

    console.log('Edge version');

    // Redirect to code editor
    window.location = '/admin/pages/editcontent/' + data.id + '/';

};

content_class.prototype.initCodeEditorEvents = function () {
    var self = this;

    // Init Code Editor
    var theme = $("#EditorTheme :selected").val();
    self.htmlEditor = ace.edit("HTMLContent");
    self.htmlEditor.setTheme(theme);
    self.htmlEditor.getSession().setMode("ace/mode/html");
    self.htmlEditor.getSession().setUseSoftTabs(true);
    self.htmlEditor.getSession().setUseWrapMode(true);
    self.htmlEditor.setShowInvisibles(true);

    // Switch to CSS
    self.htmlEditor.commands.addCommand({
        name: 'switchTab',
        bindKey: { win: 'Ctrl-2', mac: 'Command-2' },
        exec: function (editor) {
            $("a[href=#CSS]").trigger("click");
            $("#CSSContent textarea").focus();
        }
    });
    // Switch to JS
    self.htmlEditor.commands.addCommand({
        name: 'switchTab',
        bindKey: { win: 'Ctrl-3', mac: 'Command-3' },
        exec: function (editor) {
            $("a[href=#JS]").trigger("click");
            $("#JSContent textarea").focus();
        }
    });
    // Save
    self.htmlEditor.commands.addCommand({
        name: 'Save',
        bindKey: { win: 'Ctrl-S', mac: 'Command-S' },
        exec: function (editor) {
            $("#SaveContentButton").trigger("click");
        }
    });

    self.cssEditor = ace.edit("CSSContent");
    self.cssEditor.setTheme(theme);
    self.cssEditor.getSession().setMode("ace/mode/css");
    // Temp test
    self.cssEditor.commands.addCommand({
        name: 'switchTab1',
        bindKey: { win: 'Ctrl-1', mac: 'Command-1' },
        exec: function (editor) {
            $("a[href=#Html]").trigger("click");
            $("#HTMLContent textarea").focus();
        },
        readOnly: true // false if this command should not apply in readOnly mode
    });

    self.jsEditor = ace.edit("JSContent");
    self.jsEditor.setTheme(theme);
    self.jsEditor.getSession().setMode("ace/mode/javascript");

    // Change editor Theme
    $("#EditorTheme").change(function () {
        var theme = $(this).val();
        self.htmlEditor.setTheme(theme);
        self.cssEditor.setTheme(theme);
        self.jsEditor.setTheme(theme);
    });
};

content_class.prototype.initContentImageUploadEvents = function () {
    var self = this;

    $('#InsertContentImage').fileBrowser(function (object) {
        var tag;

        if (object.type === 'image') {
            tag = object.responsive
                    ? '[responsive_image src="' + object.src + '" alt="' + object.alt + '" width="' + object.width + '" height="' + object.height + '"]'
                    : '<img src="' + object.src + '" alt="' + object.alt + '" />';
        } else {
            tag = '<a href="' + object.href + '" title="' + object.title + '" >' + object.text + '</a>';
        }

        // Insert the tag into the editor
        self.htmlEditor.insert(tag);

        // Highlight the newly placed tag
        self.htmlEditor.find(tag, { backwards: true, });
    });
};

content_class.prototype.initModuleUploadEvents = function () {

    $('#ChangeImageThumbnail').fileBrowser(function (object) {
        if (object.type === 'image' && object.src) {
            $("#ModuleThumbnail").val(object.src);
            $("#ImageModuleThumbnail").attr("src", object.src);
        }
    });

    // Key up refreshes thumbnail
    $("#ModuleThumbnail").keyup(function () {
        var src = $(this).val();
        $("#ImageModuleThumbnail").attr("src", src);
    });
};

content_class.prototype.manageContentAdminEvents = function () {
    var self = this;

    // WYSIWYG Editor
    if ($('#CKEDITPAGE').length > 0) {
        self.CKPageEditor = CKEDITOR.replace('CKEDITPAGE');
    }
    else if ($('#CKEDITCONTENT').length > 0) {
        self.CKPageEditor = CKEDITOR.replace('CKEDITCONTENT', {
            // options here
            height: 430
        });
    }

    // Save Content Button
    $("#SaveContentButton").click(function () {

        // Make sure Page Title exists
        if ($("#PageTitle").val().length < 1) {
            alert("You must enter a page title before saving.");
            return false;
        }

        var data = self.getPageData();

        $("#SaveSpinner").show();
        var url = $(this).attr("data-url") || 'ModifyContent';
        $.ajax({
            url: "/admin/pages/" + url,
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                noty({ text: 'Changes saved successfully.', type: 'success', timeout: 1200 });
                $("#SaveSpinner").hide();

                self.setPublishedStatusState(true);

                $("#PublishedDate").text(data.publishDate);

                // Update "Preview" button to use the just-saved / published id
                $("#PreviewContentButton").attr("href", common.updateURLParameter($("#PreviewContentButton").attr("href"), "id", $("div.editContent").attr("data-id")));

                // Refresh Revisions list
                self.refreshRevisionListing();
            },
            error: function (data) {
                noty({ text: 'There was an error processing your request.', type: 'error' });
                $("#SaveSpinner").hide();
            }
        });
    });

    // Save Module Button
    $("#SaveModuleButton").click(function () {

        var data = self.getModuleData();

        $("#SaveSpinner").show();
        var url = $(this).attr("data-url") || 'modifycontent';
        $.ajax({
            url: "/admin/modules/" + url,
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (result) {
                noty({ text: result.message, type: result.success ? 'success' : 'alert', timeout: 1200 });
                $("#SaveSpinner").hide();
                $("#StatusLabel").text("Published");
                $("#PublishedDate").text(result.date);
                self.setPublishedStatusState(true);
                // Refresh Revisions list
                self.refreshRevisionListing();

            },
            error: function (data) {
                noty({ text: 'There was an error processing your request.', type: 'error', timeout: 5000 });
                $("#SaveSpinner").hide();
            }
        });
    });

    $("#ExportModuleButton").click(function () {
        console.log('export module ' + $("div.editContent").attr("data-id"));
        EDGE.ajaxGet(null, "/admin/modules/getmodule/" + $("div.editContent").attr("data-id"), function (data) { console.log(data); });
    });

    // Save a draft
    $("#SaveDraftButton").click(function () {

        var data = self.getPageData();

        $("#SaveSpinner").show();
        $.ajax({
            url: "/admin/pages/savedraft",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                $("#SaveSpinner").hide();

                // Update "Preview" button to use the just-saved id
                $("#PreviewContentButton").attr("href", common.updateURLParameter($("#PreviewContentButton").attr("href"), "id", data.id));

                self.setPublishedStatusState(false);

                // Refresh Revisions list
                self.refreshRevisionListing();
            },
            error: function (data) {
                noty({ text: 'There was an error processing your request.', type: 'error' });
                $("#SaveSpinner").hide();
            }
        });
    });

    $("#SaveModuleDraftButton").click(function () {

        var data = self.getPageData();

        $("#SaveSpinner").show();
        $.ajax({
            url: "/admin/modules/savedraft",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {

                $("#SaveSpinner").hide();


                self.setPublishedStatusState(false);

                // Refresh Revisions list
                self.refreshRevisionListing();
            },
            error: function (data) {
                noty({ text: 'There was an error processing your request.', type: 'error' });
                $("#SaveSpinner").hide();
            }
        });
    });

    // Toggle Draft Status Container
    $("#StatusLabel, #CloseDraftStatus").click(function () {
        $("#DraftStatusContainer").slideToggle();
    });

    // Change Draft Status
    $("#DraftStatusSelector").change(function () {
        var isActive = $("#DraftStatusSelector option:selected").val() == "published";
        var data = {
            entity: {
                ContentPageId: $("div.editContent").attr("data-id"),
                IsActive: isActive
            }
        };

        $("#SaveSpinner").show();
        $.ajax({
            url: "/admin/pages/changedraftstatus",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                $("#SaveSpinner").hide();

                self.setPublishedStatusState(isActive);

                // Update publish date if just switched to publish
                if (isActive) {
                    $("#PublishedDate").text(data.publishDate);
                }
            },
            error: function (data) {
                noty({ text: 'There was an error processing your request.', type: 'error' });
                $("#SaveSpinner").hide();
            }
        });
    });

    // View Permalink in new tab
    $("#ViewPermaLink").click(function (e) {
        e.preventDefault();
        window.open($("#SiteUrl").text() + $("#PermaLinkEnd").text() + "/",
            '_blank');
    });
};


content_class.prototype.getModuleData = function () {
    var self = this;

    var htmlContent;
    var cssContent;
    var jsContent;
    var isBasic;

    // basic editor
    if (typeof (self.htmlEditor) == "undefined") {
        htmlContent = CKEDITOR.instances.CKEDITCONTENT.getData();
        isBasic = true;
    }
    else {
        // advanced editor
        htmlContent = self.htmlEditor.getValue();
        cssContent = self.cssEditor.getValue();
        jsContent = self.jsEditor.getValue();
        isBasic = false;
    }
    var htmlFormatted = self.parseSchemaContent(htmlContent);

    var data = {
        entity: {
            ContentModuleId: $("div.editContent").attr("data-id"),
            ModuleName: $("#ModuleName").val(),
            HTMLContent: htmlFormatted,
            HTMLUnparsed: isBasic ? htmlFormatted : self.htmlEditor.getValue(),
            JSContent: jsContent,
            CSSContent: cssContent,
            SchemaId: $("#SchemaSelector option:selected").attr("data-id"),
            SchemaEntryValues: JSON.stringify(self.getSchemaValues("#FieldEntry > ol > ", false))
        },
        isBasic: isBasic
    };

    return data;
};

content_class.prototype.getPageData = function () {
    var self = this;

    var htmlContent;
    var cssContent;
    var jsContent;
    var template = $("#ContentTemplateSelect option:selected").val();
    var isBasic;

    // basic editor
    if (typeof (self.htmlEditor) == "undefined") {
        htmlContent = CKEDITOR.instances.CKEDITCONTENT.getData();
        isBasic = true;
    }
    else {
        // advanced editor
        htmlContent = self.htmlEditor.getValue();
        cssContent = self.cssEditor.getValue();
        jsContent = self.jsEditor.getValue();
        isBasic = false;
    }
    var htmlFormatted = self.parseSchemaContent(htmlContent);

    var data = {
        page:
        {
            details: {
                ContentPageId: $("div.editContent").attr("data-id"),
                DisplayName: $("#ContentName").val(),
                Permalink: $("#PermaLinkEnd").text().toLowerCase(),
                Description: $("#ModuleDescription").val(),
                ThumbnailLocation: $("#ModuleThumbnail").val(),
                HTMLContent: htmlFormatted,
                HTMLUnparsed: isBasic ? htmlFormatted : self.htmlEditor.getValue(),
                JSContent: jsContent,
                CSSContent: cssContent,
                Template: template,
                PublishDate: $("#PublishDate").val(),
                Title: $("#PageTitle").val(),
                MetaDescription: $("#MetaDescription").val(),
                OGTitle: $("#OGTitle").val(),
                OGImage: $("#OGImage").val(),
                OGType: $("#OGType").val(),
                OGUrl: $("#OGUrl").val(),
                Canonical: $("#Canonical").val(),
                ParentNavigationItemId: $("#PageCategory option:selected").attr("data-id"),
                SchemaId: $("#SchemaSelector option:selected").attr("data-id"),
                SchemaEntryValues: JSON.stringify(self.getSchemaValues("#FieldEntry > ol > ", false)),
                NoIndex: $('.no-index').is(':checked'),
                NoFollow: $('.no-follow').is(':checked')
            }
        },
        // Let Ajax handler know if we're using an advanced editor or baszzic editor
        // Basic editor does not send over JS / CSS rules so we should leave the content as is in the controller
        isBasic: isBasic
    };

    return data;
};

// Returns an object with values
// @forMustache -- Set to true to format data for mustache templating. This mostly effects list types in that it won't wrap in a "values" object parameter
content_class.prototype.getSchemaValues = function (selector, forMustache) {
    var self = this;

    // Key Value Pairs of labels / selected value
    var oValues = {};

    // Text , Image, and Paragraph / TextArea Values, DropDown
    $(selector + "div.formContainer.text input[type=text], " +
        selector + "div.formContainer.image input[type=text], " +
        selector + "div.formContainer.dropdown select, " +
        selector + "div.formContainer.module select, " +
        selector + "div.formContainer.paragraph textarea, " +
        selector + "div.formContainer.date input[type=text]").each(function () {

            var label = $(this).attr("data-label");
            var value = $(this).val();

            oValues[label] = value;
        });

    // WYSIWYG Editors
    $(selector + "div.formContainer.wysiwyg textarea").each(function () {
        var id = $(this).attr("id");
        var label = $(this).attr("data-label");
        var value = CKEDITOR.instances[id].getData();

        oValues[label] = value;
    });

    // Radio Boxes
    $(selector + "div.radioContainer").each(function () {
        var label = $(this).attr("data-label");
        var $radioChecked = $(this).find("input[type=radio]:checked");
        var value = $(this).find("input[type=radio]:checked").attr("data-label");

        if ($radioChecked.hasClass("otherInput")) {
            value = $radioChecked.closest(".radioSingle").find("input.otherSpecifyInput").val();
        }

        oValues[label] = value;
    });

    // Check Boxes
    $(selector + "div.checkContainer").each(function () {
        var label = $(this).attr("data-label");
        var values = [];
        $(this).find("input[type='checkbox']:checked").each(function () {

            var inputValue = "";
            if ($(this).hasClass("otherInput")) {
                inputValue = $(this).closest(".checkSingle").find("input.otherSpecifyInput").val();
            }
            else {
                inputValue = $(this).attr("data-label");
            }

            values.push(inputValue);
        });

        oValues[label] = values;
    });

    // List items
    $(selector + "div.formContainer.list").each(function () {

        var label = $(this).attr("data-label");
        var values = [];

        var thisId = $(this).attr("id") || self.generateListId();
        $(this).attr("id", thisId);

        // Scope next iteration to children of current listing container
        var newValues = [];
        $("#" + thisId + " > li > .listContainer").each(function () {
            var thisId = $(this).attr("id") || self.generateListId();
            $(this).attr("id", thisId);
            var newSelector = "#" + thisId + " > ";

            newValues.push(self.getSchemaValues(newSelector, forMustache));
        });

        var listValues = {
            id: thisId,
            values: newValues
        };

        values.push(listValues);

        if (forMustache) {
            oValues[label] = newValues;
        }
        else {
            oValues[label] = values;
        }
    });

    return oValues;
};

content_class.prototype.generateListId = function () {

    // if duplicate, imcrement till we find a free spot
    var count = 0;
    var tagName = "li";
    var id = tagName + "_" + count;
    var $frame = $("#FieldEntry");
    while ($frame.find("#" + id).length > 0) {
        id = tagName + "_" + count;
        count++;
    }
    return id;

};

content_class.prototype.generateCId = function () {

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

content_class.prototype.parseSchemaContent = function (htmlContent) {
    var jsTemplates = {},
        $htmlContent = $.parseHTML(htmlContent),
        htmlString = '',
        renderedHtml,
        $renderedHtml;

    if (!$htmlContent) {
        return '';
    }

    // Loop through DOM nodes and find any Mustache templates.
    // Hold onto them so we can dump that back in after
    // schema placeholders have been rendered.
    $.each($htmlContent, function () {
        if ($(this).attr('type') === 'text/template') {
            jsTemplates[$(this).attr('id')] = $(this).text();
        }
    });

    // Render the module HTML. Replaces schema placeholders.
    renderedHtml = Mustache.to_html(htmlContent, this.getSchemaValues("#FieldEntry > ol > ", true));

    // Parse rendered HTML to DOM nodes so we can do things to it.
    $renderedHtml = $.parseHTML(renderedHtml, document, true);

    // Add the Mustache templates we stored earlier back in.
    $.each($renderedHtml, function () {
        if ($(this).attr('type') === 'text/template') {
            var id = $(this).attr('id');
            $(this).html(jsTemplates[id]);
        }

        // Rebuild renderedHtml string with new HTML.
        // DOM nodes will have outerHTML property
        if ($(this)[0].outerHTML) {
            htmlString = htmlString + '\n' + $(this)[0].outerHTML;
        }
            // Text nodes will have data property
        else if ($(this)[0].data) {
            htmlString = htmlString + '\n' + $(this)[0].data;
        }
    });

    return htmlString;
};

// Set publish button text, drop down selected
content_class.prototype.setPublishedStatusState = function (isActive) {

    if (isActive) {
        $("#SaveContentButton").text("Update");
        $("#StatusLabel").text("Published");
        $("#DraftStatusSelector").val("published");
    }
    else {
        $("#SaveContentButton").text("Publish");
        $("#StatusLabel").text("Draft");
        $("#DraftStatusSelector").val("draft");
    }

    // Just saved a draft or published, can now remove version notice
    this.updateVersionNotice();
};

// If there is a "there is a newer version available" message, close and remove it.
content_class.prototype.updateVersionNotice = function () {
    $("#VersionInfoContainer").fadeOut(function () {
        $(this).remove();
    });
};

content_class.prototype.initRevisionEvents = function () {

    var self = this;

    // Show the revision modal and insert proper html into the text area
    $("#RevisionsList ul li").on("click", "a", function () {
        var revisionId = $(this).attr("data-id");
        var type = $("#RevisionsList").attr("type");

        $("#RevisionDetailModal").modal();

        // Update the switch revision link to point to the new revision id we just loaded
        var isBasic = typeof (self.htmlEditor) == "undefined";

        var editContentUrl;
        if (type == "Module") {
            editContentUrl = isBasic ? "/admin/modules/editmoudebasic/" : "/admin/modules/editmodule/";
        }
        else {
            editContentUrl = isBasic ? "/admin/pages/editcontentbasic/" : "/admin/pages/editcontent/";
        }

        $("#SwitchRevision").attr("href", editContentUrl + revisionId + "/");

        self.setRevisionModalHtml(revisionId, type);
    });
};

content_class.prototype.refreshRevisionListing = function () {
    var $listContainer = $("#RevisionsList");
    var pageId = $("#Main div.editContent").attr("data-id");
    var self = this;
    if ($listContainer.length < 1 || pageId < 1) { return; }

    common.showAjaxLoader($listContainer);

    var type = $("#RevisionsList").attr("type");
    var url;
    if (type == "Module") {
        url = '/admin/modules/getrevisionlist/';
    }
    else {
        url = '/admin/pages/getrevisionlist/';
    }


    $.get(url + pageId + '/', function (data) {
        $listContainer.html(data.html);
        common.hideAjaxLoader($listContainer);
        self.initRevisionEvents();
    });
};

content_class.prototype.setRevisionModalHtml = function (revisionId, type) {
    var url;
    if (type == "Module") {
        url = "/admin/modules/getrevisionhtml";
    } else {
        url = "/admin/pages/getrevisionhtml";
    }

    $("#RevisionDetailModal textarea").val("Loading...");
    $.ajax({
        url: url,
        type: "POST",
        data: { revisionId: revisionId },
        success: function (data) {
            $("#RevisionDetailModal textarea").val(data.html);
        },
        error: function (data) {
            noty({ text: 'There was an error processing your request.', type: 'error' });
        }
    });
};

content_class.prototype.formatContentLink = function (value) {
    // replace spaces with dashes
    value = value.replace(/ /g, '-');

    // Strip bad characters
    return value.replace(/[^a-zA-Z0-9-_]/g, '').toLowerCase();
};

content_class.prototype.initParentCategoryEvents = function () {
    var self = this;

    $('#PageCategory').on('change', function () {
        var categoryId = $("#PageCategory option:selected").attr("data-id");
        var pageId = $("div.editContent").attr("data-id") || $("#BuilderContents").attr("data-id");

        $.ajax({
            url: "/admin/navigation/getpageurl/",
            type: "POST",
            data: { pageid: pageId, categoryId: categoryId },
            success: function (data) {
                var siteBaseUrl = $("#SiteUrl").attr("data-url");
                var newFullUrl = data.url;
                var permalink = $("#PermaLinkEnd").text();
                var formattedPermalink = newFullUrl.replace(permalink + "/", "", "i");
                var newUrl = siteBaseUrl + formattedPermalink;

                // Replace multiple slashes with a single slash, ignoring the protocol
                newUrl = newUrl.replace(/([^:]\/)\/+/g, "$1");

                $("#SiteUrl").text(newUrl);
            },
            error: function (data) {
                $('#NewUserModal').modal('hide');
                //common.hideAjaxLoader($container);
                noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    }).change();
};

// Keep at the bottom
$(document).ready(function () {
    content = new content_class();
    content.initPageEvents();
});