using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;
using Newtonsoft.Json;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class CategoryController : DirigoBaseAdminController
    { 
        public class Category
        {
            public int Id { get; set; }
            public String Name { get; set; }
        }

        public JsonResult All()
        {
            var categories = Context.BlogCategories.ToList()
                .Select(
                    cat => new Category
                    {
                        Id = cat.CategoryId,
                        Name = cat.CategoryName
                    }
                 ).ToList();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = JsonConvert.SerializeObject(categories)
            };
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blog Categories")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddCategory(string name)
        {
            var result = new JsonResult
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;
            int theId = 0;

            if (!String.IsNullOrEmpty(name))
            {
                var newCategory = new BlogCategory
                {
                    CategoryName = name,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    CategoryNameFormatted = ContentUtils.GetFormattedUrl(name)
                };

                Context.BlogCategories.Add(newCategory);
                success = Context.SaveChanges();
                theId = newCategory.CategoryId;
            }
            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Category added successfully.",
                    id = theId
                };
            }
            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blog Categories")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteCategory(int id, int newId)
        {
            var result = new JsonResult
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;


            var cat = Context.BlogCategories.FirstOrDefault(x => x.CategoryId == id);
            var newCat = Context.BlogCategories.First(x => x.CategoryId == newId);
            Context.SaveChanges();

            // did we find a category
            if (cat != null)
            {
                // find all posts with this category and change to Uncategorized
                foreach (var x in Context.Blogs.Where(x => x.Category.CategoryName == cat.CategoryName))
                {
                    x.Category = newCat;
                }

                Context.BlogCategories.Remove(cat);
                success = Context.SaveChanges();
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    newCount = Context.Blogs.Count(x => x.Category.CategoryId == newId),
                    message = "Category removed successfully. Blog posts changed: " + (success - 1)
                };
            }
            return result;
        }
    }
}
