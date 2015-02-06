using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class BlogSettingsViewModel : DirigoBaseModel
    {
        public BlogSettings Settings;

        public BlogSettingsViewModel()
        {
            BookmarkTitle = "Blog Settings";
            Settings = Context.BlogSettings.FirstOrDefault();

            // Set some initial values if none are found.
            if (Settings == null)
            {
                SettingsUtils.SetDefaultBlogSettings();
                Settings = Context.BlogSettings.FirstOrDefault();
            }
        }
    }
}