using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirigoEdge.Models.Shortcodes
{
    public class EventsModule : IShortcode
    {
        public List<Event> TheEvents;
        public Boolean ShowHeading;

        public string GetDisplayName()
        {
            return "Events Module";
        }

        public string GetCSSClass()
        {
            return "eventsModule no-edit-bar";
        }

        public string GetHtml(Dictionary<string, string> parameters)
        {
            TheEvents = new List<Event>();
            ShowHeading = (parameters != null && parameters.ContainsKey("showHeading") ? parameters["showHeading"] != "false" : true);
            var events = new List<string>();

            using (var context = new DataContext())
            {
                // event name(s) are supplied, only select by names
                if (parameters != null && parameters.ContainsKey("name"))
                {
                    if (parameters["name"].Contains(" "))
                    {
                        events.AddRange(parameters["name"].ToLower().Split(' ').ToList());
                    }
                    else
                    {
                        events.Add(parameters["name"].ToLower().Replace(" ", ""));
                    }

                    TheEvents =
                        context.Events.Where(x => x.IsActive && events.Contains(x.Title.Replace(" ", "").ToLower())).ToList();
                }
                else if (parameters != null && parameters.ContainsKey("id"))
                {
                    var eventId = Convert.ToInt16(parameters["id"]);
                    TheEvents = context.Events.Where(x => x.IsActive && x.EventId == eventId).ToList();
                }
                else
                {
                    var count = (parameters != null && parameters.ContainsKey("count") ? Int32.Parse(parameters["count"]) : 4);

                    var query = "SELECT * FROM dbo.Events WHERE Events.IsActive = 1";
                    if (parameters != null && parameters.ContainsKey("featured"))
                    {
                        query += " AND Events.IsFeatured = " + parameters["featured"].ToLower() == "true" ? "1" : "0";
                    }
                    // categories have been supplied, could be multiples
                    if (parameters != null && parameters.ContainsKey("category"))
                    {
                        if (parameters["category"].Contains(" "))
                        {
                            events.AddRange(parameters["category"].ToLower().Split(' ').ToList());
                        }
                        else
                        {
                            events.Add(parameters["category"].ToLower().Replace(" ", ""));
                        }
                        query += " AND " + string.Format("(Events.MainCategory IS NOT NULL AND LOWER(REPLACE(Events.MainCategory, N' ', N'')) IN ({0}))", String.Join(",", events.Select(x => "'" + x + "'")));
                    }
                    var startDate = (parameters != null && parameters.ContainsKey("startDate") ? DateTime.Parse(parameters["startDate"]) : DateTime.UtcNow.Date);
                    var endDate = (parameters != null && parameters.ContainsKey("endDate") ? DateTime.Parse(parameters["endDate"]) : DateTime.MaxValue);
                    query += " AND " + string.Format("(Events.StartDate IS NULL OR Events.StartDate <= '{0}') AND (Events.EndDate IS NULL OR Events.EndDate >= '{1}')", endDate, startDate);
                    query += " ORDER BY Events.StartDate";

                    TheEvents = context.Database.SqlQuery<Event>(query).Take(count).ToList();
                }

                return TheEvents.Any() ? DynamicModules.GetViewHtml("Partials/FeaturedEventsShortcodePartial", this) : String.Empty;
            }
        }

        public object GetViewModel()
        {
            return null;
        }

        public string GetPartialView()
        {
            return null;
        }

        public bool PageBuilderEnabled()
        {
            return false;
        }

        public bool IsCacheable
        {
            get { return false; }
        }
    }
}
