using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Models.ViewModels
{
    public class CategorySingleViewModel : DirigoBaseModel
    {
        public BlogCategory TheCategory;
        public List<Blog> AllBlogsInCategory;

        public List<Blog> BlogRoll;
        public List<Blog> BlogsByCat;
        public int BlogRollCount = 10;
        public int MaxBlogCount = 10;
        public int LastBlogId = 0;
        public bool ReachedMaxBlogs;
        public int SkipBlogs;
        public List<string> ImageList;
        public string BlogTitle;

        public CategorySingleViewModel(string category)
        {
            category = ContentUtils.GetFormattedUrl(category);


            AllBlogsInCategory = Context.Blogs.Where(x => x.Category.CategoryNameFormatted == category && x.IsActive)
                        .OrderByDescending(blog => blog.Date)
                        .ToList();

            BlogRoll = AllBlogsInCategory
                .Take(MaxBlogCount)
                .ToList();


            TheCategory = Context.BlogCategories.FirstOrDefault(x => x.CategoryNameFormatted == category);
            var model = new BlogListModel(Context);
            MaxBlogCount = model.GetBlogSettings().MaxBlogsOnHomepageBeforeLoad;
            SkipBlogs = MaxBlogCount;
            BlogTitle = model.GetBlogSettings().BlogTitle;

            BlogsByCat = AllBlogsInCategory
                        .Take(MaxBlogCount)
                        .ToList();
        }
    }

}