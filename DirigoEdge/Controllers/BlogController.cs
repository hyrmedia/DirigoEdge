using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using DirigoEdge.Business;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Models.DataModels;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Controllers
{
    public class BlogController : DirigoBaseController
    {
        private BlogLoader _loader;
        private BlogLoader Loader
        {
            get { return _loader ?? (_loader = new BlogLoader(Context)); }
        }

        /// <summary>
        /// If no title or category, could be just listing page.
        /// If no title, but category is set, probably a category listing page
        /// If title and category are set, individual blog.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="category"></param>
        /// <param name="date"></param>
        /// <returns>View</returns>
        public ActionResult Index(string title, string category, string date)
        {
            // Blog Listing Homepage
            if (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(category))
            {
                var model = Loader.LoadBlogHome(date);
                return View("~/Views/Home/Blog.cshtml", model);
            }
            // Category

            if (String.IsNullOrEmpty(title))
            {
                return GetBlogByTitle(category);
            }

            // Tag
            if (category == "tags" && !string.IsNullOrEmpty(title))
            {
                return GetBlogsByTag(title);
            }

            // Blog User
            if (category == "user" && !string.IsNullOrEmpty(title))
            {
                return GetBlogsByUser(title);
            }

            // Category is set and we are trying to view an individual blog
            if (Context.Blogs.Any(x => x.PermaLink == title))
            {
                return GetSingleBlogByTitle(title);
            }

            // Not a blog category or a blog
            return GetBlog404();
        }

        private ActionResult GetBlog404()
        {
            HttpContext.Response.StatusCode = 404;
            return View("~/Views/Home/Error404.cshtml");
        }

        private ActionResult GetSingleBlogByTitle(string title)
        {
            var theModel = Loader.LoadSingleBlog(title);
            return View("~/Views/Home/BlogSingle.cshtml", theModel);
        }

        private ActionResult GetBlogsByUser(string username)
        {
            var model = Loader.LoadBlogsByUser(username);
            return View("~/Views/Blog/BlogsByUser.cshtml", model);
        }

        private ActionResult GetBlogsByTag(string title)
        {
            var model = Loader.LoadBlogsByTag(title);
            return View("~/Views/Blog/TagSingle.cshtml", model);
        }

        private ActionResult GetBlogByTitle(string category)
        {
            if (!Context.BlogCategories.Any(cat => cat.CategoryNameFormatted == category))
            {
                return GetBlog404();
            }

            var model = Loader.LoadBlogsByCategory(category);
            return View("~/Views/Blog/CategoriesSingle.cshtml", model);
        }

        public ActionResult User(string username)
        {
            var model = Loader.LoadBlogsByUser(username);
            return View("~/Views/Blog/BlogsByUser.cshtml", model);
        }

        public ActionResult Categories(string category)
        {
            var model = Loader.LoadBlogsByCategory(category);
            return View("~/Views/Blog/CategoriesSingle.cshtml", model);
        }

        public ActionResult NewPosts()
        {
            var blog = Context.Blogs.FirstOrDefault(x => x.IsActive);
            if (HttpContext.Request.Url == null || blog == null)
            {
                return GetBlog404();
            }

            var blogUrl = "http://" + HttpContext.Request.Url.Host + "/blog/";
            var postItems = Context.Blogs.Where(p => p.IsActive).OrderByDescending(p => p.Date).Take(25).ToList()
                .Select(p => new SyndicationItem(p.Title, p.HtmlContent, new Uri(blogUrl + p.Title)));


            var feed = new SyndicationFeed(blog.Title, blog.Title, new Uri(blogUrl + blog.Title), postItems)
            {
                Language = "en-US",
                Title = new TextSyndicationContent(blog.Title)
            };

            return new FeedResult(new Rss20FeedFormatter(feed));
        }

        public ActionResult Tags(string tag)
        {
            // Don't index blog tag pages
            ViewBag.Robots = "NOINDEX, NOFOLLOW";

            // Blog Listing Homepage
            if (String.IsNullOrEmpty(tag))
            {
                tag = "";
            }

            var model = Loader.LoadBlogsByTag(tag);
            return View("~/Views/Blog/TagSingle.cshtml", model);
        }
    }
}
