using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageBlogsViewModel : DirigoBaseModel
    {
        public List<Blog> BlogListing;

        public ManageBlogsViewModel()
        {
            BookmarkTitle = "Manage Posts";
            BlogListing = Context.Blogs.ToList();
        }
    }
}