using System.Collections.Generic;
using System.Linq;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Models.ViewModels
{
    public class LatestBlogsViewModel : DirigoBaseModel
    {
        public List<Blog> Blogs;

        public LatestBlogsViewModel(int blogCount = 10)
        {
            Context = new WebDataContext();
            Blogs = Context.Blogs.OrderByDescending(x => x.Date).Take(blogCount).ToList();
        }
    }
}