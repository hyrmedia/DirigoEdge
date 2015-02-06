using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdge.Entities;
using DirigoEdge.Utils;

namespace DirigoEdge.Models
{
    public class BlogListModel : DirigoBaseModel
    {
        public BlogListModel(DataContext context) : base(context)
        {
            
        }

        /// <summary>
        /// Gets a list of Blogs since after the date of the given blog id
        /// </summary>
        /// <param name="take">The id of the blog to start from</param>
        /// <param name="skip">Number of blogs to return</param>
        /// <param name="category">Blog category to pull from</param>
        /// <param name="user">Number of blogs to return</param>
        /// <param name="date">Date MM/yyyy </param>
        public List<Blog> GetMoreBlogs(int take, int skip, string category, string user = "", string date = "")
        {
            if (user != "")
            {
                return Context.Blogs.Where(x => x.Author == user && x.MainCategory != "")
                    .OrderByDescending(blog => blog.Date)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }
            if (date != "")
            {
                var dateConverted = Convert.ToDateTime(date);
                return Context.Blogs.Where(
                        x => x.IsActive
                             && (x.Date.Month == dateConverted.Month)
                             && (x.Date.Year == dateConverted.Year)
                        )
                           .OrderByDescending(x => x.Date)
                           .Skip(skip)
                           .Take(take)
                           .ToList();
            }
            if (category != "")
            {
                return Context.Blogs.Where(x => x.MainCategory != "Default" && x.MainCategory == category)
                    .OrderByDescending(blog => blog.Date)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }

            return Context.Blogs.Where(x => x.MainCategory != "")
                    .OrderByDescending(blog => blog.Date)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
        }

        /// <summary>
        /// Return a list of blogs based on search term and category
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<Blog> GetMoreBlogsByTags(string tags = "", string category = "")
        {
            
            
                if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(tags))
                {
                    return Context.Blogs.Where(x => x.MainCategory != "Default" && x.MainCategory == category
                        && (x.MainCategory.Contains(tags) || x.Tags.Contains(tags)))
                        .OrderByDescending(blog => blog.Date)
                        .ToList();
                }
                if (!string.IsNullOrEmpty(tags))
                {
                    return Context.Blogs.Where(x => x.MainCategory != "Default"
                        && (x.MainCategory.Contains(tags) || x.Tags.Contains(tags)))
                        .OrderByDescending(blog => blog.Date)
                        .ToList();
                }
                if (!string.IsNullOrEmpty(category))
                {
                    return Context.Blogs.Where(x => x.MainCategory != "Default" && x.MainCategory == category)
                        .OrderByDescending(blog => blog.Date)
                        .ToList();
                }
                return Context.Blogs.Where(x => x.MainCategory != "Default")
                        .OrderByDescending(blog => blog.Date)
                        .ToList();
            
        }

        /// <summary>
        /// Gets a list of Blogs since after the date of the given blog id
        /// </summary>
        /// <param name="lastMonth">The last month shown.Start from month + 1 month</param>
        /// <param name="count">Number of blogs to return</param>
        /// <param name="idList">List of blog ids already on page, not used here yet</param>
        /// <param name="user">Number of blogs to return</param>
        /// <param name="date">Date MM/yyyy, not used here yet </param>
        public IEnumerable<string> GetArchives(string lastMonth, int count, List<string> idList, string user = "", string date = "")
        {

                var lastDate = new DateTime(Convert.ToDateTime(lastMonth).Year, Convert.ToDateTime(lastMonth).Month, 1);
                var blogs = Context.Blogs.Where(x => x.IsActive && x.Date < lastDate)
                            .OrderByDescending(blog => blog.Date)
                            .ToList();

                return (from p in blogs
                        group p by
                            new { month = p.Date.ToString("MMM"), year = p.Date.ToString("yyyy"), dateString = p.Date.ToString("MM/yyyy") }
                            into d
                            select
                                String.Format(
                                    "<a href=\"/blog?date={3}\" class=\"archive\">" +
                                    "<span class=\"dateRef\" data-date=\"{3}\">{0} {1}</span>  ({2}) </a>",
                                    d.Key.month, d.Key.year, d.Count(), d.Key.dateString)
                ).Take(count);
        }

        public BlogSettings GetBlogSettings()
        {
                // Get the amount of blogs to load on homepage
                var blogSettings = Context.BlogSettings.FirstOrDefault();
                if (blogSettings != null)
                {
                    return blogSettings;
                }
                else
                {
                    // Set the default blog settings then come back and get data
                    SettingsUtils.SetDefaultBlogSettings();
                    blogSettings = Context.BlogSettings.FirstOrDefault();
                    return blogSettings;
                }
        }

        /// <summary>
        /// Gets a list of Blogs since after the date of the given blog id
        /// </summary>
        /// <param name="take">The id of the blog to start from</param>
        /// <param name="skip">Number of blogs to return</param>
        /// <param name="category">Blog category to pull from</param>
        /// <param name="id">Blog Id to base search</param>
        public List<Blog> GetMoreRelatedBlogs(int take, int skip, string category, int id)
        {
            if (id < 1)
            {
                return new List<Blog>();
            }

            var theBlog = Context.Blogs.FirstOrDefault(x => x.BlogId == id);

            List<string> tags = theBlog.Tags.Split(',').ToList();

            if (tags.Any())
            {
                return Context.Blogs.Where(x => x.MainCategory != "Default"
                        && x.BlogId != theBlog.BlogId
                        && tags.Contains(x.Tags)
                        && x.MainCategory == theBlog.MainCategory)
                    .OrderByDescending(blog => blog.Date)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
            }

            return Context.Blogs.Where(x => x.MainCategory != "Default"
                        && x.BlogId != theBlog.BlogId
                        && x.MainCategory == theBlog.MainCategory)
                    .OrderByDescending(blog => blog.Date)
                    .Skip(skip)
                    .Take(take)
                    .ToList();
        }

    }
}