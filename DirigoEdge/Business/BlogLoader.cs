using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;
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

        public BlogSingleHomeViewModel PopulateBlogModel(String title)
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