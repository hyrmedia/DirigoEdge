using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Controllers
{
    public class ContentController : DirigoBaseController
    {
        public ActionResult Index()
        {
            ContentViewViewModel model = null;

            //Remove query string
            var thisUri = new Uri(Request.Url.GetLeftPart(UriPartial.Path));

            // Check for content pages before returning a 404
            var title = GetPageTitle(thisUri);

            // If url has a subdirectory, try the master url list to see if it is a child page
            bool hasSubDirectory = title.Contains("/");
            if (hasSubDirectory)
            {
                model = GetSubDirectoryModel(title);
            }

            // If not a subdirectory try based on permalink / title
            if (model == null || model.ThePage == null)
            {
                model = new ContentViewViewModel(title);
            }

            // Last ditch try based strictly on the right-most url piece. 
            // Ex: /parent/child/myUrl will dp a lookup on "myUrl" and return a page if it is the *only* page with that permalink
            // todo: use recusion to check parent nav items
            if (model.ThePage == null && title.Split('/').Count() >= 3)
            {
                string lastDitchTitle = title.Split('/').LastOrDefault();

                var page = Context.ContentPages.Where(x => x.Permalink == lastDitchTitle && x.IsActive == true);
                if (page.Count() == 1)
                {
                    model = new ContentViewViewModel(lastDitchTitle, true);
                }
            }

            // If we found a hit, return the view, otherwise 404
            if (model.ThePage != null)
            {
                if (UserUtils.UserIsAdmin())
                {

                    var userName = UserUtils.CurrentMembershipUsername();
                    var user = Context.Users.First(usr => usr.Username == userName);

                   var pageModel = new EditContentViewModel(model.ThePage.ContentPageId)
                   {
                       BookmarkTitle = model.ThePage.Title,
                       IsBookmarked = Context.Bookmarks.Any(bookmark => bookmark.Title == title && bookmark.Url == Request.RawUrl && bookmark.UserId == user.UserId)
                   };


                   
                    ViewBag.PageModel = pageModel;
                }

                ViewBag.IsPage = true;
                ViewBag.OGType = model.ThePage.OGType ?? "website";
                ViewBag.MetaDesc = model.ThePage.MetaDescription;
                ViewBag.Title = model.ThePage.Title;
                ViewBag.OGTitle = model.ThePage.OGTitle ?? model.ThePage.Title;

                // Set the page Canonical Tag and OGURl
                ViewBag.OGUrl = model.ThePage.OGUrl ?? GetCanonical(model.ThePage);
                ViewBag.Canonical = GetCanonical(model.ThePage);

                if (model.ThePage.NoIndex)
                {
                    ViewBag.NoIndex = "noindex";
                }
                if (model.ThePage.NoFollow)
                {
                    ViewBag.NoIndex = "nofollow";
                }

                return View(model.TheTemplate.ViewLocation, model);
            }

            HttpContext.Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View("~/Views/Home/Error404.cshtml");
        }

        private static string GetPageTitle(Uri thisUri)
        {
            string title = thisUri.PathAndQuery;

            // remove beginning and ending slashes from uri
            if (title.StartsWith("/"))
            {
                title = title.Substring(1);
            }
            if (title.EndsWith("/"))
            {
                title = title.Substring(0, title.Length - 1);
            }
            return title;
        }

        private ContentViewViewModel GetSubDirectoryModel(string title)
        {
            var masterList = CachedObjects.GetMasterNavigationList(Context);
            string leftUrl = title.Split(new[] { '/' }).FirstOrDefault();
            string rightUrl = title.Split(new[] { '/' }).LastOrDefault();

            var urlItem = masterList.FirstOrDefault(x => x.Href.ToLower().Replace("/", "") == leftUrl.ToLower());

            if (urlItem == null)
            {
                return null;
            }
            var thePage = Context.ContentPages.FirstOrDefault(x => x.Permalink == rightUrl && x.ParentNavigationItemId == urlItem.NavigationItemId);

            return thePage != null 
                ? new ContentViewViewModel(thePage.ContentPageId) 
                : null;
        }

        // Helper methods
        public string GetCanonical(ContentPage page)
        {
            // If canonical is explicitly set, use that
            if (!String.IsNullOrEmpty(page.Canonical))
            {
                return page.Canonical;
            }

            // otherwise generate canonical
            string generatedBaseUrl = System.Web.HttpContext.Current.Request.Url.Scheme + "://" + System.Web.HttpContext.Current.Request.Url.Authority + System.Web.HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/"; // Fallback if site settings isn't in place
            string baseUrl = SettingsUtils.GetSiteBaseUrl() ?? generatedBaseUrl;

            // Use Permalink if it's available
            if (String.IsNullOrEmpty(page.Permalink))
            {
                return baseUrl + page.DisplayName + "/";
            }

            var generatedUrl = NavigationUtils.GetGeneratedUrl(page);

            if (generatedUrl == "/home/")
            {
                generatedUrl = generatedUrl.Replace("/home/", "/");
            }

            return baseUrl.TrimEnd('/') + generatedUrl;

            // Otherwise use DisplayName
        }
    }
}
