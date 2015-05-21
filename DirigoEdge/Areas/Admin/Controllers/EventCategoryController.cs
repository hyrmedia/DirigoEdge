using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class EventCategoryController : WebBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Events")]
        public ActionResult ManageEventCategories()
        {
            var model = new ManageEventCategoriesViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Categories")]
        public ActionResult ManageCategories()
        {
            var model = new ManageCategoriesViewModel();
            return View(model);
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Events")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddCategory(string name)
        {
            var result = new JsonResult();

            if (!String.IsNullOrEmpty(name))
            {
                var newCategory = new EventCategory
                {
                    CategoryName = name,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                Context.EventCategories.Add(newCategory);
                Context.SaveChanges();

                result.Data = new { id = newCategory.EventCategoryId };

                return result;
            }

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Events")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteCategory(string id)
        {
            var result = new JsonResult()
            {
                Data = new { success = false, message = "There was an error processing your request." }
            };

            if (!String.IsNullOrEmpty(id))
            {
                int catId = Int32.Parse(id);

                var cat = Context.EventCategories.FirstOrDefault(x => x.EventCategoryId == catId);

                Context.EventCategories.Remove(cat);
                var success = Context.SaveChanges();

                if (success > 0)
                {
                    result.Data = new { success = true, message = "The event category has been successfully deleted." };
                }
            }

            return result;
        }
    }
}
