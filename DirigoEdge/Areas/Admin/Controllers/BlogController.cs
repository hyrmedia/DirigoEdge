using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class BlogController : DirigoBaseAdminController
    {

        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        public ActionResult ManageBlogs()
        {
            var model = new ManageBlogsViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public ActionResult ManageBlogAuthors()
        {
            var model = new ManageBlogAuthorsViewModel();
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        public ActionResult EditBlog(string id)
        {
            var model = new EditBlogViewModel(id);
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Categories")]
        public ActionResult ManageCategories()
        {
            var model = new ManageCategoriesViewModel();
            return View(model);
        }


        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public JsonResult ModifyBlogUser(BlogUser user)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (!String.IsNullOrEmpty(user.UserId.ToString()))
            {
                var editUser = Context.BlogUsers.FirstOrDefault(x => x.UserId == user.UserId);

                editUser.DisplayName = user.DisplayName;
                editUser.UserImageLocation = user.UserImageLocation;
                editUser.IsActive = user.IsActive;

                success = Context.SaveChanges();
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Changes saved successfully."
                };
            }
            return result;
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public JsonResult AddBlogUser(BlogUser user)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (!String.IsNullOrEmpty(user.DisplayName))
            {
                var newUser = new BlogUser
                {
                    CreateDate = DateTime.UtcNow,
                    DisplayName = user.DisplayName,
                    Username = user.Username,
                    IsActive = user.IsActive,
                    UserImageLocation = user.UserImageLocation
                };
                Context.BlogUsers.Add(newUser);
                success = Context.SaveChanges();
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "User added successfully."
                };
            }
            return result;
        }


        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public JsonResult DeleteBlogUser(BlogUser user)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (!String.IsNullOrEmpty(user.UserId.ToString()))
            {
                var UserToDelete = Context.BlogUsers.FirstOrDefault(x => x.UserId == user.UserId);

                Context.BlogUsers.Remove(UserToDelete);
                success = Context.SaveChanges();
            }
            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "User deleted successfully."
                };
            };

            return result;
        }


        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        public ActionResult AddBlog()
        {
            string blogId = String.Empty;

            // Create a new blog to be passed to the edit blog action
            Blog blog = new Blog() { IsActive = false, Title = "New Blog", Date = DateTime.UtcNow, Tags = "New Blog" };

            var cats = Context.BlogCategories.ToList();

            if (cats.Any() && cats.Select(x => x.CategoryName).Any(x => x == "General"))
            {
                blog.Category = cats.First(x => x.CategoryName == "General");
            }
            else
            {
                var def = new BlogCategory()
                {
                    CategoryName = "General",
                    IsActive = true,
                    CreateDate = DateTime.UtcNow
                };

                Context.BlogCategories.Add(def);
                Context.SaveChanges();
            }

            Context.Blogs.Add(blog);
            Context.SaveChanges();

            // Update the blog title / permalink with the new id we now have
            blogId = blog.BlogId.ToString();

            blog.Title = blog.Title + " " + blogId;
            Context.SaveChanges();

            return RedirectToAction("EditBlog", "Blog", new { id = blogId });
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ModifyBlog(Blog entity)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            if (String.IsNullOrEmpty(entity.MainCategory))
            {
                result.Data = new
                {
                    success = false,
                    message = "Please select a category."
                };
            };

            var success = 0;

            if (!String.IsNullOrEmpty(entity.Title))
            {
                Blog editedBlog = Context.Blogs.FirstOrDefault(x => x.BlogId == entity.BlogId);
                if (editedBlog != null)
                {
                    editedBlog.Author = ContentUtils.ScrubInput(entity.Author);
                    editedBlog.AuthorId = entity.AuthorId;
                    editedBlog.HtmlContent = entity.HtmlContent;
                    editedBlog.ImageUrl = ContentUtils.ScrubInput(entity.ImageUrl);
                    editedBlog.IsActive = entity.IsActive;
                    editedBlog.IsFeatured = entity.IsFeatured;
                    editedBlog.Title = ContentUtils.ScrubInput(entity.Title);
                    editedBlog.PermaLink = ContentUtils.GetFormattedUrl(entity.PermaLink);

                    editedBlog.MainCategory = ContentUtils.ScrubInput(entity.MainCategory);
                    editedBlog.Tags = ContentUtils.ScrubInput(entity.Tags);
                    editedBlog.ShortDesc = entity.ShortDesc;
                    editedBlog.Date = entity.Date;

                    // Meta
                    editedBlog.Canonical = entity.Canonical;
                    editedBlog.OGImage = entity.OGImage;
                    editedBlog.OGTitle = entity.OGTitle;
                    editedBlog.OGType = entity.OGType;
                    editedBlog.OGUrl = entity.OGUrl;
                    editedBlog.MetaDescription = entity.MetaDescription;

                    success = Context.SaveChanges();
                    CachedObjects.GetCacheContentPages(true);
                    BookmarkUtil.UpdateTitle("/admin/pages/editblog/" + editedBlog.BlogId + "/", entity.Title);
                }
            }

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Blog saved successfully.",
                    id = entity.BlogId
                };
            };

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddBlog(Blog entity)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (!String.IsNullOrEmpty(entity.Title))
            {
                Context.Blogs.Add(entity);
                success = Context.SaveChanges();
            }
            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Blog created successfully.",
                    id = entity.BlogId
                };
            };
            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteBlog(string id)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            if (String.IsNullOrEmpty(id))
            {
                return result;
            }

            int blogId = Int32.Parse(id);

            var blog = Context.Blogs.FirstOrDefault(x => x.BlogId == blogId);

            Context.Blogs.Remove(blog);
            int success = Context.SaveChanges();

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Blog removed successfully."
                };

                BookmarkUtil.DeleteBookmarkForUrl("/admin/pages/editblog/" + id + "/");
            }

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Modules")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public JsonResult SaveModules(AdminModules entity)
        {
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

            if (entity != null)
            {
                var user = Membership.GetUser(HttpContext.User.Identity.Name);
                string userName = user.UserName;

                // First delete all entries for user
                var modules = Context.BlogAdminModules.Where(x => x.User.Username == userName);
                foreach (var mod in modules)
                {
                    Context.BlogAdminModules.Remove(mod);
                }

                // Then add the new modules to the user
                if (entity.AdminModulesColumn1 != null)
                {
                    foreach (var module in entity.AdminModulesColumn1)
                    {
                        var thisUser = Context.Users.FirstOrDefault(x => x.Username == userName);

                        // Make sure modules exist
                        checkNullUserModules(thisUser);

                        thisUser.BlogAdminModules.Add(module);
                    }
                }

                if (entity.AdminModulesColumn2 != null)
                {
                    foreach (var module in entity.AdminModulesColumn2)
                    {
                        var thisUser = Context.Users.FirstOrDefault(x => x.Username == userName);

                        // Make sure modules exist
                        checkNullUserModules(thisUser);

                        thisUser.BlogAdminModules.Add(module);
                    }
                }

                success = Context.SaveChanges();
            }
            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Modules updated successfully."
                };
            };
            return result;
        }

        #region Helper Methods

        private void checkNullUserModules(User thisUser)
        {
            if (thisUser.BlogAdminModules == null)
            {
                thisUser.BlogAdminModules = new List<BlogAdminModule>();
            }
        }

        #endregion
    }

    public class BlogEntity
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }
    }

    public class AdminModules
    {
        public List<BlogAdminModule> AdminModulesColumn1 { get; set; }
        public List<BlogAdminModule> AdminModulesColumn2 { get; set; }
    }
}