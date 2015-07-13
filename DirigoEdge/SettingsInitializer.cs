using System;
using System.Linq;
using DirigoEdge.Data.Context;
using DirigoEdge.Data.Entities;
using DirigoEdge.Models;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge
{
    public static class SettingsInitializer
    {
        public static void EnsureRequiredSettingsExist()
        {
            using (var context = new WebDataContext())
            {
                InitSiteSettings(context);
                InitTimeZone(context);
                context.SaveChanges();
            }
        }

        private static void InitSiteSettings(DataContext context)
        {
            var siteSettings = context.SiteSettings.FirstOrDefault();
            if (siteSettings != null)
            {
                return;
            }

            siteSettings = new SiteSettings
            {
                SearchIndex = true
            };
            context.SiteSettings.Add(siteSettings);
        }

        private static void InitTimeZone(WebDataContext context)
        {
            var timeZone = context.Configurations.FirstOrDefault();
            if (timeZone != null)
            {
                return;
            }

            var setting = new SiteConfiguration
            {
                Key = ConfigSettings.TimeZone.ToString(),
                Value = TimeZoneInfo.Utc.Id
            };

            context.Configurations.Add(setting);
        }
    }
}