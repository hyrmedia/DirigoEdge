using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class BlogPopularViewModel : DirigoBaseModel
    {
        public List<Blog> BlogRoll;
        public List<Blog> AllBlogs;
        public int BlogRollCount = 10;
        public int MaxBlogCount = 10;
        public int LastBlogId;
        public int SkipBlogs;

		public bool ReachedMaxBlogs;

	    public string BlogTitle;

		public BlogPopularViewModel()
		{
				var model = new BlogListModel(Context);
                MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
                SkipBlogs = MaxBlogCount;
                BlogTitle = model.GetBlogSettings().BlogTitle;

                AllBlogs = Context.Blogs.Where(x => x.IsActive).ToList();

                BlogRoll = AllBlogs
                            .OrderByDescending(x => x.Date)
                            .Take(MaxBlogCount)
                            .ToList();
		}
	}
}