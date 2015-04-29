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
using Newtonsoft.Json;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class BlogController : DirigoBaseAdminController
    {
        public BlogUtils Utils;

        public BlogController()
        {
            Utils = new BlogUtils(Context);
        }

        private JsonResult JsonErrorResult
        {
            get
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "There was an error processing your request."
                    }
                };
            }
        }

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
            try
            {
                var model = new EditBlogViewModel(id);
                return View(model);
            }
            catch (Exception)
            {
                return Redirect("/admin/blog/manageblogs/");
            }
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Categories")]
        public ActionResult ManageCategories()
        {
            var model = new ManageCategoriesViewModel();
            return View(model);
        }

        public class BlogAuthor
        {
            public String Username;
            public String DisplayName;
            public int Id;
        }

        public JsonResult GetAllBlogAuthors()
        {
            var users = Context.BlogUsers.ToList()
                        .Select(
                            user => new BlogAuthor
                            {
                                Id = user.UserId,
                                Username = user.Username,
                                DisplayName = user.DisplayName
                            }
                         ).ToList();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = JsonConvert.SerializeObject(users)
            };
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public JsonResult ModifyBlogUser(BlogUser user)
        {
            var success = 0;

            if (!String.IsNullOrEmpty(user.UserId.ToString()))
            {
                var editUser = Context.BlogUsers.FirstOrDefault(x => x.UserId == user.UserId);

                if (editUser == null)
                {
                    return JsonErrorResult;
                }

                editUser.DisplayName = user.DisplayName;
                editUser.UserImageLocation = user.UserImageLocation;
                editUser.IsActive = user.IsActive;

                success = Context.SaveChanges();
            }

            if (success > 0)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Changes saved successfully."
                    }
                }
                ;
            }
            return JsonErrorResult;
        }

        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public JsonResult AddBlogUser(BlogUser user)
        {
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
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "User added successfully."
                    }
                };
            }

            return JsonErrorResult;
        }


        [PermissionsFilter(Permissions = "Can Edit Blog Authors")]
        public JsonResult DeleteBlogUser(BlogUser user)
        {
            var newUser = Context.BlogUsers.First(usr => usr.UserId == newUserId);
            foreach (var blog in Context.Blogs.Where(x => x.BlogAuthor.UserId == userId))
                blog.BlogAuthor = newUser;
            }

            var userToDelete = Context.BlogUsers.FirstOrDefault(x => x.UserId == userId);
            Context.BlogUsers.Remove(userToDelete);
            var success = Context.SaveChanges();

            if (success > 0)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "User deleted successfully."
                    }
                };
            }

            return JsonErrorResult;
        }


        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        public ActionResult AddBlog()
        {
            // Create a new blog to be passed to the edit blog action
            Blog blog = new Blog
            {
                IsActive = false,
                Title = "New Blog",
                Date = DateTime.UtcNow,
                Tags = new List<BlogTag> { Utils.GetNewBlogTag() },
                BlogAuthor = Context.BlogUsers.First(usr => usr.UserId == 1) // This is anonymous and can't be deleted
            };

            var cat = Utils.GetUncategorizedCategory();
            blog.Category = cat;

            Context.Blogs.Add(blog);
            Context.SaveChanges();

            // Update the blog title / permalink with the new id we now have
            var blogId = blog.BlogId.ToString();

            blog.Title = blog.Title + " " + blogId;
            Context.SaveChanges();

            return RedirectToAction("EditBlog", "Blog", new { id = blogId });
        }

        public class EditBlogModel
        {
            public int AuthorId { get; set; }
            public int BlogId { get; set; }
            public String Title { get; set; }
            public String Tags { get; set; }
            public String HtmlContent { get; set; }
            public String Category { get; set; }
            public String ImageUrl { get; set; }
            public Boolean IsActive { get; set; }
            public Boolean IsFeatured { get; set; }
            public String PermaLink { get; set; }
            public String ShortDesc { get; set; }
            public DateTime Date { get; set; }
            public String Canonical { get; set; }
            public String OGImage { get; set; }
            public String OGTitle { get; set; }
            public String OGType { get; set; }
            public String OGUrl { get; set; }
            public String MetaDescription { get; set; }

        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ModifyBlog(EditBlogModel entity)
        {
            if (String.IsNullOrEmpty(entity.Title))
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "Your post must have a title"
                    }
                };
            }

            var editedBlog = Context.Blogs.FirstOrDefault(x => x.BlogId == entity.BlogId);

            if (editedBlog == null)
            {
                return JsonErrorResult;
            }

            // Straight copies from the model
            editedBlog.AuthorId = entity.AuthorId;
            editedBlog.HtmlContent = entity.HtmlContent;
            editedBlog.IsActive = entity.IsActive;
            editedBlog.IsFeatured = entity.IsFeatured;
            editedBlog.ShortDesc = entity.ShortDesc;
            editedBlog.Date = entity.Date;
            // Meta
            editedBlog.Canonical = entity.Canonical;
            editedBlog.OGImage = entity.OGImage;
            editedBlog.OGTitle = entity.OGTitle;
            editedBlog.OGType = entity.OGType;
            editedBlog.OGUrl = entity.OGUrl;
            editedBlog.MetaDescription = entity.MetaDescription;

            // Cleaned inpuit
            editedBlog.Title = ContentUtils.ScrubInput(entity.Title);
            editedBlog.ImageUrl = ContentUtils.ScrubInput(entity.ImageUrl);
            editedBlog.PermaLink = ContentUtils.GetFormattedUrl(entity.PermaLink);

            // Database Nav property mappings
            editedBlog.Category = Utils.GetCategoryOrUncategorized(entity.Category);
            editedBlog.BlogAuthor = Context.BlogUsers.First(usr => usr.UserId == entity.AuthorId);

            if (editedBlog.Tags == null)
            {
                editedBlog.Tags = new List<BlogTag>();
            }

            if (!String.IsNullOrEmpty(entity.Tags))
            {
                foreach (var tag in entity.Tags.Split(','))
                {
                    editedBlog.Tags.Add(Utils.GetOrCreateTag(tag));
                }
            }

            var success = Context.SaveChanges();
            CachedObjects.GetCacheContentPages(true);
            BookmarkUtil.UpdateTitle("/admin/pages/editblog/" + editedBlog.BlogId + "/", entity.Title);

            if (success > 0)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Blog saved successfully.",
                        id = entity.BlogId
                    }
                };
            }

            return JsonErrorResult;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddBlog(Blog entity)
        {
            var result = JsonErrorResult;

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
            }
            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Blogs")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteBlog(string id)
        {
            var result = JsonErrorResult;

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
        public JsonResult SaveModules(AdminModules moduleLists)
        {
            var user = Membership.GetUser(HttpContext.User.Identity.Name);

            if (moduleLists == null || user == null)
            {
                return JsonErrorResult;
            }

            var userName = user.UserName;
            Context.BlogAdminModules.RemoveRange(Context.BlogAdminModules.Where(x => x.User.Username == userName));

            var thisUser = Context.Users.FirstOrDefault(x => x.Username == userName);
            CheckNullUserModules(thisUser);

            UpdateModulesUser(moduleLists.AdminModulesColumn1, thisUser);
            UpdateModulesUser(moduleLists.AdminModulesColumn2, thisUser);

            var success = Context.SaveChanges();

            if (success > 0)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = true,
                        message = "Modules updated successfully."
                    }
                };
            }

            return JsonErrorResult;
        }

        private void UpdateModulesUser(List<BlogAdminModule> modules, User thisUser)
        {
            if (modules == null)
            {
                return;
            }

            foreach (var module in modules)
            {
                module.User = thisUser;
                Context.BlogAdminModules.Add(module);
            }
        }

        #region Helper Methods

        private static void CheckNullUserModules(User thisUser)
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