using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Business
{
    public class BlogLoader
    {
        private DataContext Context;
        private BlogUtils utils;
        private SiteSettingsUtils SettingsUtils;

        public BlogLoader(DataContext context = null)
        {
            Context = context ?? new DataContext();
            utils = new BlogUtils(Context);
            SettingsUtils = new SiteSettingsUtils(Context);
        }

        public BlogSingleHomeViewModel PopulateSingleBlogModel(String title)
        {
            var model = new BlogSingleHomeViewModel
            {
                TheBlog = Context.Blogs.FirstOrDefault(x => x.PermaLink == title)
            };

            // If no go then try title as a final back up
            if (model.TheBlog == null)
            {
                title = title.Replace(ContentGlobals.BLOGDELIMMETER, " ");
                model.TheBlog = Context.Blogs.FirstOrDefault(x => x.Title == title);

                if (model.TheBlog == null)
                {
                    return model;
                }

                // Go ahead and set the permalink for future reference
                if (String.IsNullOrEmpty(model.TheBlog.PermaLink))
                {
                    model.TheBlog.PermaLink = ContentUtils.GetFormattedUrl(model.TheBlog.Title);
                    Context.SaveChanges();
                }
            }

            model.RelatedPosts = new BlogRelatedViewModel(model.TheBlog.Title);
            model.TheBlogUser = Context.BlogUsers.FirstOrDefault(x => x.UserId == model.TheBlog.AuthorId);
            model.BlogAuthorModel = new BlogAuthorViewModel(model.TheBlog.BlogAuthor.Username);
            model.ShowFacebookLikeButton = SettingsUtils.ShowFbLikeButton();
            model.ShowFacebookComments = SettingsUtils.ShowFbComments();
            model.BlogAbsoluteUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            model.UseDisqusComments = SettingsUtils.UseDisqusComments();

            if (model.UseDisqusComments)
            {
                model.DisqusShortName = SettingsUtils.DisqusShortName();
            }

            model.Categories = GetActiveBlogCategories();

            return model;
        }

        public CategorySingleViewModel LoadBlogsByCategory(String category)
        {
            var catModel = new CategorySingleViewModel();

            category = ContentUtils.GetFormattedUrl(category);


            catModel.AllBlogsInCategory = Context.Blogs.Where(x => x.Category.CategoryNameFormatted == category && x.IsActive)
                        .OrderByDescending(blog => blog.Date)
                        .ToList();

            catModel.BlogRoll = catModel.AllBlogsInCategory
                .Take(catModel.MaxBlogCount)
                .ToList();


            catModel.TheCategory = Context.BlogCategories.FirstOrDefault(x => x.CategoryNameFormatted == category);
            var model = new BlogListModel(Context);
            catModel.MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            catModel.SkipBlogs = catModel.MaxBlogCount;
            catModel.BlogTitle = model.GetBlogSettings().BlogTitle;

            catModel.BlogsByCat = catModel.AllBlogsInCategory
                        .Take(catModel.MaxBlogCount)
                        .ToList();

            return catModel;
        }

        public BlogsByUserViewModel PopulateBlogsByUser(String userName)
        {
            var blogModel = new BlogsByUserViewModel
            {
                BlogUsername = userName.Replace(ContentGlobals.BLOGDELIMMETER, " ")
            };

            // Get User based on authorid
            blogModel.TheBlogUser = Context.BlogUsers.FirstOrDefault(x => x.Username == blogModel.BlogUsername);

            var model = new BlogListModel(Context);
            blogModel.MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            blogModel.SkipBlogs = blogModel.MaxBlogCount;
            blogModel.BlogTitle = model.GetBlogSettings().BlogTitle;

            blogModel.AllBlogs = Context.Blogs.Where(x => x.BlogAuthor.Username == blogModel.BlogUsername && x.IsActive).ToList();

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

            foreach (var cat in Context.BlogCategories.Where(x => x.IsActive).ToList())
            {
                var count = Context.Blogs.Count(x => x.Category.CategoryId == cat.CategoryId);
                allCats.Add(new BlogsCategoriesViewModel.BlogCatExtraData { TheCategory = cat, BlogCount = count });
            }

            return allCats;
        }

    }
}