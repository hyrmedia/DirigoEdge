using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Models.ViewModels
{
    public class BlogsByUserViewModel : DirigoBaseModel
    {
        public Blog TheBlog;
        public List<Blog> AllBlogs;
        public List<Blog> BlogsByUser;
        public BlogUser TheBlogUser;
        public int BlogRollCount;
        public int MaxBlogCount;
        public int LastBlogId;
        public bool ReachedMaxBlogs;
        public string BlogUsername;
        public int SkipBlogs;

        public string BlogTitle;

        public BlogsByUserViewModel()
        {
            
        }
    }
}