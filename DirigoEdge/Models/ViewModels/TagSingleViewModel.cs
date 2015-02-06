using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class TagSingleViewModel : DirigoBaseModel
    {
        public List<Blog> BlogRoll;
        public string TheTag;

        public List<BlogsCategoriesViewModel.BlogCatExtraData> Categories;

        public TagSingleViewModel(string tag)
        {
            TheTag = tag;

            BlogRoll = Context.Blogs.Where(x => x.Tags.Contains(tag) && x.IsActive == true).OrderByDescending(x => x.Date).ToList();

            Categories = new List<BlogsCategoriesViewModel.BlogCatExtraData>();

            var cats = Context.BlogCategories.Where(x => x.IsActive == true).ToList();
            foreach (var cat in cats)
            {
                int count = Context.Blogs.Count(x => x.MainCategory == cat.CategoryName);
                Categories.Add(new BlogsCategoriesViewModel.BlogCatExtraData() { TheCategory = cat, BlogCount = count });
            }
        }
    }
}