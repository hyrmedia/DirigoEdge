using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers;
using DirigoEdgeCore.Business;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class PagesController : DirigoBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult ManageContent()
        {
            var templateViews = Directory.GetFiles(Server.MapPath("~/Areas/Admin/Views/Shared/Partials/StockPageTemplates"));

            var model = new ManageContentViewModel(templateViews);
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult ManageEntity(string heading, string buttonText, string editHeading, int schemaId, string sort = "")
        {
            var model = new ManageContentViewModel(schemaId)
            {
                Heading = heading,
                NewButtonText = buttonText,
                EditContentHeading = editHeading,
                UseTemplateSelector = false,
                Sort = sort,
            };

            return View("ManageContent", model);
        }

        /// <summary>
        ///     Update the Sort order from a list of ContentPages
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Pages")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdatePageSort(IEnumerable<ContentPage> entities)
        {
            var result = new JsonResult();

            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    ContentPage editedPage = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == entity.ContentPageId);
                    if (editedPage != null)
                    {
                        editedPage.SortOrder = entity.SortOrder;
                    }
                }

                result.Data = Context.SaveChanges();
            }
            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult NewContentPage(string schemaId, string editContentHeading)
        {
            // Create a new Content Page to be passed to the edit content action
            ContentPage page = GetDefaultContentPage();

            // If a schema was passed in, we will want to assign that schema id to the newly created page
            // We will also want to copy over html from an existing page that uses that html. That way the user has a consistent editor.
            int iSchemaId = !string.IsNullOrEmpty(schemaId) ? Int32.Parse(schemaId) : 0;
            if (iSchemaId > 0)
            {
                page.SchemaId = iSchemaId;

                var pageToCloneFrom = Context.ContentPages.FirstOrDefault(x => x.SchemaId == iSchemaId);
                if (pageToCloneFrom != null)
                {
                    page.HTMLContent = pageToCloneFrom.HTMLContent;
                    page.HTMLUnparsed = pageToCloneFrom.HTMLUnparsed;
                    page.JSContent = pageToCloneFrom.JSContent;
                    page.CSSContent = pageToCloneFrom.CSSContent;
                    page.Template = pageToCloneFrom.Template;
                    page.ParentNavigationItemId = pageToCloneFrom.ParentNavigationItemId;
                    page.SortOrder = Context.ContentPages.Where(x => x.SchemaId == iSchemaId).Max(x => x.SortOrder) + 1;
                }
            }

            Context.ContentPages.Add(page);
            Context.SaveChanges();

            // Update the page title / permalink with the new id we now have
            page.DisplayName = "Page " + page.ContentPageId;
            page.HTMLContent = ContentUtils.ReplacePageParametersInHtmlContent(page.HTMLUnparsed, page);
            Context.SaveChanges();
            CachedObjects.GetCacheContentPages(true);

            // Pass content Heading along if it exists
            object routeParameters = new { id = page.ContentPageId };
            if (!String.IsNullOrEmpty(editContentHeading))
            {
                routeParameters = new { id = page.ContentPageId, schema = schemaId, editContentHeading };
            }

            return RedirectToAction("EditContent", "Pages", routeParameters);
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult EditContent(int id, string schema, string editContentHeading)
        {
            var model = new EditContentViewModel();

            var editContentHelper = new EditContentHelper(Context);
            editContentHelper.LoadContentViewById(id, model);
            
            var userName = UserUtils.CurrentMembershipUsername();
            var user = Context.Users.First(usr => usr.Username == userName);
            model.IsBookmarked = Context.Bookmarks.Any(bookmark => bookmark.Url == Request.RawUrl && bookmark.UserId == user.UserId);

            schema = schema ?? "0";

            // If Schema is Assigned, make sure to show the Field Editor
            int schemaId = Int32.Parse(schema);
            if (schemaId > 0)
            {
                model.ShowFieldEditor = true;
            }

            if (!String.IsNullOrEmpty(editContentHeading))
            {
                model.Heading = editContentHeading;
            }

            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult EditContentBasic(int id)
        {
            var model = new EditContentViewModel();
            
            var editContentHelper = new EditContentHelper(Context);
            editContentHelper.LoadContentViewById(id, model);

            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public JsonResult DeleteContent(string id)
        {
            var result = new JsonResult()
            {
                Data = new { success = false, message = "There was an error processing your request." }
            };

            if (String.IsNullOrEmpty(id))
            {
                return result;
            }

            int pageId = Int32.Parse(id);

            var page = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == pageId);
            var revisions = Context.ContentPages.Where(x => x.ParentContentPageId == page.ContentPageId);
            Context.ContentPages.Remove(page);
            if (revisions.Any())
            {
                Context.ContentPages.RemoveRange(revisions);
            }
            var success = Context.SaveChanges();

            BookmarkUtil.DeleteBookmarkForUrl("/admin/pages/editcontent/" +  pageId + "/");

            if (success > 0)
            {
                result.Data = new { success = true, message = "The page has been successfully deleted." };
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult PreviewContent(int id)
        {
            var model = new ContentViewViewModel {ThePage = ContentLoader.GetDetailById(id)};
            model.TheTemplate = ContentLoader.GetContentTemplate(model.ThePage.Template);
            model.PageData = ContentUtils.GetFormattedPageContentAndScripts(model.ThePage.HTMLContent);

            if (model.ThePage != null)
            {
                ViewBag.IsPublished = model.IsPublished;
                return View(model.TheTemplate.ViewLocation, model);
            }

            HttpContext.Response.StatusCode = 404;
            return View("~/Views/Home/Error404.cshtml");
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Pages")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ModifyContent(ContentPage entity, bool isBasic)
        {
            var result = new JsonResult();

            if (String.IsNullOrEmpty(entity.Title))
            {
                return result;
            }

            if (String.IsNullOrEmpty(entity.Title)) return result;

            ContentPage editedContent =
                Context.ContentPages.FirstOrDefault(x => x.ContentPageId == entity.ContentPageId);
            if (editedContent == null)
            {
                return result;
            }

            SaveDraft(editedContent, editedContent.PublishDate);
            BookmarkUtil.UpdateTitle("/admin/pages/editcontent/" + editedContent.ContentPageId + "/", entity.Title);

            SetContentPageData(ref editedContent, entity, false, isBasic, null);
            editedContent.IsActive = true; // Saving / Publishing content sets this to true.
            editedContent.NoIndex = entity.NoIndex;
            editedContent.NoFollow = entity.NoFollow;

            Context.SaveChanges();

            CachedObjects.GetCacheContentPages(true);

            result.Data = new { publishDate = Convert.ToDateTime(DateTime.UtcNow).ToString("MM/dd/yyyy @ hh:mm") };

            return result;
        }


        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Pages")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SaveDraft(ContentPage entity, DateTime? publishDate)
        {
            var result = new JsonResult();
            var draft = new ContentPage();

            SetContentPageData(ref draft, entity, true, false, publishDate);

            Context.ContentPages.Add(draft);
            Context.SaveChanges();

            result.Data = new { id = draft.ContentPageId };

            return result;
        }


        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Pages")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeDraftStatus(ContentPage entity)
        {
            var result = new JsonResult();

            ContentPage editedContent = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == entity.ContentPageId);
            if (editedContent != null)
            {
                editedContent.IsActive = entity.IsActive;
            }

            // Return last publish date if we just changed to publish
            if (Convert.ToBoolean(entity.IsActive))
            {
                editedContent.PublishDate = DateTime.UtcNow;
                result.Data = new { publishDate = Convert.ToDateTime(DateTime.UtcNow).ToString("MM/dd/yyyy @ hh:mm") };
            }

            Context.SaveChanges();

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public JsonResult SetWordWrap(bool wordWrap)
        {
            var membershipUser = Membership.GetUser();

            if (membershipUser == null) return new JsonResult();

            var userName = membershipUser.UserName;

            if (String.IsNullOrEmpty(userName)) return new JsonResult();

            var firstOrDefault = Context.Users.FirstOrDefault(x => x.Username == userName);
            if (firstOrDefault != null)
                firstOrDefault.ContentAdminWordWrap = wordWrap;

            Context.SaveChanges();

            return new JsonResult();
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public JsonResult GetRevisionHtml(int revisionId)
        {
            var result = new JsonResult();
            string html = "";

            var firstOrDefault = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == revisionId);
            if (firstOrDefault != null) html = firstOrDefault.HTMLContent;

            result.Data = new { html = html ?? String.Empty };

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public JsonResult GetRevisionList(int id)
        {
            JsonResult result = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            string html = "";

            var drafts = Context.ContentPages.Where(x => x.ParentContentPageId == id || x.ContentPageId == id).OrderByDescending(x => x.PublishDate).ToList().Select(rev => new RevisionViewModel
            {
                Date = rev.PublishDate,
                ContentId = rev.ContentPageId,
                AuthorName = rev.DraftAuthorName,
                WasPublished = rev.WasPublished
            }).ToList();
            html = ContentUtils.RenderPartialViewToString("/Areas/Admin/Views/Shared/Partials/RevisionsListInnerPartial.cshtml", drafts, ControllerContext, ViewData, TempData);

            result.Data = new { html = html };

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public JsonResult AddNewPageFromTemplate(string templatePath, string viewTemplate, string permalink, string title, int parent)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing you request."
                }
            };

            // check to see if permalink exists
            if (Context.ContentPages.Any(x => x.Permalink == permalink))
            {
                result.Data = new
                {
                    success = false,
                    message = "Permalink is already in use."
                };
                return result;
            }

            var success = 0;
            var urlLink = "";
            var page = new ContentPage
            {
                Title = title,
                IsActive = false,
                CreateDate = DateTime.UtcNow,
                Permalink = permalink,
                DisplayName = permalink,
                ParentNavigationItemId = parent,
                Template = !String.IsNullOrEmpty(viewTemplate) ? viewTemplate.ToLower() : "blank",
                HTMLUnparsed = ContentUtils.RenderPartialViewToString(templatePath, null, ControllerContext, ViewData, TempData),
                HTMLContent = ContentUtils.RenderPartialViewToString(templatePath, null, ControllerContext, ViewData, TempData)
            };

            page.HTMLContent = ContentUtils.ReplacePageParametersInHtmlContent(page.HTMLUnparsed, page);

            Context.ContentPages.Add(page);
            success = Context.SaveChanges();

            var parentHref = NavigationUtils.GetNavItemUrl(parent);

            if (!String.IsNullOrEmpty(parentHref))
            {
                urlLink = parentHref + page.Permalink;
            }

            if (success > 0)
            {
                urlLink = string.IsNullOrEmpty(urlLink) ? "/" + page.Permalink : urlLink;
                result.Data = new
                {
                    id = page.ContentPageId,
                    url = urlLink,
                    success = true,
                    message = "Page created, redirecting."
                };
            }
            return result;
        }

        #region Helper Methods

        private static ContentPage GetDefaultContentPage()
        {
            return new ContentPage()
            {
                DisplayName = "PlaceHolder",
                IsActive = false,
                HTMLContent = "",
                CSSContent = "",
                JSContent = "",
                CreateDate = DateTime.UtcNow,
                IsRevision = false,
                PublishDate = DateTime.UtcNow
            };
        }

        protected void SetContentPageData(ref ContentPage editedContent, ContentPage entity, bool isRevision, bool isBasic, DateTime? publishDate)
        {
            if (isRevision)
            {
                editedContent.IsRevision = true;
                editedContent.ParentContentPageId = entity.ContentPageId;
            }
            else
            {
                editedContent.IsRevision = false;
            }

            if (!isBasic)
            {
                editedContent.JSContent = entity.JSContent;
                editedContent.CSSContent = entity.CSSContent;
            }

            editedContent.DraftAuthorName = UserUtils.CurrentMembershipUsername();
            editedContent.DisplayName = ContentUtils.ScrubInput(entity.DisplayName);
            editedContent.Permalink = ContentUtils.GetFormattedUrl(entity.Permalink);
            editedContent.HTMLContent = entity.HTMLContent;
            editedContent.HTMLUnparsed = entity.HTMLUnparsed;
            editedContent.SchemaId = entity.SchemaId;
            editedContent.SchemaEntryValues = entity.SchemaEntryValues;
            editedContent.Template = entity.Template;
            editedContent.Title = entity.Title;
            editedContent.WasPublished = publishDate.HasValue;
            editedContent.PublishDate = publishDate ?? DateTime.UtcNow;

            // SEO Related Info
            editedContent.MetaDescription = entity.MetaDescription;
            editedContent.OGTitle = entity.OGTitle;
            editedContent.OGImage = entity.OGImage;
            editedContent.OGType = entity.OGType;
            editedContent.OGUrl = entity.OGUrl;
            editedContent.Canonical = entity.Canonical;
            editedContent.NoIndex = entity.NoIndex;
            editedContent.NoFollow = entity.NoFollow;

            editedContent.ParentNavigationItemId = entity.ParentNavigationItemId;

            editedContent.HTMLContent = ContentUtils.ReplacePageParametersInHtmlContent(editedContent.HTMLUnparsed, entity);
        }

        #endregion
    }
}