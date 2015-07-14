navBuilder_class = function () {
    

};

navBuilder_class.prototype.initPageEvents = function () {
    // Only fire off init if we're on the slide edit page
    if ($("#AddNavItem").length > 0) {
        this.initEventHandlers();

        this.initSaveEvent();

        this.initDeleteEvent();
    }
};

navBuilder_class.prototype.initEventHandlers = function () {
    var self = this;
    
    // Fire off the nestedSortable plugin
    $('#BuildList').nestedSortable({
        handle: 'div.handle',
        items: 'li',
        toleranceElement: '> div',
        maxLevels : 4
    });
    
    // Toggle editor pane
    $('#BuildList').on('click', 'div.handle > a:not(.remove)', function (e) {
        e.preventDefault();
        $(this).parent().find("div.editor").first().toggleClass("active");
    });
    
    // Add Nav Item to root
    $("#AddNavItem").click(function (e) {
        e.preventDefault();
        
        var $container = $("#BuildList");
        common.showAjaxLoader($container);
        
        $.post('/admin/navigation/getNewNavItem', { parentId: $("#BuildList").attr("data-id") }, "json").done(function (data) {
            $("ol.topLevelNav").append(data.html);
            common.hideAjaxLoader($container);
        });
    });
    
    // Add Nav Item to sublevel
    $('#BuildList').on('click', '.AddSubNavItem', function (e) {
        e.preventDefault();

        var $this = $(this);
        $this.closest('li').find('div.editor').first().toggleClass('active');
        var $container = $('#BuildList');
        common.showAjaxLoader($container);

        $.post('/admin/navigation/getNewNavItem', { parentId: $("#BuildList").attr("data-id") }, "json").done(function (data) {
            
            if ($this.closest('.subnav').closest('.subnav').length) {
                data.html = data.html.replace("<i class='fa fa-plus pull-right AddSubNavItem' data-toggle='tooltip' title='Add new subnav item'></i>", "");
            }

            // Add the new item at a sub level
            var $list = $this.closest('li');
            if ($list.find('ol').length) {
                $list.find('ol').first().append(data.html);
            } else {
                $list.append('<ol class="subnav">' + data.html + '</ol>');
            }
            
            common.hideAjaxLoader($container);
        });
    });
    
    // Modify the title on key up
    $('#BuildList').on('keyup', 'input.name', function () {
        var val = $(this).val();
        $(this).closest("div.handle").find("> a").text(val);
    });
    
    // Remove Menu Item show modal
    $('#BuildList').on('click', 'div.editor a.remove', function (e) {
        e.preventDefault();
        
        // Store the active li so we can remove it later if user confirms delete
        self.$activeLi = $(this).closest('li');

        $('#NavBuilderConfirmDeleteModal').modal();
    });
};

navBuilder_class.prototype.initDeleteEvent = function() {
    var self = this;
    
    $("#ConfirmNavItemDelete").click(function () {
        var $container = $("#BuildList");
        common.showAjaxLoader($container);

        var id = self.$activeLi.find("> div.handle > a").first().attr("data-id");

        $.post('/admin/navigation/removenavitem', { id: id }, "json").done(function (data) {
            
            common.hideAjaxLoader($container);

            // remove item from list
            self.$activeLi.remove();
            
            // Close the modal
            $('#NavBuilderConfirmDeleteModal').modal('hide');
        });
    });
};

navBuilder_class.prototype.initSaveEvent = function () {

    $("#SaveNavigation").click(function () {

        var $container = $("#BuildList");
        common.showAjaxLoader($container);

        var ParentNavId = $("#BuildList").attr("data-id");

        // store list of nav objects
        var navList = [];

        // Start with the top level nav items then work our way down
        var $topLevel = $("#BuildList > li > div > a:not('.remove')");
        var iLevel = 0;
        $topLevel.each(function () {

            var $editor = $(this).parent().find("div.editor").first();

            // Save the top level
            var navItem = {};
            navItem.Order = iLevel;
            navItem.Href = $editor.find("input.href").val();
            navItem.Name = $editor.find("input.name").val();
            navItem.TargetBlank = $editor.find("input.target").is(':checked');
            navItem.NavigationItemId = $(this).attr("data-id");             
            navItem.ParentNavigationId = ParentNavId;
            navItem.ParentNavigationItemId = -2;// -2 if no children - signifies root. Would use -1 if /content/
            navItem.UsesContentPage = $editor.find("input.pageRadio").is(":checked");
            navItem.ContentPageId = $editor.find("select.pageList option:selected").attr("data-id");
            //navItem.Promo = $editor.find("select.promo-select").val();

            navList.push(navItem);
            
            // Now loop through the children
            var iSubLevel = 0;
            $(this).closest("li").find("ol li a:not('.remove')").each(function () {
                $editor = $(this).parent().find("div.editor").first();

                navItem = {};
                navItem.Order = iSubLevel;
                navItem.Href = $editor.find("input.href").val();
                navItem.Name = $editor.find("input.name").val();
                navItem.TargetBlank = $editor.find("input.target").is(':checked');
                navItem.NavigationItemId = $(this).attr("data-id");
                navItem.ParentNavigationId = ParentNavId;
                navItem.ParentNavigationItemId = $(this).closest("ol").parent().find("> div.handle > a:not('.remove')").attr("data-id") || -2; // Since this is a child, find it's parent id. -2 if no parent
                navItem.UsesContentPage = $editor.find("input.pageRadio").is(":checked");
                navItem.ContentPageId = $editor.find("select.pageList option:selected").attr("data-id");

                navList.push(navItem);

                iSubLevel++;
            });

            iLevel++;
        });

        // Save the data
        var data = {
            navigationId: ParentNavId,
            name: $("#NavBuilderName").val(),
            items: navList
        };
        
        $.ajax({
            url: "/admin/navigation/SaveNavigationSet/",
            type: "POST",
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data, null, 2),
            success: function (data) {
                common.hideAjaxLoader($container);
                var noty_id = noty({ text: 'Navigation set saved successfully.', type: 'success', timeout: 1200 });
            },
            error: function (data) {
                var noty_id = noty({ text: 'There was an error processing your request.', type: 'error', timeout: 3000 });
            }
        });

        return false;
    });
};

// Keep at the bottom
$(document).ready(function () {
    navBuilder = new navBuilder_class();
    navBuilder.initPageEvents();
});