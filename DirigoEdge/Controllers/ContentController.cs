using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DirigoEdgeCore.Business;
using DirigoEdgeCore.Business.Models;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Controllers
{
    public class ContentController : DirigoBaseController
    {
        static ContentController()
        {
            Mapper.CreateMap<ContentPage, PageDetails>();
        }

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
                model = new ContentViewViewModel { ThePage = ContentLoader.GetDetailsByTitle(title) };
                model.TheTemplate = ContentLoader.GetContentTemplate(model.ThePage.Template);
                model.PageData = ContentUtils.GetFormattedPageContentAndScripts(model.ThePage.HTMLContent, Context);
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
                    ViewBag.NoFollow = "nofollow";
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

        private ContentViewViewModel GetSubDirectoryModel(string path)
        {
            var masterList = CachedObjects.GetMasterNavigationList(Context);
            var pathPieces = path.Split('/');
            var permalink = pathPieces.Last().ToLower();

            var currentPath = "/" + pathPieces[0] + "/";
            var currentNavigation = masterList.FirstOrDefault(x => String.Equals(x.Href, currentPath, StringComparison.CurrentCultureIgnoreCase));

            foreach (var piece in pathPieces.Skip(1).Take(pathPieces.Count() - 2))
            {
                if (currentNavigation == null) break;
                currentPath = currentPath + piece + "/";
                currentNavigation = currentNavigation.Children.FirstOrDefault(x => String.Equals(x.Href, currentPath, StringComparison.CurrentCultureIgnoreCase));
            }

            if (currentNavigation == null) return null;

            var thePage = Context.ContentPages.FirstOrDefault(x => x.Permalink == permalink && x.ParentNavigationItemId == currentNavigation.NavigationItemId);

            if (thePage != null)
            {
                var model = new ContentViewViewModel {ThePage = ContentLoader.GetDetailById(thePage.ContentPageId)};
                model.TheTemplate = ContentLoader.GetContentTemplate(model.ThePage.Template);
                model.PageData = ContentUtils.GetFormattedPageContentAndScripts(model.ThePage.HTMLContent, Context);
                return new ContentViewViewModel();
            }
            else
            {
                return null;
            }
        }

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
        }
    }
}
