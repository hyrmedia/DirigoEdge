
builder_class = function () {
    
    // Keep track of column to append modules to
    this.$activeColumn = null;

    this.pageId = $("#BuilderContents").attr("data-id");
    
    // If it's inline allow CKEditor control
    if (typeof (CKEDITOR) != "undefined") {

        // todo : comment
        $("#BuilderContents div.richTextArea .content").attr("contenteditable", "true");

        CKEDITOR.disableAutoInline = false;
    }
    
    // Will be appended to row to offer further controls. Is a reusable element
    this.rowHelperHtml = '<div class="rowHelper editor-removable"> <i class="icon-move rowMoveIcon"></i>  <i class="icon-remove rowDeleteIcon"></i> </div>';

    // Appended to static module to remove from page, edit module contents, etc.
    this.editStaticModuleHelperHtml = '<div class="staticModuleEditBar editor-removable"> <span class="title"></span>   <i class="remove icon-remove" title="remove"></i> </div>';
    
    // Appended to static module to remove from page, edit module contents, etc.
    this.editRichTextModuleHelperHtml = '<div class="richTextModuleEditBar editor-removable"> <span class="title">Rich Text</span>   <i class="remove icon-remove" title="remove"></i> </div>';
};

builder_class.prototype.initPageEvents = function () {
    var self = this;
    
    // Experimental - Sortable Modules
    $("#BuilderContents > div.row").sortable({
        placeholder: "ui-state-highlight",
        forcePlaceholderSize: false,
        items: ".sortableModule",
        connectWith: "#BuilderContents > div.row > div.columns",       
        dropOnEmpty: true
    });
        
    // Add edit buttons, etc.
    this.initPageLoadedEvents();

    this.initToolBarEvents();

    this.initColumnEvents();

    this.initAddModuleEvent();

    this.initAddRowEvents();

    // Move, delete, etc.
    this.initRowEvents();

    this.initRichTextAreaEvents();

    this.initSaveEvent();

    this.initPageSettingsEvents();
};

builder_class.prototype.initRichTextAreaEvents = function () {
    var self = this;

    // Add Rich Text Area
    $("#AddRichTextArea").click(function () {

        // Close the dialog box
        $('#AddModuleModal').trigger('reveal:close');

        var id = self.generateEditableId();

        // Append Lorem Ipsum
        self.$activeColumn.append("<div class='richTextArea module'> <div id='" + id + "' class='content' contenteditable='true'><h1>Header 1</h1><h2>Header 2</h2><h3>Header 3</h3><p>Dirigo ipsum dolor sit amet trifecta edgie unicorn, json api jquery steak beef boudin salami meatball shankle ground round andouille short loin. Chicken pork loin doner tenderloin ham andouille.</p></div>" + self.editRichTextModuleHelperHtml + "</div>");

        // Start editing
        CKEDITOR.inline(id);
    });
    
    // Rich Text Area - Delete Rich Text Area Modal Event
    $("#BuilderContents div.richTextModuleEditBar i.remove").live("click", function () {
        self.$activeColumn = $(this).parent().parent(); // parent parent should be more reliable than .closest()

        $("#ConfirmDeleteRichTextModal").reveal();
    });

    // Confirm Delete Rich Text Area
    $("#ConfirmBuilderRichTextDelete").click(function () {

        $('#ConfirmDeleteRichTextModal').trigger('reveal:close');

        setTimeout(function () {
            // remove from dom after modal is closed
            self.$activeColumn.remove();
        }, 300);
    });
};

builder_class.prototype.initSaveEvent = function () {
    var self = this;

    // Save Content
    $("#SavePageButton").click(function (e) {
        e.preventDefault();

        var pageId = $("#BuilderContents").attr("data-id");

        var data = {
            entity: {
                ContentPageId: pageId,
                HTMLContent: self.getHtmlContent(),
            }
        };

        $.ajax({
            url: "/ContentAdmin/SaveBuilderPage",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                var noty_id = noty({ text: 'Changes saved successfully.', type: 'success', timeout: 1200 });
            },
            error: function (data) {
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
                $("#SaveSpinner").hide();
            }
        });
    });
};

builder_class.prototype.initRowEvents = function () {
    var self = this;
    
    $("#BuilderContents > .row:not('#AddRowContainer')").append(self.rowHelperHtml);

    // On hover show toobar to allow moving the row around, delete, etc.
    $("#BuilderContents > .row:not('#AddRowContainer')").live(
        {
            mouseenter: function () {
                
                var height = parseInt($(this).outerHeight()) - 40; // 40 is for margin. Could calculate, faster to hardcode
                var $rowHelper = $(this).find(".rowHelper");

                $rowHelper.height(height);                
            },
            mouseleave: function () {
                
            }
        }
    );
    
    // Sortable Rows
    $("#BuilderContents").sortable({
        placeholder: "ui-state-highlight",
        forcePlaceholderSize: false,
        items: ".row",
        handle: ".icon-move",
        //connectWith: ".row",
        cancel: ':input,button,[contenteditable]'
    });
    
    // Delete Row Modal Event
    $("#BuilderContents div.rowHelper .rowDeleteIcon").live("click", function () {
        self.$activeRow = $(this).parent().parent(); // parent parent should be more reliable than .closest()

        $("#ConfirmDeleteRowModal").reveal();
    });
    
    // Confirm Delete Row
    $("#ConfirmBuilderRowDelete").click(function() {

        $('#ConfirmDeleteRowModal').trigger('reveal:close');

        setTimeout(function() {
            // remove from dom after modal is closed
            self.$activeRow.remove();
        }, 300);
    });
};

builder_class.prototype.initAddRowEvents = function () {
    var self = this;

    $("ul.addRowListing li a").click(function(e) {

        e.preventDefault();

        var cols = $(this).attr("data-cols");
        var html = "<div class='row'>";
        
        if (cols == "1") {
            html += "<div class='twelve columns'></div>";
        }
        else if (cols == "2") {
            html += "<div class='six columns'></div><div class='six columns'></div>";
        }
        else if (cols == "3") {
            html += "<div class='four columns'></div><div class='four columns'></div><div class='four columns'></div>";
        }
        else if (cols == "4") {
            html += "<div class='four columns'></div><div class='eight columns'></div>";
        }
        else if (cols == "5") {
            html += "<div class='eight columns'></div><div class='four columns'></div>";
        }
        html += "</div>";

        var $html = $(html);

        // Add the add module button
        $html.find("div.columns").each(function() {
            $(this).append("<a title='Add Module' href='#' class='addModule editor-removable'>+</a>");
        });
        
        // Add the Row Helper element
        $html.append(self.rowHelperHtml);
        
        $("#AddRowModal").trigger('reveal:close');

        $("#AddRowContainer").before($html);

        // Scroll to the newly appended row
        $('html,body').animate({ scrollTop: $html.offset().top }, 300);
    });

    // Add a row to the bottom of the page
    $("#AddRowButton").click(function (e) {
        e.preventDefault();
        
        $("#AddRowModal").reveal();
    });
};

builder_class.prototype.initColumnEvents = function() {
    var self = this;
    
    // Shortcodes - Delete Column Modal Event
    $("#BuilderContents div.shortCodeInsert div.staticModuleEditBar i.remove").live("click", function () {
        self.$activeColumn = $(this).parent().parent(); // parent parent should be more reliable than .closest()

        $("#ConfirmDeleteColumnModal").reveal();
    });

    // Confirm Delete Column
    $("#ConfirmBuilderColumnDelete").click(function () {

        $('#ConfirmDeleteColumnModal').trigger('reveal:close');

        setTimeout(function () {
            // remove from dom after modal is closed
            self.$activeColumn.remove();
        }, 300);
    });
};

builder_class.prototype.initAddModuleEvent = function () {
    var self = this;
    
    // Static Modules
    $("#staticModulesTab a.moduleEntry").live("click", function() {

        var moduleName = $(this).attr("data-name");
        var moduleId = $(this).attr("data-id");

        var $html = $("<div class='shortCodeInsert sortableModule' data-name='" + moduleName + "'></div>");

        var $container = $("#AddModuleModal div.moduleListingContainer");
        common.showAjaxLoader($container);
        $.post('/admin/getModuleData', { id: moduleId }, "json").done(function(result) {
            
            // Insert the html inside
            $html.append(result.html);
            
            // Append editor bar (remove, edit, etc) 
            var $editorHtml = $(self.editStaticModuleHelperHtml);
            $editorHtml.find("span.title").text(moduleName);
            $html.append($editorHtml);
            
            // Insert gathered html into the active column
            self.$activeColumn.append($html);

            // Fire off script if any
            var script = document.createElement("script");
            script.type = "text/javascript";
            script.text = result.js;               // use this for inline scripts
            $("#UserScripts").append(script);
            //document.body.appendChild(script);

            common.hideAjaxLoader($container);

            // Close the dialog box
            $('#AddModuleModal').trigger('reveal:close');
        });
    });

    // Dynamic Modules
    $("#dynamicModulesTab a.moduleEntry").live("click", function () {

        var moduleName = $(this).attr("data-module");

        var $container = $("#AddModuleModal div.moduleListingContainer");
        common.showAjaxLoader($container);
        $.post('/admin/addDynamicModuleData', { name: moduleName }, "json").done(function (result) {

            // Insert gathered html into the active column
            var $html = $(result.html);
            
            // Append editor button
            var editButton = "<a class='editButton removable'><i class='icon-gear'></i></a>";
            $html.append(editButton);
            
            // Append 
            self.$activeColumn.append($html);

            // Fire off script if any
            if (typeof(result.js) != "undefined" && result.js.length > 0) {
                var script = document.createElement("script");
                script.type = "text/javascript";
                script.text = result.js;               // use this for inline script
                document.body.appendChild(script);
            }

            common.hideAjaxLoader($container);

            // Close the dialog box
            $('#AddModuleModal').trigger('reveal:close');
        });
    });

};

builder_class.prototype.getHtmlContent = function() {

    var $html = $("#BuilderContents").clone();
    
    // Remove anything that is deemed..well, removable
    $html.find(".editor-removable").remove();
    
    // Strip out inline styles jqueryui sortable may have thrown on sortable rows
    $html.find("div.row").removeAttr("style").removeClass("ui-sortable");

    // Strip out extra crap that CKEditor throws on editable items
    $html.find("div.richTextArea").each(function () {

        $(this).removeClass("cke_editable cke_editable_inline cke_contents_ltr").removeAttr("style tabindex spellcheck contenteditable");

        $(this).find("div.content").removeClass("cke_editable cke_editable_inline cke_contents_ltr").removeAttr("style tabindex spellcheck contenteditable");
    });
    
    // Replace generated modules with their shortcode equivalent
    $html.find("div.shortCodeInsert").each(function() {
        var moduleName = $(this).attr("data-name");

        // replace with [modulename]
        $(this).replaceWith("[" + moduleName + "]");
    });

   return $html.html();
};

builder_class.prototype.initPageLoadedEvents = function () {
    var self = this;

    // Add "Add Module" buttons to every column
    $("#Main .pageBuilder > div.row > div.columns").each(function () {
        var $button = $("<a class='addModule editor-removable' href='#' title='Add Module'>+</a>");

        $(this).append($button);
    });
    
    // Edit Static Module button to every editable static module
    $("#Main .pageBuilder .shortCodeInsert").each(function () {
        var moduleName = $(this).attr("data-name");

        // Append editor bar (remove, edit, etc) 
        var $editorHtml = $(self.editStaticModuleHelperHtml);
        $editorHtml.find("span.title").text(moduleName);

        $(this).append($editorHtml);
    });
    
    // Edit Rich Text Module button to every editable rich text module
    $("#Main .pageBuilder .richTextArea").each(function () {
        // Append editor bar (remove, edit, etc) 
        var $editorHtml = $(self.editRichTextModuleHelperHtml);

        $(this).append($editorHtml);
    });    
    
    // Add the "Add Row" button at the end of the page content
    var addButtonHtml = '<div class="row editor-removable" id="AddRowContainer"><div class="twelve columns"><a href="#" class="button" id="AddRowButton">Add Row +</a></div></div>';
    $("#BuilderContents").append(addButtonHtml);
    
    // Edit Button Click
    $("a.addModule").live("click", function (e) {

        e.preventDefault();

        self.$activeColumn = $(this).parent();

        $("#AddModuleModal").reveal();
    });
};

builder_class.prototype.initToolBarEvents = function () {

    // Preview Button
    $("#PreviewPageButton").click(function (e) {
        e.preventDefault();
        
        $("body").removeClass("building").addClass("previewing");
        
        // disable all contenteditable items
        $("div.richTextArea .content").attr("contenteditable", "false");
    });

    // Edit / Un-Preview Button
    $("#EditPageButton").click(function (e) {
        e.preventDefault();

        $("body").removeClass("previewing").addClass("building");
        
        // re-enable all contenteditable items
        $("div.richTextArea .content").attr("contenteditable", "true");
    });
};

builder_class.prototype.initPageSettingsEvents = function () {
    var self = this;

    // Open Page Settings and load info if necessary
    $("#PageSettingsButton").parent().click(function (e) {
        e.preventDefault();

        $("body").toggleClass("showPageSettings");
        
        // Populate form fields if we haven't yet, this page load
        if (!$("#PageSettingsMenu").hasClass("activated")) {

            var $container = $("#PageSettingsMenu div.content .innerContent");

            common.showAjaxLoader($container);

            $.post('/contentadmin/getPageSettings', { pageId: self.pageId }, "json").done(function (result) {
                $("#PageUrl").val(result.pageUrl);
                $("#PageTitle").val(result.pageTitle);


                common.hideAjaxLoader($container);

                // Let the handler know not to populate again
                $("#PageSettingsMenu").addClass("activated");
            });
        }
    });
    
    // Close Settings
    $("#CloseSettingsButton").click(function (e) {
        e.preventDefault();
        $("body").removeClass("showPageSettings");
    });
    
    // Save Settings
    $("#SaveBuilderPageSettings").click(function(e) {
        e.preventDefault();

        var pageUrl = $("#PageUrl").val();
        var pageTitle = $("#PageTitle").val();

        var $container = $("#PageSettingsMenu div.content .innerContent");
        
        common.showAjaxLoader($container);

        $.post('/contentadmin/savePageSettings', { pageId: self.pageId, pageTitle : pageTitle, pageUrl : pageUrl }, "json").done(function (result) {

            noty({ text: 'Page Settings Successfully Saved.', type: 'success', timeout: 1200 });

            common.hideAjaxLoader($container);
        });

    });

};

// Generate an id for the current active item
builder_class.prototype.generateEditableId = function ($el) {
    
    // if duplicate, imcrement till we find a free spot
    var count = 0;
    var tagName = "rt";
    var id = tagName + "_" + count;
    var $frame = $("#Main");
    while ($frame.find("#" + id).length > 0) {
        id = tagName + "_" + count;
        count++;
    }
    return id;
};

// Keep at the bottom
$(document).ready(function () {
    builder = new builder_class();
    builder.initPageEvents();
});