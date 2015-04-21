using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class ModulesController : DirigoBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult ManageModules()
        {
            var model = new ManageModulesViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult EditModule(int id)
        {
            var model = new EditModuleViewModel(id);
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult EditModuleBasic(int id)
        {
            var model = new EditModuleViewModel(id);
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public JsonResult DeleteModule(string id)
        {
            var result = new JsonResult()
            {
                Data = new { success = false, message = "There was an error processing your request." }
            };

            if (String.IsNullOrEmpty(id))
            {
                return result;
            }

            int moduleId = Int32.Parse(id);

            var module = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == moduleId);
            var revisions = Context.ContentModules.Where(x => x.ParentContentModuleId == module.ContentModuleId);
            if (revisions.Any())
            {
                Context.ContentModules.RemoveRange(revisions);
            }
            Context.ContentModules.Remove(module);
            var success = Context.SaveChanges();

            if (success > 0)
            {
                BookmarkUtil.DeleteBookmarkForUrl("/admin/modules/" + id + "/");
                result.Data = new { success = true, message = "The module has been successfully deleted." };
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult CloneModule(int id)
        {
            var newModule = new ContentModule();

            var moduleToClone = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == id);

            newModule.CreateDate = DateTime.Today;
            newModule.HTMLContent = moduleToClone.HTMLContent;
            newModule.HTMLUnparsed = moduleToClone.HTMLUnparsed;
            newModule.JSContent = moduleToClone.JSContent;

            newModule.CSSContent = moduleToClone.CSSContent;
            newModule.SchemaEntryValues = moduleToClone.SchemaEntryValues;
            newModule.SchemaId = moduleToClone.SchemaId;
            newModule.ModuleName = "New Module";

            Context.ContentModules.Add(newModule);
            Context.SaveChanges();

            // Update the Display Name
            newModule.ModuleName = "New Module " + newModule.ContentModuleId;
            Context.SaveChanges();


            return RedirectToAction("EditModule", "Modules", new { id = newModule.ContentModuleId });
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult NewContentModule()
        {
            int ModuleId = 0;

            // Create a new Content Page to be passed to the edit content action

            ContentModule module = GetDefaultContentModule();

            Context.ContentModules.Add(module);
            Context.SaveChanges();

            // Update the page title / permalink with the new id we now have
            ModuleId = module.ContentModuleId;
            module.ModuleName = "Module " + ModuleId;
            module.DraftAuthorName = UserUtils.CurrentMembershipUsername();

            Context.SaveChanges();

            return RedirectToAction("EditModule", "Modules", new { id = ModuleId });
        }

        [HttpPost]
        [Authorize(Roles = "Administrators")]
        public JsonResult SaveDraft(ContentModule entity, DateTime? publishDate)
        {
            var draft = SetContentModuleData(entity, false, publishDate);

            var originalModule = Context.ContentModules.First(module => module.ContentModuleId == entity.ContentModuleId);
            draft.ParentContentModuleId = originalModule.ParentContentModuleId ?? originalModule.ContentModuleId;

            Context.ContentModules.Add(draft);
            Context.SaveChanges();

            return new JsonResult
            {
                Data = new { id = draft.ContentModuleId }
            };
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Modules")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult UpdateModuleShort(int id, string html)
        {
            var result = new JsonResult();


            var module = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == id);
            if (module != null)
            {
                module.HTMLContent = html;
            }

            Context.SaveChanges();

            return result;
        }

        [Authorize(Roles = "Administrators")]
        public JsonResult GetRevisionHtml(int revisionId)
        {
            var result = new JsonResult();
            string html = "";


            var firstOrDefault = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == revisionId);
            if (firstOrDefault != null) html = firstOrDefault.HTMLContent;

            result.Data = new { html = html ?? String.Empty };

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public JsonResult GetRevisionList(int id)
        {
            var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            string html;

            var drafts = Context.ContentModules.Where(x => x.ParentContentModuleId == id || x.ContentModuleId == id).OrderByDescending(x => x.CreateDate).ToList().Select(rev => new RevisionViewModel
            {
                Date = rev.CreateDate,
                ContentId = rev.ContentModuleId,
                AuthorName = rev.DraftAuthorName,
                WasPublished = rev.WasPublished
            }).ToList();
            html = ContentUtils.RenderPartialViewToString("/Areas/Admin/Views/Shared/Partials/RevisionsListInnerPartial.cshtml", drafts, ControllerContext, ViewData, TempData);

            result.Data = new { html };

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Modules")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ModifyModule(ContentModule entity)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There as an error processing your request"
                }
            };

            var success = 0;

            if (String.IsNullOrEmpty(entity.ModuleName))
            {
                return result;
            }

            var editedContent = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == entity.ContentModuleId);

            if (editedContent == null)
            {
                return result;
            }

            SaveDraft(editedContent, editedContent.CreateDate);

            editedContent.DraftAuthorName = UserUtils.CurrentMembershipUsername();
            editedContent.CreateDate = DateTime.UtcNow;
            editedContent.ModuleName = ContentUtils.ScrubInput(entity.ModuleName);
            editedContent.HTMLContent = entity.HTMLContent;
            editedContent.HTMLUnparsed = entity.HTMLUnparsed;
            editedContent.JSContent = entity.JSContent;
            editedContent.CSSContent = entity.CSSContent;
            editedContent.SchemaId = entity.SchemaId;
            editedContent.SchemaEntryValues = entity.SchemaEntryValues;
            editedContent.IsActive = true;

            success = Context.SaveChanges();

            if (success > 0)
            {
                BookmarkUtil.UpdateTitle("/admin/modules/" + editedContent.ContentModuleId + "/", editedContent.ModuleName);
                result.Data = new
                {
                    success = true,
                    message = "Content saved successfully."
                };
            }

            return result;
        }

        [UserIsLoggedIn]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetModuleData(string id)
        {
            var result = new JsonResult();

            int moduleId = Int32.Parse(id);

            var editorData = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == moduleId);
            result.Data = new
            {
                html = editorData.HTMLContent,
                js = editorData.JSContent,
                css = editorData.CSSContent,
                title = editorData.ModuleName
            };

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public JsonResult UploadModuleThumb(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (!Directory.Exists(Server.MapPath("~" + ContentGlobals.MODULETHUMBNAILIMAGEUPLOADDIRECTORY)))
                {
                    Directory.CreateDirectory(Server.MapPath("~" + ContentGlobals.MODULETHUMBNAILIMAGEUPLOADDIRECTORY));
                }

                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~" + ContentGlobals.MODULETHUMBNAILIMAGEUPLOADDIRECTORY), fileName);

                file.SaveAs(physicalPath);
            }

            string imgPath = ContentGlobals.MODULETHUMBNAILIMAGEUPLOADDIRECTORY + file.FileName;

            return new JsonResult() { Data = new { path = imgPath } };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddDynamicModuleData(string name)
        {
            var result = new JsonResult();

            string html = "";
            string js = "";
            string css = "";

            // todo : should come from global registry. 
            // todo: loop over shortcode directory and call method
            // CurrentYearShortcode is the value of name
            // Look it up
            // Call the method
            MethodInfo method = this.GetType().GetMethod(name);
            if (method != null)
            {
                var shortcode = method.Invoke(this, new object[0]);
                var getHtmlValue = shortcode.GetType().GetMethod("GetHtml").ToString();
                html = getHtmlValue;
            }
            // Dynamic Modules will add themselves to the registry, then we'll just index into each object
            //if (name == "formbuilder")
            //{
            //    // Build out the form
            //    html = new FormBuilderModel().GetDefaultHtml();
            //}

            //if (name == "lodgingphotos")
            //{
            //    html = RenderPartialViewToString("/Areas/Admin/Views/Shared/Partials/LodgingPhotosPartial.cshtml", null, ControllerContext, ViewData, TempData);
            //}

            //var editorData = context.ContentModules.FirstOrDefault(x => x.ContentModuleId == moduleId);
            result.Data = new
            {
                html,
                js,
                css
            };

            return result;
        }

        #region Helper Methods
        public ContentModule GetDefaultContentModule()
        {
            return new ContentModule()
            {
                ModuleName = "Placeholder",
                HTMLContent = "<h2>My Module</h2>",
                CSSContent = ".page { \n\n}",
                JSContent = "$(document).ready(function() { \n    // Awesome Code Here\n\n});",
                CreateDate = DateTime.UtcNow
            };
        }

        protected ContentModule SetContentModuleData(ContentModule entity, bool isBasic, DateTime? createDate)
        {
            var editedContent = new ContentModule
            {
                CreateDate = createDate ?? DateTime.UtcNow,
                ModuleName = entity.ModuleName,
                HTMLContent = entity.HTMLContent,
                HTMLUnparsed = entity.HTMLUnparsed,
                SchemaId = entity.SchemaId,
                SchemaEntryValues = entity.SchemaEntryValues,
                IsActive = false,
                ParentContentModuleId = entity.ContentModuleId,
                DraftAuthorName = UserUtils.CurrentMembershipUsername(),
                WasPublished = createDate != null
            };

            // Basic Editors don't pass JS / CSS Content along
            if (!isBasic)
            {
                editedContent.JSContent = entity.JSContent;
                editedContent.CSSContent = entity.CSSContent;
            }

            return editedContent;

        }
        #endregion
    }
}