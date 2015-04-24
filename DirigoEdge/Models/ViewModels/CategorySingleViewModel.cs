using System.Collections.Generic;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

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
    }

}