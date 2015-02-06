using System;
using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class EventHomeViewModel : DirigoBaseModel
    {
        public List<Event> Events;
        public int MaxEventCount;

        public Event FeaturedEvent;
        public List<EventCatExtraData> Categories;
        public bool ReachedMaxEvents;
        public List<NavigationItem> Breadcrumbs;

        public EventHomeViewModel()
        {
            var tomorrow = DateTime.UtcNow.Date;
            Events = Context.Events.Where(x => x.IsActive == true && DateTime.Compare(x.EndDate.Value, tomorrow) >= 0).ToList();
            FeaturedEvent = Context.Events.FirstOrDefault(x => x.IsFeatured == true);
            Categories = new List<EventCatExtraData>();

            var cats = Context.EventCategories.Where(x => x.IsActive == true).ToList();
            foreach (var cat in cats)
            {
                int count = Context.Events.Count(x => x.MainCategory == cat.CategoryName);
                Categories.Add(new EventCatExtraData() { TheCategory = cat, EventCount = count });
            }
        }
    }

    public class EventCatExtraData
    {
        public EventCategory TheCategory;
        public int EventCount;
    }
}