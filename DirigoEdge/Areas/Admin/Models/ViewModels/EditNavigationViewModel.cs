using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils;
using System.Configuration;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class EditNavigationViewModel : DirigoBaseModel
    {
        public Navigation TheNav;
        public List<NavigationItem> TopLevelNavItems;
        //public List<ContentModule> PromoModules;

        // id, Name
        public Dictionary<int, string> PageIdNameCollection;

        public EditNavigationViewModel(int id)
        {
            var excludeSchemasNavigation = ConfigurationManager.AppSettings["ExcludeSchemasForNavigation"].Split(',').Select(int.Parse);

            // Grab Every Page and Id so we can list it in the dropdown
            PageIdNameCollection = Context.ContentPages.Where(x => x.IsActive.Value && !x.IsRevision && (x.SchemaId == null || !excludeSchemasNavigation.Contains(x.SchemaId.Value))).Select(x => new { x.ContentPageId, x.Title }).OrderBy(x => x.Title).ToDictionary(o => o.ContentPageId, o => o.Title);

            //PromoModules = context.ContentModules.Where(m => m.SchemaId == 3).ToList();

            TheNav = Context.Navigations.Single(x => x.NavigationId == id);
            BookmarkTitle = TheNav.Name;


            // Grab the top level items first, then populate their children
            if (TheNav != null)
            {
                TopLevelNavItems = Context.NavigationItems.Where(x => x.ParentNavigationId == TheNav.NavigationId && x.ParentNavigationItemId < 0).OrderBy(x => x.Order).ToList();

                if (TopLevelNavItems.Count > 0)
                {
                    TopLevelNavItems = PopulateNavList(TopLevelNavItems);
                }

            }
        }

        private List<NavigationItem> PopulateNavList(List<NavigationItem> items)
        {
            // Populate children recursively
            foreach (var item in items)
            {
                item.Children = Context.NavigationItems.Where(x => x.ParentNavigationItemId == item.NavigationItemId).OrderBy(x => x.Order).ToList();
                item.Href = NavigationUtils.GetNavItemUrl(item);
                item.HasChildren = item.Children.Count > 0;

                if (item.HasChildren)
                {
                    PopulateNavList(item.Children);
                }
            }

            return items;
        }
    }

    public class ParentNavViewContainer
    {
        public EditNavigationViewModel NavViewModel;
        public NavigationItem NavItem;
    }

    public class NavContainer
    {
        public NavigationItem NavItem;
        public Dictionary<int, string> OptionList;
    }
}