using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Utils;
using Delete = Microsoft.Ajax.Utilities.Delete;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class BookmarkController : DirigoBaseAdminController
    {
        public ActionResult Index()
        {
            var model = new BookmarkViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(String url, String title)
        {
            var result = new JsonResult
            {
                Data = new { response = "error", error = "An error occurred" }
            };

            if (String.IsNullOrEmpty(title))
            {
                title = url;
            }

            var user= UserUtils.GetCurrentUser(Context);

            if (Context.Bookmarks.Any(bkmrk => bkmrk.UserId == user.UserId && bkmrk.Url == url))
            {
                result.Data = new { response = "warning", data = "Bookmark already exists" };
            }

            try
            {
                var bookmark = BookmarkUtil.InsertBookmark(url, title, user);
                result.Data = new { response = "success", data = new {bookmark.BookmarkId, bookmark.Url, bookmark.Title} };
            }
            catch (Exception err)
            {
                result.Data = new { response = "error", error = err };
            }

            return result;
        }

        [HttpPost]
        public ActionResult Edit(String url, String title)
        {
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = new JsonResult
            {
                Data = new { response = "error", error = "An error occurred" }
            };

            var userId = UserUtils.GetCurrentUserId(Context);

            try
            {
                BookmarkUtil.DeleteUserBookmark(id, userId);
                result.Data = new {response = "success", data = new {BookmarkId = id}};
            }
            catch (Exception err)
            {
                Log.Error(err);
                result.Data = new {response = "error", error = "An error occurred"};
            }

            return result;
        }
    }
}
