using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Business
{
    public class BlogLoader
    {
        private readonly DataContext _context;
        private readonly SiteSettingsUtils _settingsUtils;

        public BlogLoader(DataContext context = null)
        {
            _context = context ?? new DataContext();
            _settingsUtils = new SiteSettingsUtils(_context);
        }

        public BlogSingleHomeViewModel LoadSingleBlog(String title)
        {
            var model = new BlogSingleHomeViewModel
            {
                TheBlog = _context.Blogs.FirstOrDefault(x => x.PermaLink == title)
            };

            // If no go then try title as a final back up
            if (model.TheBlog == null)
            {
                title = title.Replace(ContentGlobals.BLOGDELIMMETER, " ");
                model.TheBlog = _context.Blogs.FirstOrDefault(x => x.Title == title);

                if (model.TheBlog == null)
                {
                    return model;
                }

                // Go ahead and set the permalink for future reference
                if (String.IsNullOrEmpty(model.TheBlog.PermaLink))
                {
                    model.TheBlog.PermaLink = ContentUtils.GetFormattedUrl(model.TheBlog.Title);
                    _context.SaveChanges();
                }
            }

            model.RelatedPosts = new BlogRelatedViewModel(model.TheBlog.Title);
            model.TheBlogUser = _context.BlogUsers.FirstOrDefault(x => x.UserId == model.TheBlog.AuthorId);
            model.BlogAuthorModel = new BlogAuthorViewModel(model.TheBlog.BlogAuthor.Username);
            model.ShowFacebookLikeButton = _settingsUtils.ShowFbLikeButton();
            model.ShowFacebookComments = _settingsUtils.ShowFbComments();
            model.BlogAbsoluteUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            model.UseDisqusComments = _settingsUtils.UseDisqusComments();

            if (model.UseDisqusComments)
            {
                model.DisqusShortName = _settingsUtils.DisqusShortName();
            }

            model.Categories = GetActiveBlogCategories();

            return model;
        }

        public CategorySingleViewModel LoadBlogsByCategory(String category)
        {
            var catModel = new CategorySingleViewModel();

            category = ContentUtils.GetFormattedUrl(category);


            catModel.AllBlogsInCategory = _context.Blogs.Where(x => x.Category.CategoryNameFormatted == category && x.IsActive)
                        .OrderByDescending(blog => blog.Date)
                        .ToList();

            catModel.BlogRoll = catModel.AllBlogsInCategory
                .Take(catModel.MaxBlogCount)
                .ToList();


            catModel.TheCategory = _context.BlogCategories.FirstOrDefault(x => x.CategoryNameFormatted == category);
            var model = new BlogListModel(_context);
            catModel.MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            catModel.SkipBlogs = catModel.MaxBlogCount;
            catModel.BlogTitle = model.GetBlogSettings().BlogTitle;

            catModel.BlogsByCat = catModel.AllBlogsInCategory
                        .Take(catModel.MaxBlogCount)
                        .ToList();

            return catModel;
        }

        public BlogHomeViewModel LoadBlogHome(string date = "")
        {
            var homeModel = new BlogHomeViewModel();
            var model = new BlogListModel(_context);
            homeModel.MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            homeModel.SkipBlogs = homeModel.MaxBlogCount;

            homeModel.BlogTitle = model.GetBlogSettings().BlogTitle;

            homeModel.FeaturedBlog = _context.Blogs.FirstOrDefault(x => x.IsFeatured);

            homeModel.CurrentMonth = "";

            homeModel.AllBlogs = _context.Blogs.Where(x => x.IsActive).ToList();

            homeModel.BlogRoll = homeModel.AllBlogs.Where(x => x.IsActive)
                        .OrderByDescending(x => x.Date)
                        .Take(homeModel.MaxBlogCount)
                        .ToList();

            homeModel.BlogCats = new BlogsCategoriesViewModel("");

            if (!String.IsNullOrEmpty(date))
            {
                var startDate = Convert.ToDateTime(date);

                homeModel.CurrentMonth = startDate.ToString("MM/yyyy");

                homeModel.BlogRoll =
                    _context.Blogs.Where(
                        x => x.IsActive
                             && (x.Date.Month == startDate.Month)
                             && (x.Date.Year == startDate.Year)
                        )
                           .OrderByDescending(x => x.Date)
                           .Take(homeModel.MaxBlogCount)
                           .ToList();
            }

            return homeModel;
        }

        public TagSingleViewModel LoadBlogsByTag(String tag)
        {
            var model = new TagSingleViewModel
            {
                TheTag = tag,
                BlogRoll = _context.Blogs.Where(x => x.Tags.Any(tg => tg.BlogTagName == tag)
                                                    && x.IsActive).OrderByDescending(x => x.Date).ToList()
            };

            model.UserNameToDisplayName = UserUtils.GetUsernamesForBlogs(model.BlogRoll, _context);

            model.Categories = new List<BlogsCategoriesViewModel.BlogCatExtraData>();

            var cats = _context.BlogCategories.Where(x => x.IsActive).ToList();
            foreach (var cat in cats)
            {
                int count = _context.Blogs.Count(x => x.Category.CategoryId == cat.CategoryId);
                model.Categories.Add(new BlogsCategoriesViewModel.BlogCatExtraData() { TheCategory = cat, BlogCount = count });
            }

            return model;
        }

        public BlogsByUserViewModel LoadBlogsByUser(String userName)
        {
            var blogModel = new BlogsByUserViewModel
            {
                BlogUsername = userName.Replace(ContentGlobals.BLOGDELIMMETER, " ")
            };

            // Get User based on authorid
            blogModel.TheBlogUser = _context.BlogUsers.FirstOrDefault(x => x.Username == blogModel.BlogUsername);

            var model = new BlogListModel(_context);
            blogModel.MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            blogModel.SkipBlogs = blogModel.MaxBlogCount;
            blogModel.BlogTitle = model.GetBlogSettings().BlogTitle;

            blogModel.AllBlogs = _context.Blogs.Where(x => x.BlogAuthor.Username == blogModel.BlogUsername && x.IsActive).ToList();

            blogModel.BlogsByUser = blogModel.AllBlogs
                        .OrderByDescending(blog => blog.Date)
                        .Take(blogModel.MaxBlogCount)
                        .ToList();

            // Try permalink first
            blogModel.TheBlog = blogModel.BlogsByUser.FirstOrDefault(x => x.BlogAuthor.Username == blogModel.BlogUsername);

            return blogModel;
        }


        public List<BlogsCategoriesViewModel.BlogCatExtraData> GetActiveBlogCategories()
        {
            var allCats = new List<BlogsCategoriesViewModel.BlogCatExtraData>();

            foreach (var cat in _context.BlogCategories.Where(x => x.IsActive).ToList())
            {
                var count = _context.Blogs.Count(x => x.Category.CategoryId == cat.CategoryId);
                allCats.Add(new BlogsCategoriesViewModel.BlogCatExtraData { TheCategory = cat, BlogCount = count });
            }

            return allCats;
        }

    }
}