using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class FeatureSettingsViewModel : DirigoBaseModel
    {
        public FeatureSettings Settings;

        public FeatureSettingsViewModel()
        {
            BookmarkTitle = "Enable Features";
            Settings = Context.FeatureSettings.FirstOrDefault();

            // Set some initial values if none are found.
            if (Settings == null)
            {
                Settings = new FeatureSettings()
                {
                    EventsEnabled = false
                };

                Context.FeatureSettings.Add(Settings);

                Context.SaveChanges();
            }
        }
    }
}