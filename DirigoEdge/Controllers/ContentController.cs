using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DirigoEdge.CustomUtils;
using DirigoEdgeCore.Business;
using DirigoEdgeCore.Business.Models;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Controllers
{
    public class ContentController : WebBaseController
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
                model = new ContentViewViewModel { ThePage = ContentLoader.GetDetailsByTitle(title) };
                
            }

            // If we found a hit, return the view, otherwise 404
            if (model.ThePage != null)
            {
                model.TheTemplate = ContentLoader.GetContentTemplate(model.ThePage.Template);
                model.PageData = ContentUtils.GetFormattedPageContentAndScripts(model.ThePage.HTMLContent);

                if (UserUtils.UserIsAdmin())
                {

                    var userName = UserUtils.CurrentMembershipUsername();
                    var user = Context.Users.First(usr => usr.Username == userName);

                    var pageModel = new EditContentViewModel();
                    var editContentHelper = new EditContentHelper(Context);
                    editContentHelper.LoadContentViewById(model.ThePage.ContentPageId, pageModel);

                    pageModel.BookmarkTitle = model.ThePage.Title;
                    pageModel.IsBookmarked =
                        Context.Bookmarks.Any(
                            bookmark =>
                                bookmark.Title == title && bookmark.Url == Request.RawUrl &&
                                bookmark.UserId == user.UserId);
                    

                    ViewBag.PageModel = pageModel;
                }

                ViewBag.IsPage = true;
                ViewBag.PageId = model.ThePage.ContentPageId;
                ViewBag.IsPublished = model.ThePage.IsActive;
                ViewBag.OGType = model.ThePage.OGType ?? "website";
                ViewBag.MetaDesc = model.ThePage.MetaDescription ?? "";
                ViewBag.Title = model.ThePage.Title;
                ViewBag.OGTitle = model.ThePage.Title ?? model.ThePage.OGTitle;
                ViewBag.OGImage = model.ThePage.OGImage ?? "";

                // Set the page Canonical Tag and OGURl
                ViewBag.Canonical = GetCanonical(model.ThePage);
                ViewBag.OGUrl = model.ThePage.OGUrl ?? ViewBag.Canonical;

                ViewBag.Index = model.ThePage.NoIndex ? "noindex" : "index";
                ViewBag.Follow = model.ThePage.NoFollow ? "nofollow" : "follow";

                return View(model.TheTemplate.ViewLocation, model);
            }

            model = new ContentViewViewModel { ThePage = ContentLoader.GetDetailsByTitle("404") };

            model.TheTemplate = ContentLoader.GetContentTemplate(model.ThePage.Template);
            model.PageData = ContentUtils.GetFormattedPageContentAndScripts(model.ThePage.HTMLContent);

            ViewBag.IsPage = true;
            ViewBag.PageId = model.ThePage.ContentPageId;
            ViewBag.IsPublished = model.ThePage.IsActive;
            ViewBag.Title = model.ThePage.Title;
            ViewBag.Index = "noindex";
            ViewBag.Follow = "nofollow";

            HttpContext.Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View(model.TheTemplate.ViewLocation, model);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResponsiveImageTemplate(ResponsiveImageUtils.ResponsiveImageObject imageObject)
        {
            var result = new JsonResult();

            try
            {
                var html = ContentUtils.RenderPartialViewToString(
                    "~/Views/Shared/Partials/_ResponsiveImagePartial.cshtml", imageObject, ControllerContext, ViewData,
                    TempData);

                result.Data = new { success = true, html };
            }
            catch (Exception err)
            {
                result.Data = new { success = false, error = err };
            }

            return result;
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
            var masterList = CachedObjects.GetCacheNavigationList(1);
            var pathPieces = path.Split('/');
            var permalink = pathPieces.Last().ToLower();

            var currentPath = "/" + pathPieces[0] + "/";
            var currentNavigation = masterList.FirstOrDefault(x => String.Equals(x.Href, currentPath, StringComparison.CurrentCultureIgnoreCase));

            foreach (var piece in pathPieces.Skip(1).Take(pathPieces.Count() - 2))
            {
                if (currentNavigation == null) break;
                currentPath = currentPath + piece + "/";
                currentNavigation =
                    currentNavigation.Children.FirstOrDefault(
                        x => String.Equals(x.Href, currentPath, StringComparison.CurrentCultureIgnoreCase));
            }

            if (currentNavigation == null) return null;

            var thePage = CachedObjects.GetCacheContentPages().FirstOrDefault(x => x.Permalink == permalink && x.ParentNavigationItemId == currentNavigation.NavigationItemId);

            if (thePage == null)
            {
                thePage = Context.ContentPages.FirstOrDefault(x => x.Permalink == permalink && x.ParentNavigationItemId == currentNavigation.NavigationItemId);
            }

            if (thePage == null) return null;

            var model = new ContentViewViewModel { ThePage = ContentLoader.GetDetailById(thePage.ContentPageId) };

            return model;
        }

        public string GetCanonical(PageDetails page)
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
