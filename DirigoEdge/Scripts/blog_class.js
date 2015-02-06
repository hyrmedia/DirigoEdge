// Handles blog interaction such as AJAX loading 
blog_class = function () {



};

blog_class.prototype.initPageEvents = function () {

    var self = this;

    function getUrlVars() {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    // Ajax Load in more blog when clicking "Load More" button
    $("#LoadMoreBlogs").click(function () {
        var $button = $(this);

        var skip = $button.attr("data-skip");
        var take = $button.attr("data-take");
        var category = $button.attr("data-category");
        var user = typeof $button.attr('data-user') !== 'undefined' ? $button.attr('data-user') : '';
        var date = getUrlVars()['date'];

        common.showAjaxLoader($button);

        $.ajax({
            url: "/blogactions/loadblogs/",
            type: "POST",
            data: {
                take: take,
                skip: skip,
                category: category,
                user: user,
                date: typeof date != 'undefined' ? date : ''
            },
            success: function (data) {

                $(".bigBlogListContainer").append(data.html);
                // Update the button so we load the correct module
                $button.attr("data-skip", data.skip).addClass(data.buttonClass);
                common.hideAjaxLoader($button);
            },
            error: function (data) {

                // All blogs are loaded. Hide the button for now
                $button.fadeOut();

                common.hideAjaxLoader($button);
            }
        });
    });

    // Load more popular blogs
    // Ajax Load in more blog when clicking "Load More" button
    $("#LoadMorePopularBlogs").click(function () {

        var $button = $(this);
        var skip = $button.attr("data-skip");
        var take = $button.attr("data-take");
        var category = $button.attr("data-category");

        common.showAjaxLoader($button);
        $.ajax({
            url: "/blogactions/loadmorepopularblogs/",
            type: "POST",
            data: {
                take: take,
                skip: skip,
                category: category
            },
            success: function (data) {
                $('.popularBlogsContainer ul').append(data.html);
                // Update the button so we load the correct module
                $button.attr("data-skip", data.skip).addClass(data.buttonClass);
                common.hideAjaxLoader($button);
            },
            error: function (data) {
                // All blogs are loaded. Hide the button for now
                $button.fadeOut();
                common.hideAjaxLoader($button);
            }
        });

    });

    // Ajax Load in more blog when clicking "Load More" button
    $("#LoadMoreRelatedBlogs").click(function () {
        var $button = $(this);

        var skip = $button.attr("data-skip");
        var take = $button.attr("data-take");
        var category = $button.attr("data-category");
        var id = $button.attr('data-id');

        common.showAjaxLoader($button);
        $.ajax({
            url: "/blogactions/loadmorerelatedblogs/",
            type: "POST",
            data: {
                take: take,
                skip: skip,
                category: category,
                id: id
            },
            success: function (data) {
                $('.relatedPostsInner').append(data.html);
                // Update the button so we load the correct module
                $button.attr("data-skip", data.skip).addClass(data.buttonClass);
                common.hideAjaxLoader($button);
            },
            error: function (data) {
                // All blogs are loaded. Hide the button for now
                $button.fadeOut();
                common.hideAjaxLoader($button);
            }
        });
    });

    // Ajax Load in more archives when clicking "Load More" button
    $("#LoadMoreArchives").click(function () {
        var lastMonth = $('.archiveContainer ul li:last-child .archive span').data('date');
        var blogCount = $(this).attr("data-blogCount");
        var user = $('.authorTitle').length ? $('.authorTitle a').html() : '';
        var date = getUrlVars()['date'];
        var idList = [];

        $('.bigBlogListContainer article').each(function () {
            idList.push($(this).data('id'));
        });

        var $button = $(this);
        common.showAjaxLoader($button);
        if (lastMonth != 0) {
            $.ajax({
                url: "/BlogActions/LoadMoreArchives/",
                type: "POST",
                data: {
                    lastMonth: lastMonth,
                    count: blogCount,
                    user: user,
                    date: typeof date != 'undefined' ? date : '',
                    idList: idList
                },
                success: function (data) {

                    $('.archiveContainer ul').append(data.html);

                    // Update the button so we load the correct module
                    $button.attr("data-lastMonth", $('.archiveContainer ul li:last-child .archive span').data('date'));

                    if (data.lastMonth == "0") {
                        // All blogs are loaded. Hide the button for now
                        $button.attr("data-lastMonth", 0);
                        $button.fadeOut();
                    }

                    common.hideAjaxLoader($button);
                },
                error: function (data) {

                    // All blogs are loaded. Hide the button for now
                    $button.fadeOut();

                    common.hideAjaxLoader($button);
                }
            });
        } else {
            $button.fadeOut();
        }
    });
};

blog_class.prototype.filterMethods = function () {
    var self = this;

    if ($('.blog-list').length) {
        $('.blog-filter').removeClass('hide');
    }

    $('.side-bar').on('keyup', '.filter-input', function () {
        var val = $(this).val();
        var $button = $("#LoadMoreBlogs");
        var category = $('.category-link.active').length ? $('.category-link.active').attr("data-category") : '';
        var $container = $(this).parent();
        common.showAjaxLoader($container);

        clearTimeout(self.timer);
        self.timer = setTimeout(function () {

            self.ajax = $.ajax({
                url: "/blogactions/loadblogsbytags/",
                type: "POST",
                data: {
                    category: category,
                    tags: val
                },
                success: function (data) {
                    // empty result returns %E2%86%B5 or ↵
                    var content = data.html !== '' && data.html.length > 10 ? data.html : '<h3>No results found.</h3>';
                    $('.blog-list').empty().append(content);

                    // Update the button so we load the correct module
                    $button.attr("data-skip", data.skip).addClass(data.buttonClass);
                    common.hideAjaxLoader($container);
                },
                error: function (data) {
                    common.hideAjaxLoader($container);
                }
            });
            //var val = $(this).val();
            //var $articles = $('.blog');
            //if (val === '') {
            //    $articles.removeClass('hide');
            //    return false;
            //}
            //$articles.addClass('hide');
            //$articles.filter('[data-tags*="' + val.toLowerCase() + '"]').removeClass('hide');
            //return false;
        }, 1000);
    });
};

// Keep at the bottom
$(document).ready(function () {
    blogActions = new blog_class();
    blogActions.initPageEvents();
    blogActions.filterMethods();
});