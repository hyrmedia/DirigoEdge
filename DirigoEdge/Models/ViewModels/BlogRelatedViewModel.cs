using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class BlogRelatedViewModel : DirigoBaseModel
    {
        public List<Blog> RelatedPosts;
        public Blog TheBlog;
        public int BlogRollCount;
        public int MaxBlogCount;
        public int SkipBlogs;
        public int LastBlogId;

        public Blog FeaturedBlog;
        public bool ReachedMaxBlogs;

        public BlogRelatedViewModel(string title)
        {
            // Try permalink first
            TheBlog = Context.Blogs.FirstOrDefault(x => x.PermaLink == title);

            var model = new BlogListModel(Context);
            MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            SkipBlogs = MaxBlogCount;

            // If no go then try title as a final back up
            if (TheBlog == null)
            {
                title = title.Replace(ContentGlobals.BLOGDELIMMETER, " ");
                TheBlog = Context.Blogs.FirstOrDefault(x => x.Title == title);
            }

            if (TheBlog != null && TheBlog.Tags != null)
            {
                List<string> tags = TheBlog.Tags.Split(',').ToList();
                RelatedPosts = Context.Blogs.Where(x => x.BlogId != TheBlog.BlogId && tags.Contains(x.Tags) && x.MainCategory == TheBlog.MainCategory)
                                .OrderByDescending(blog => blog.Date)
                                .Take(MaxBlogCount)
                                .ToList();

            }
        }
    }
}