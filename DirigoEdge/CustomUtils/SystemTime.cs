using System;
using System.Linq;
using DirigoEdge.Data.Context;
using DirigoEdge.Models;

namespace DirigoEdge.CustomUtils
{
    public static class SystemTime
    {
        private static TimeZoneInfo LocalTimeZoneInfo { get; set; }

        public static TimeSpan LocalOffset
        {
            get
            {
                if (LocalTimeZoneInfo == null)
                {
                    SetLocalTimeZoneFromConfig();
                }

                return LocalTimeZoneInfo.BaseUtcOffset;
            }
        }

        public static TimeZoneInfo LocalTimeZone
        {
            get
            {
                if (LocalTimeZoneInfo == null)
                {
                    SetLocalTimeZoneFromConfig();
                }

                return LocalTimeZoneInfo;
            }
        }

        private static void SetLocalTimeZoneFromConfig()
        {
            try
            {
                using (var context = new WebDataContext())
                {
                    var zoneId =
                        context.Configurations.First(config => config.Key == ConfigSettings.TimeZone.ToString())
                            .Value;
                    LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zoneId);

                    if (LocalTimeZoneInfo == null)
                    {
                        LocalTimeZoneInfo = TimeZoneInfo.Utc;
                    }
                }
            }
            catch
            {
                LocalTimeZoneInfo = TimeZoneInfo.Utc;
            }
        }
    }
}