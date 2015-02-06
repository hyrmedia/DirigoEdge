using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class SiteSettingsViewModel : DirigoBaseModel
    {
        public SiteSettings Settings;
        public Dictionary<int, bool> SiteRetensionTimeValues; // Count / IsSelected
        public List<string> RolesList;

        public SiteSettingsViewModel()
        {
            BookmarkTitle = "Site Settings";
            Settings = Context.SiteSettings.FirstOrDefault();

            // Set some initial values if none are found.
            if (Settings == null)
            {
                Settings = new SiteSettings()
                {
                    SearchIndex = true
                };

                Context.SiteSettings.Add(Settings);

                Context.SaveChanges();
            }

            SiteRetensionTimeValues = new Dictionary<int, bool>
				{
					{ 5, Settings.ContentPageRevisionsRetensionCount == 5 },
					{ 10, Settings.ContentPageRevisionsRetensionCount == 10 },
					{ 25, Settings.ContentPageRevisionsRetensionCount == 25 },
					{ 50, Settings.ContentPageRevisionsRetensionCount == 50 }
				};

            RolesList = Roles.GetAllRoles().ToList();
        }
    }
}