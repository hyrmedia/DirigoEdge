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
    }
}