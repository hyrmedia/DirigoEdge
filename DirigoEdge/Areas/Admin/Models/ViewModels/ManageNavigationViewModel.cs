using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageNavigationViewModel : DirigoBaseModel
    {
        public List<Navigation> Navs;

        public ManageNavigationViewModel()
        {
            BookmarkTitle = "Manage Navigation";
            Navs = Context.Navigations.ToList();
        }
    }
}