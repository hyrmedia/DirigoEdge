using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class LatestBlogsViewModel : DirigoBaseModel
    {
        public List<Blog> Blogs;

        public LatestBlogsViewModel(int blogCount = 10)
        {
            Context = new DataContext();
            Blogs = Context.Blogs.OrderByDescending(x => x.Date).Take(blogCount).ToList();
        }
    }
}