
formBuilder_class = function () {

    // Keep track of active form to make adjustments to
    this.$activeForm = null;
    this.activeFormId = null;
};

formBuilder_class.prototype.initPageEvents = function () {
    var self = this;

    this.initAddFieldEvents();
    
    this.initInspectFormEvents();
    
    this.initSaveFormEvents();

    // Pop Form Modal on click
    $("div.contactFormModule a.editButton").live("click", function() {
        self.$activeForm = $(this).parent().find("form.customForm");
        self.activeFormId = $(this).parent().attr("data-id");

        // Set modal values
        
        // set the editor html
        var $html = self.$activeForm.html();

        $("#FormHtml").html($html);
        
        

        $("#EditFormModal").reveal();        
    });

    // Sorting of form builder elements
    $("#FormHtml").sortable({
        placeholder: "ui-state-highlight",
        forcePlaceholderSize: false,
        items: ".fieldContainer"
    });

};

formBuilder_class.prototype.initInspectFormEvents = function () {

    var self = this;

    $("#FormHtml div.fieldContainer").live("click", function() {
        self.$activeFieldContainer = $(this);

        var type = $(this).attr("data-type");
        
        // Make tab active
        $("#FormInspectorTabs dd.active").removeClass("active").parent().find("dd.fieldSettings").addClass("active");
        $("#FormTabsContent > li.active").removeClass("active").parent().find("#fieldSettingsTab").addClass("active");

        // Show Appropriate Inspector Window
        $("#fieldSettingsTab div.inspectorPane.active").removeClass("active").hide();
        $("#fieldSettingsTab div.inspectorPane[data-type='" + type + "']").addClass("active").show();
        
        // Set the active tab element properties
        var $activeTab = $("#fieldSettingsTab div.inspectorPane.active");

        // Label
        $activeTab.find("div.fieldContainer[data-field='label'] input").attr("value", self.$activeFieldContainer.find("label").text());
        
        $activeTab.find("div.fieldContainer[data-field='placeholder'] input").attr("value", self.$activeFieldContainer.find("input[type='text']").attr("placeholder"));

    });


};

formBuilder_class.prototype.initSaveFormEvents = function () {
    var self = this;

    $("#UpdateFormButton").click(function (e) {
        e.preventDefault();

        // simply replace the old form with the new forms html and close the modal
        self.$activeForm.html($("#FormHtml").html());

        $('#EditFormModal').trigger('reveal:close');
    });

};

formBuilder_class.prototype.initAddFieldEvents = function () {

    $("#addFormFieldTab a.button").click(function (e) {
        e.preventDefault();

        var $appendTo = $("#FormHtml");
        var html = "";

        var formType = $(this).attr("data-type");
        
        if (formType == "text") {
            html = '<div class="fieldContainer" data-type="text"><label>Text Label</label><input type="text"></div>';
        }
        else if (formType == "textarea") {
            html = '<div class="fieldContainer" data-type="textarea"><label>Message Label</label> <textarea rows="4"></textarea> </div>';
        }
        
        $appendTo.prepend(html);
    });

};

// Keep at the bottom
$(document).ready(function () {
    formBuilder = new formBuilder_class();
    formBuilder.initPageEvents();
});