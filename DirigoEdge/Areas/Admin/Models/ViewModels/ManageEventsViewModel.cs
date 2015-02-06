using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageEventsViewModel : DirigoBaseModel
    {
        public List<Event> EventListing;

        public ManageEventsViewModel()
        {
            EventListing = Context.Events.ToList();
            if (EventListing.Count == 0)
            {
                EventListing = Context.Events.ToList();
            }
        }
    }
}