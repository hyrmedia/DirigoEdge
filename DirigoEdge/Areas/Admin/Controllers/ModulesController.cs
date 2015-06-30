using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;
using AutoMapper;
using DirigoEdge.Business.Models;
using Module = DirigoEdge.Business.Models.Module;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class ModulesController : WebBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult ManageModules()
        {
            var model = new ManageModulesViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult EditModule(int id, string editContentHeading)
        {
            var model = new EditModuleViewModel(id);

            if (!String.IsNullOrEmpty(editContentHeading))
            {
                model.Heading = editContentHeading;
            }

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

            var moduleToClone = Context.ContentModules.First(x => x.ContentModuleId == id);

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
        public ActionResult NewContentModule(string schemaId, string editContentHeading)
        {
            // Create a new Content Module to be passed to the edit content action

            ContentModule module = GetDefaultContentModule();

            // If a schema was passed in, we will want to assign that schema id to the newly created module
            // We will also want to copy over html from an existing module that uses that html. That way the user has a consistent editor.
            int iSchemaId = !string.IsNullOrEmpty(schemaId) ? Int32.Parse(schemaId) : 0;
            if (iSchemaId > 0)
            {
                module.SchemaId = iSchemaId;

                var moduleToCloneFrom = Context.ContentModules.FirstOrDefault(x => x.SchemaId == iSchemaId);
                if (moduleToCloneFrom != null)
                {
                    module.HTMLContent = moduleToCloneFrom.HTMLContent;
                    module.HTMLUnparsed = moduleToCloneFrom.HTMLUnparsed;
                    module.JSContent = moduleToCloneFrom.JSContent;
                    module.CSSContent = moduleToCloneFrom.CSSContent;
                }
            }

            Context.ContentModules.Add(module);
            Context.SaveChanges();

            // Update the module name
            var moduleId = module.ContentModuleId;
            module.ModuleName = "Module " + moduleId;
            module.DraftAuthorName = UserUtils.CurrentMembershipUsername();

            Context.SaveChanges();

            CachedObjects.GetCacheContentModules(true);

            object routeParameters = new { id = moduleId };
            if (!String.IsNullOrEmpty(editContentHeading))
            {
                routeParameters = new { id = moduleId, editContentHeading };
            }

            return RedirectToAction("EditModule", "Modules", routeParameters);
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

            CachedObjects.GetCacheContentModules(true);

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

            var module = Context.ContentModules.First(mod => mod.ContentModuleId == id);
            var parentId = module.ParentContentModuleId ?? module.ContentModuleId;

            var drafts = Context.ContentModules.Where(x => x.ParentContentModuleId == parentId || x.ContentModuleId == parentId).OrderByDescending(x => x.CreateDate).ToList().Select(rev => new RevisionViewModel
            {
                Date = rev.CreateDate,
                ContentId = rev.ContentModuleId,
                AuthorName = rev.DraftAuthorName,
                WasPublished = rev.WasPublished
            }).ToList();

            var html = ContentUtils.RenderPartialViewToString("/Areas/Admin/Views/Shared/Partials/RevisionsListInnerPartial.cshtml", drafts, ControllerContext, ViewData, TempData);

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

            if (String.IsNullOrEmpty(entity.ModuleName))
            {
                return result;
            }

            var editedContent = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == entity.ContentModuleId);

            if (editedContent == null)
            {
                return result;
            }

            if (editedContent.ParentContentModuleId.HasValue)
            {
                editedContent = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == editedContent.ParentContentModuleId.Value);
                if (editedContent == null)
                {
                    return result;
                }
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

            var success = Context.SaveChanges();

            if (success > 0)
            {
                CachedObjects.GetCacheContentModules(true);

                BookmarkUtil.UpdateTitle("/admin/modules/" + editedContent.ContentModuleId + "/", editedContent.ModuleName);
                result.Data = new
                {
                    success = true,
                    message = "Content saved successfully.",
                    date = editedContent.CreateDate.Value.ToString("dd/MM/yyy @ h:mm tt")
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

            var editorData = Context.ContentModules.First(x => x.ContentModuleId == moduleId);
            result.Data = new
            {
                html = editorData.HTMLContent,
                js = editorData.JSContent,
                css = editorData.CSSContent,
                title = editorData.ModuleName
            };

            return result;
        }



        [HttpPost]
        public JsonResult UploadModule(Module module)
        {
            try
            {
                var contentModuleId = ImportTools.AddContentModule(module);
                return new JsonResult
                {
                    Data =  new 
                    {
                        ModuleId = contentModuleId,
                        Success = true
                    }
                };
            }
            catch(Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        Success = false,
                        Error = ex.Message
                    }
                };
            }
        }

        [HttpGet]
        [UserIsLoggedIn]
        public JsonResult GetModule(int id)
        {
            try
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new {
						Modules = new List<Object>(){Mapper.Map<ContentModule, Module>(
                            Context.ContentModules.First(x => x.ContentModuleId == id))}}

                };
            }
            catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = ex.Message
                };
            }
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

        [PermissionsFilter(Permissions = "Can Edit Modules")]
        public ActionResult ManageEntity(string heading, string buttonText, string editHeading, int schemaId, string sort = "")
        {
            var model = new ManageModulesViewModel(schemaId)
            {
                Heading = heading,
                NewButtonText = buttonText,
                EditContentHeading = editHeading,
                Sort = sort
            };

            return View("ManageModules", model);
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