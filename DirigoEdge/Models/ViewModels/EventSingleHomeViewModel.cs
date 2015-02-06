using System.Linq;
using System.Web;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;

namespace DirigoEdgeCore.Models.ViewModels
{
    public class EventSingleHomeViewModel : DirigoBaseModel
    {
        public Event TheEvent;

        public string EventAbsoluteUrl;

        public EventSingleHomeViewModel(string title)
        {
            // Try permalink first
            TheEvent = Context.Events.FirstOrDefault(x => x.PermaLink == title);

            // If no go then try title as a final back up
            if (TheEvent == null)
            {
                title = title.Replace(ContentGlobals.BLOGDELIMMETER, " ");
                TheEvent = Context.Events.FirstOrDefault(x => x.Title == title);

                // Go ahead and set the permalink for future reference
                TheEvent.PermaLink = ContentUtils.GetFormattedUrl(TheEvent.Title);
                Context.SaveChanges();
            }

            // Absolute Url for FB Like Button
            EventAbsoluteUrl = HttpContext.Current.Request.Url.AbsoluteUri;
        }
    }
}