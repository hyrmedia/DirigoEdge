using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using DirigoEdge.Utils;
using DirigoEdge.Utils.Logging;

namespace DirigoEdge.Models
{
    public class DirigoBaseModel
    {
        protected ILog Log;
        protected DataContext Context;
        protected SiteSettingsUtils SettingsUtils;
        protected NavigationUtils NavigationUtils;
        protected ContentUtils ContentUtils;

        public string TimeZone;

        public string BookmarkTitle = String.Empty;
        public bool IsBookmarked = false;

        public DirigoBaseModel() : this(new DataContext())
        {
        }

        public DirigoBaseModel(DataContext context)
        {
            Context = context;
            SettingsUtils = new SiteSettingsUtils(Context);
            NavigationUtils = new NavigationUtils(Context);
            ContentUtils = new ContentUtils(Context);
            TimeZone = UserUtils.GetCurrentUserTimeZone(context);
            Log = LogFactory.GetLog(GetType());
            
            var username = Membership.GetUser().UserName;
            var user = Context.Users.FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                var bookmark = context.Bookmarks.FirstOrDefault(b => b.Url == HttpContext.Current.Request.Url.AbsolutePath && b.UserId == user.UserId);
                if (bookmark != null)
                {
                    IsBookmarked = true;
                    BookmarkTitle = bookmark.Title;
                }
            }
        }
    }
}