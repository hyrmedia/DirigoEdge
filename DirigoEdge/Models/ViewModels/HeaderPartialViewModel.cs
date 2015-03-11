using System.Collections.Generic;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class HeaderPartialViewModel : DirigoBaseModel
    {
        public List<NavigationItem> MainNav;
 
        public HeaderPartialViewModel()
        {
            MainNav = CachedObjects.GetMasterNavigationList(false);
		}
    }
}