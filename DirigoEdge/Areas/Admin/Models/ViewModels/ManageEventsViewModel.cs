using System.Collections.Generic;
using System.Linq;
using DirigoEdge.CustomUtils;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageEventsViewModel : DirigoBaseModel
    {
        public List<Event> EventListing;

        public void LoadEvents()
        {
            EventListing = new List<Event>();

            EventListing = Context.Events.ToList();
            
            foreach (var edgeEvent in EventListing)
            {
                if (edgeEvent.StartDate.HasValue)
                {
                    edgeEvent.StartDate = TimeUtils.ConvertUTCToLocal(edgeEvent.StartDate.Value);
                }

                if (edgeEvent.EndDate.HasValue)
                {
                    edgeEvent.EndDate = TimeUtils.ConvertUTCToLocal(edgeEvent.EndDate.Value);
                }
            }
            }
    }

    
}