using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class SchemasController : DirigoBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        public ActionResult ManageSchemas()
        {
            var model = new ManageSchemasViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        public ActionResult NewSchema()
        {
            // Create a new Content Page to be passed to the edit content action
            var schema = new Schema() { DisplayName = "New Schema " };
            Context.Schemas.Add(schema);
            Context.SaveChanges();

            // Update the DisplayName with the new id we now have
            schema.DisplayName = schema.DisplayName + schema.SchemaId;
            Context.SaveChanges();

            return RedirectToAction("EditSchema", "Schemas", new { id = schema.SchemaId });
        }

        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        public ActionResult EditSchema(int id)
        {
            if (!Context.Schemas.Any(schema => schema.SchemaId == id))
            {
                return NewSchema();
            }

            var model = new EditSchemaViewModel(id);
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        public JsonResult SaveSchema(int id, string data, string name)
        {
            var result = new JsonResult();

            var schema = Context.Schemas.FirstOrDefault(x => x.SchemaId == id);

            if (schema != null)
            {
                schema.JSONData = data;
                schema.DisplayName = name;
            }

            Context.SaveChanges();
            BookmarkUtil.UpdateTitle("/admin/schemas/editschema/" + id + "/", name);

            result.Data = new { };

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        public JsonResult DeleteSchema(string id)
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
            var module = Context.Schemas.FirstOrDefault(x => x.SchemaId == moduleId);

            Context.Schemas.Remove(module);
            var success = Context.SaveChanges();

            if (success > 0)
            {
                BookmarkUtil.DeleteBookmarkForUrl("/admin/schemas/editschema/" + id + "/");
                result.Data = new { success = true, message = "The schema has been successfully deleted." };
            }

            return result;
        }


        [PermissionsFilter(Permissions = "Can Edit Pages, Can Edit Modules, Can Edit Schemas")]
        public JsonResult GetSchemaHtml(int schemaId, int moduleId, bool isPage = false)
        {
            var result = new JsonResult();
            Schema theSchema;

            string entryValues = String.Empty;

            theSchema = Context.Schemas.FirstOrDefault(x => x.SchemaId == schemaId);

            if (isPage)
            {
                ContentPage thePage = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == moduleId);
                entryValues = thePage.SchemaEntryValues;
            }
            else
            {
                ContentModule theModule = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == moduleId);
                entryValues = theModule.SchemaEntryValues;
            }

            if (theSchema != null)
            {
                result.Data = new { data = theSchema.JSONData, values = entryValues };
            }

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        [HttpGet]
        public JsonResult GetSchemaSyncData(int schemaId, int pageId)
        {
            var result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            var currentPage = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == pageId);

            if (currentPage == null)
            {
                result.Data = new { error = "Content page not found." };
                return result;
            }

            var viewHtml = currentPage.HTMLUnparsed;
            var pages = Context.ContentPages.Where(x => x.SchemaId != null && x.SchemaId == schemaId && x.IsActive.Value).ToList();
            var pagesList = new List<object>();

            foreach (var page in pages)
            {
                pagesList.Add(new
                {
                    id = page.ContentPageId,
                    schemaValues = page.SchemaEntryValues
                });
            }

            result.Data = new
            {
                schema = schemaId,
                pages = pagesList,
                html = viewHtml,
                template = currentPage.Template,
                parent = currentPage.ParentNavigationItemId
            };

            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Schemas")]
        [HttpPost]
        public JsonResult SyncSchemaView(int pageId, string parsedHtml, string unparsedHtml, string template, int parent)
        {
            var result = new JsonResult();

            var page = Context.ContentPages.FirstOrDefault(x => x.ContentPageId == pageId);

            if (page == null)
            {
                result.Data = new { success = false, error = "Content page could not be found." };
                return result;
            }

            page.HTMLContent = parsedHtml;
            page.HTMLUnparsed = unparsedHtml;
            page.Template = template;
            page.ParentNavigationItemId = parent;

            Context.SaveChanges();

            result.Data = new { success = true };

            return result;
        }
    }
}
