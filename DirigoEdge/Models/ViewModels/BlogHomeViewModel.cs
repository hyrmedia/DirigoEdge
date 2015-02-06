using System;
using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Models.ViewModels;

namespace DirigoEdge.Models.ViewModels
{
    public class BlogHomeViewModel : DirigoBaseModel
    {
        public List<Blog> BlogRoll;
        public List<Blog> AllBlogs;
        public BlogsCategoriesViewModel BlogCats;

        public int BlogRollCount;
        public int MaxBlogCount;
        public int LastBlogId;
        public int SkipBlogs;

        public Blog FeaturedBlog;
        public bool ReachedMaxBlogs;

        public string CurrentMonth;

        public string BlogTitle;

        public BlogHomeViewModel(string date = "")
        {
            var model = new BlogListModel(Context);
            MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            SkipBlogs = MaxBlogCount;

            BlogTitle = model.GetBlogSettings().BlogTitle;

            FeaturedBlog = Context.Blogs.FirstOrDefault(x => x.IsFeatured);

            CurrentMonth = "";

            AllBlogs = Context.Blogs.Where(x => x.IsActive).ToList();

            BlogRoll = AllBlogs.Where(x => x.IsActive)
                        .OrderByDescending(x => x.Date)
                        .Take(MaxBlogCount)
                        .ToList();

            BlogCats = new BlogsCategoriesViewModel("");

            if (!String.IsNullOrEmpty(date))
            {
                DateTime startDate = Convert.ToDateTime(date);

                CurrentMonth = startDate.ToString("MM/yyyy");

                BlogRoll =
                    Context.Blogs.Where(
                        x => x.IsActive
                             && (x.Date.Month == startDate.Month)
                             && (x.Date.Year == startDate.Year)
                        )
                           .OrderByDescending(x => x.Date)
                           .Take(MaxBlogCount)
                           .ToList();
            }

        }
    }
}