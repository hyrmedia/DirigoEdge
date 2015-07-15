using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class SiteSettingsViewModel : DirigoBaseModel
    {
        public SiteSettings Settings;
        public List<string> RolesList;
        public IReadOnlyCollection<TimeZoneInfo> TimeZones = TimeZoneInfo.GetSystemTimeZones();

        public Dictionary<String, String> ConfigSettings;

        public SiteSettingsViewModel()
        {
            BookmarkTitle = "Site Settings";
            RolesList = new List<string>();
            ConfigSettings = new Dictionary<string, string>();
        }

        public static SiteSettingsViewModel LoadSiteSettings(WebDataContext context)
        {
            var model = new SiteSettingsViewModel
            {
                Settings = context.SiteSettings.FirstOrDefault(),
                RolesList = Roles.GetAllRoles().ToList()
            };

            var configs = context.Configurations;
            foreach (var config in configs)
            {
               model.ConfigSettings.Add(config.Key, config.Value);
            }

            return model;
        }
    }
}