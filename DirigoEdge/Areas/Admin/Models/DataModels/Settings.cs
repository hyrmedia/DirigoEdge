namespace DirigoEdge.Areas.Admin.Models.DataModels
{
    public class Settings
    {
        public string ContactEmail { get; set; }
        public bool SearchIndex { get; set; }
        public string GoogleAnalyticsId { get; set; }
        public string GoogleAnalyticsType { get; set; }
        public string DefaultUserRole { get; set; }
        public string TimeZoneId { get; set; }
    }
}