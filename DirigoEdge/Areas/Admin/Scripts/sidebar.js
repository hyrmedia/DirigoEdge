sidebar_class = function() {
    this.minWidth = 1200;
};

sidebar_class.prototype.initPageEvents = function () {
    var self = this;
    
    // Sidebar menu
    $("#sidebar ul > li.has-dropdown > a").click(function (e) {
        e.preventDefault();
        
        var $li = $(this).parent();
        
        $li.find("ul.dropdown").slideToggle("fast", function() {
            if ($(this).is(":visible")) {
                $(this).parent().addClass("active");                
            }
            else {
                $(this).parent().removeClass("active");
            }
        });
    });

    // Keep sidebar open if on manage page
    var path = window.location.pathname;
    $("#sidebar ul > li.has-dropdown > ul > li > a[href='" + path + "']").closest("li.has-dropdown").addClass("active").find('ul.dropdown').show();

    $('#sidebar .collapse').collapse({ toggle: false });

    self.timeout = {};
    $("body #sidebar").hover(function () {

        $("body").addClass("sidebarOpen");

    }, function () {

        $("body").removeClass("sidebarOpen");

        $('#sidebar .collapse').collapse('hide');

    });


    // Avoid event propagation issues with anchors. Should
    // be clickable when sidebar is open, should open sidebar
    // if it is not already open.
    $("#sidebar a").click(function (e) {
        if ($('body').hasClass('sidebarOpen')) {
            return true;
        }
        $("body").toggleClass("sidebarOpen");
        return false;
    });

    // If the sidebar element itself is clicked, toggle sidebar.
    $("#sidebar").click(function (e) {
        if (e.originalEvent.target.nodeName.toLowerCase() === 'section' && e.originalEvent.target.id === 'sidebar') {
            $("body").toggleClass("sidebarOpen");
            return false;
        }
    });

    self.getMenuState();
};

sidebar_class.prototype.saveMenuState = function(state) {
    if (Modernizr.localstorage) {
        localStorage.setItem("menuState", state);
    }
};

sidebar_class.prototype.getMenuState = function () {
    if (Modernizr.localstorage) {
        var bClass = localStorage["menuState"];
        $("body").addClass(bClass);
    }
};


$(document).ready(function () {
    sidebarClass = new sidebar_class();
    sidebarClass.initPageEvents();
});