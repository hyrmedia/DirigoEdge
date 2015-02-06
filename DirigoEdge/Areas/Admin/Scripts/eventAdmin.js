/// ===========================================================================================
/// This currently serves as both the event admin, user admin, and content admin Javascript area
/// ===========================================================================================

event_class = function() {
    // Track whether the event has been saved or not on the add Event page
    this.eventIsSaved = false;
    this.eventId = -1;
    this.eventSpaceReplacementChar = '-';
};

event_class.prototype.initPageEvents = function() {
    this.editEventEvents();
    this.addEventEvents();
    this.initPermaLinkEvents();
    this.manageEventEvents();
    //this.initTinyMCEEvents();
    this.initPreviewEvent();
    this.initPublishDateEvents();

    // Ckeditor instances
    if ($('#CKEDITEVENT').length > 0) {
        this.CKPageEditor = CKEDITOR.replace('CKEDITEVENT', {
            // options here
            height:412
        });
    }

    if ($('#EventShortDescription').length > 0) {
        this.CKShortDescEditor = CKEDITOR.replace('EventShortDescription', {
            toolbar: [['Source', '-', 'Bold', 'Italic']]
        });
    }
};

event_class.prototype.initPermaLinkEvents = function () {
    // Edit Permalink on edit event page
    var self = this;

    $("#EditPermaLink, #PermaLinkEnd").click(function () {
        var textVal = $("#PermaLinkEnd").text();
        $("#PermaLinkEditPane").removeClass("hide").show().val(textVal);
        $(this).hide();
        $("#PermaLinkEnd").hide();

        $("#PermaLinkEditPane").focus();
    });

    // Key up format the permalink
    $("#PermaLinkEditPane").keyup(function () {
        $(this).val(self.formatEventLink($(this).val()));
    });

    // Hide the edit pane and update permalink
    $("#PermaLinkEditPane").blur(function () {

        // Clean up permalink by removing spaces and replacing with dashes
        // Remove all illegal characters
        var val = self.formatEventLink($(this).val());

        $(this).val(val);

        // Set the text with the new value
        $("#PermaLinkEnd").text($(this).val());
        $(this).hide();
        $("#EditPermaLink , #PermaLinkEnd").show();

        // Let the editor know we've made a modification
        $("#PermaLinkEnd").attr("data-modified", "true");
    });
};


// Replace spaces with appropriate character (such as dash)
// Remove illegal characters
event_class.prototype.formatEventLink = function (value) {
    // replace spaces with whatever (currently dashes)
    value = value.replace(/ /g, this.eventSpaceReplacementChar);

    // Strip bad characters
    return value.replace(/[^a-zA-Z0-9-_]/g, '');
};

event_class.prototype.initPublishDateEvents = function() {
    $('[data-field="StartDate"], [data-field="EndDate"]').datepicker({ defaultDate: '' });
};

event_class.prototype.initPreviewEvent = function() {
    $("#PreviewEventButton").click(function() {
        var title = $("#PermaLinkEnd").text().toLowerCase();
        var host = 'http://' + window.location.host + '/Event/';

        // Open the event in a new window
        window.open(host + title + '/' + '?debug=true');
    });
};

event_class.prototype.manageEventEvents = function() {
    var self = this;

    // Delete event pop up
    $("div.manageEvents").on("click", "a.btn.delete", function () {
        self.manageEventId = $(this).attr("data-id");
        self.$manageEventRow = $(this).parent().parent();
        var title = '"' + self.$manageEventRow.find("td.title a").text() + '"';
        $("#popTitle").text(title);
        $("#DeleteModal").modal();
    });

    // Confirm Delete Event
    $(document).on("click", "#ConfirmEventDelete", function () {
        var id = self.manageEventId;
        $.ajax({
            url: "/admin/events/deleteevent",
            type: "POST",
            data: {
                id: self.manageEventId
            },
            success: function(data) {
                var noty_id = noty({ text: 'Event Successfully Deleted.', type: 'success', timeout: 3000 });
                self.$manageEventRow.remove();
                $('#DeleteModal').modal('hide');
            },
            error: function(data) {
                $('#DeleteModal').modal('hide');
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
            }
        });
    });
};

event_class.prototype.editEventEvents = function() {
    var self = this;

    // Set startup information
    if ($("div.editEvent").length > 0) {
        self.eventId = $("div.editEvent").attr("data-id");
        self.eventIsSaved = true;
    }
};

event_class.prototype.addEventEvents = function() {
    var self = this;

    // Set event title permalink hint on keypress
    $("#EventTitle").keyup(function() {
        // Only update permalink if it hasn't been touched yet
        var permLinkModified = $("#PermaLinkEnd").attr("data-modified").toLowerCase() == "true";
        if (!permLinkModified) {
            var val = $(this).val();
            val = val.replace(/ /g, self.eventSpaceReplacementChar);
            $("#PermaLinkEditPane").text(val);
        }
    });

    // Fix spacing issues on page load
    $("#EventTitle").trigger("keyup");

    // Save Event from edit / add event page
    $("#SaveEvent").click(function() {
        var mainCategory = $('[data-field="MainCategory"]').val();
        var content = CKEDITOR.instances.CKEDITEVENT.getData();
        var featText = CKEDITOR.instances.EventShortDescription.getData();
        var data = {
            entity: {
                Title: $("#EventTitle").val(),
                HtmlContent: content,
                FeaturedImageUrl: $("#FeaturedImage").val(),
                IsActive: $("#IsActiveCheck").is(":checked"),
                IsFeatured: $("#IsFeaturedCheck").is(":checked"),
                EventID: self.eventId,
                MainCategory: mainCategory,
                Tags: $("#EventTags").val(),
                ShortDesc: featText,
                StartDate: $('[data-field="StartDate"]').val(),
                EndDate: $('[data-field="EndDate"]').val(),
                PermaLink: self.formatEventLink($("#PermaLinkEnd").text())
            }
        };

        if ($("#EventTitle").attr("value").length < 1) {
            alert("Please enter a title.");
            return false;
        }
        
        if ($('[data-field="StartDate"]').val() > $('[data-field="EndDate"]').val()) {
            alert("Please enter end date greater than or equal to start date. For one day events, enter start date same as end date");
            return false;
        }
        
        $("#SaveSpinner").show();
        var postUrl = self.eventIsSaved ? "/admin/events/modifyevent" : "/admin/events/addevent";
        $.ajax({
            url: postUrl,
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function(data) {
                self.eventIsSaved = true;
                self.eventId = data.id;

                var noty_id = noty({ text: 'Changes saved successfully.', type: 'success', timeout: 3000 });
                $("#SaveSpinner").hide();
            },
            error: function(data) {
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error' });
                $("#SaveSpinner").hide();
            }
        });
    });
};

event_class.prototype.initTinyMCEEvents = function() {
    //tinymce on event editor
    $('div.editArea textarea').tinymce({
        // Location of TinyMCE script
        script_url: '/Scripts/tinyMCE/tiny_mce.js',
        height: "450px",
        // General options
        theme: "advanced",
        skin: "o2k7",
        skin_variant: "silver",
        plugins: "autolink,spellchecker,lists,save,advimage,advlink,iespell,inlinepopups,insertdatetime,media,searchreplace,contextmenu,directionality,noneditable,template,safari,xhtmlxtras",

        // Theme options
        theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,bullist,numlist,|,outdent,indent,|,forecolor,backcolor,|,fontselect,fontsizeselect,link,|,code,|,blockquote,|,image,|,insertdate,inserttime,|,sub,sup,spellchecker",
        theme_advanced_buttons2: "",
        theme_advanced_buttons3: "",

        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false,

        // Example content CSS (should be your site CSS)
        content_css: "/CSS/site.css",

        // Drop lists for link/image/media/template dialogs
        template_external_list_url: "lists/template_list.js",
        external_link_list_url: "lists/link_list.js",
        external_image_list_url: "lists/image_list.js",
        media_external_list_url: "lists/media_list.js",
    });

};
// Keep at the bottom
$(document).ready(function() {
    eventMCP = new event_class();
    eventMCP.initPageEvents();
});