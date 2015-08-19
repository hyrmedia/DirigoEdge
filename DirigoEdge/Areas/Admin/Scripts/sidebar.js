sidebar_class = function() {
    this.minWidth = 1200;
};

sidebar_class.prototype.initPageEvents = function () {
    var self = this;

    $('.sidebar .collapse').collapse({ toggle: false });

    $('body .sidebar').hover(function () {

        $('body').addClass('sidebarOpen');

    }, function () {

        $('body').removeClass('sidebarOpen');

    });

    // Avoid event propagation issues with anchors. Should
    // be clickable when sidebar is open, should open sidebar
    // if it is not already open.
    $('.sidebar a').click(function (e) {
        if ($('body').hasClass('sidebarOpen')) {
            return true;
        }
        $('body').toggleClass('sidebarOpen');
        return false;
    });

    // If the sidebar element itself is clicked, toggle sidebar.
    $('.sidebar').click(function (e) {
        if (e.originalEvent.target.nodeName.toLowerCase() === 'section' && e.originalEvent.target.id === 'sidebar') {
            $('body').toggleClass('sidebarOpen');
            return false;
        }
    });

    $('.sidebar-toggle').click(function () {
        $('body').toggleClass('sidebar-open');
    });
};


$(document).ready(function () {
    sidebarClass = new sidebar_class();
    sidebarClass.initPageEvents();
});