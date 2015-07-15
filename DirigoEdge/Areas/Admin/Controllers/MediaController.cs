using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Extensions;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class MediaController : WebBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Manage Media")]
        public ActionResult ManageMedia(string id)
        {
            var directory = id != null
                                   ? Server.MapPath("~" + ContentGlobals.IMAGEUPLOADDIRECTORY + "\\" + id)
                                   : Server.MapPath("~" + ContentGlobals.IMAGEUPLOADDIRECTORY);

            var model = new ManageMediaViewModel(directory);

            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Manage Media")]
        public JsonResult fileUpload(HttpPostedFileBase file)
        {
            var result = new JsonResult();

            if (file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~" + ContentGlobals.IMAGEUPLOADDIRECTORY), fileName);

                var overwrite = System.IO.File.Exists(physicalPath);

                file.SaveAs(physicalPath);
                System.IO.File.SetCreationTime(physicalPath, DateTime.UtcNow);



                string imgPath = ContentGlobals.IMAGEUPLOADDIRECTORY + file.FileName;
                var media = new Media { Path = imgPath, CreateDate = DateTime.UtcNow };
                string htmlstr = overwrite ? "" : ControllerContext.RenderPartialToString("~/Areas/Admin/Views/Shared/Partials/MediaRowPartial.cshtml", media);

                result.Data = new { html = htmlstr, path = imgPath };
            }

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Manage Media")]
		public JsonResult UploadFile(HttpPostedFileBase file, String category = null)
	    {
            var result = new JsonResult();
	        string warning = String.Empty;

            if (file != null)
            {
                string uploadToPath = String.IsNullOrEmpty(category)
                                       ? ContentGlobals.IMAGEUPLOADDIRECTORY
                                       : ContentGlobals.IMAGEUPLOADDIRECTORY + category + "/";

                string physicalPath = Path.Combine(Server.MapPath("~" + uploadToPath), Path.GetFileName(file.FileName));

                string fullFilename = file.FileName;

                if (System.IO.File.Exists(physicalPath))
                {
                    string filename = Path.GetFileNameWithoutExtension(physicalPath);
                    string extension = Path.GetExtension(physicalPath);
                    int dateHash = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    fullFilename = filename + "_" + dateHash + extension;

                    physicalPath = Path.Combine(Server.MapPath("~" + uploadToPath), fullFilename);

                    warning = "File was renamed.";
                }

                try
                {
                    // Save the file to disk
                    file.SaveAs(physicalPath);

                    // If it's an image, render a thumbnail
                    if (ImageUtils.IsFileAnImage(physicalPath))
                    {
                        ImageUtils.GenerateThumbnail(uploadToPath + file.FileName);
                    }

                    // Set metadata
                    System.IO.File.SetCreationTime(physicalPath, DateTime.UtcNow);

                    // Return html to be populated client side
                    var media = new Media { Path = uploadToPath + fullFilename, CreateDate = DateTime.UtcNow };
                    string renderedHtml = ControllerContext.RenderPartialToString("~/Areas/Admin/Views/Shared/Partials/MediaRowPartial.cshtml", media);

                    result.Data = new {success = true, html = renderedHtml, path = media.Path, category, warning};
                }
                catch (SystemException err)
                {
                    result.Data = new { success = false, error = err};
                }
            }
            else
            {
                result.Data = new {success = false, error = "Could not find specified file"};
            }

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Manage Media")]
        public JsonResult RemoveFile(String filename)
        {
            string response;
            bool success;

            // Do a little scrubbing to prevent malicious deletion
            filename = filename.Replace(ContentGlobals.IMAGEUPLOADDIRECTORY, "").Replace("..", "");

            var filepath = Server.MapPath(ContentGlobals.IMAGEUPLOADDIRECTORY + "/" + filename);
            var resizedDirPath = Server.MapPath(ContentGlobals.RESIZEDIMAGEPARENTDIRECTORY);
            
            if (System.IO.File.Exists(filepath))
            {
                try
                {
                    if (Directory.Exists(resizedDirPath))
                    {
                        // loop over directories in /images/ to remove any resized versions
                        List<string> dirs = new List<string>(Directory.EnumerateDirectories(resizedDirPath));
                        foreach (var dir in dirs)
                        {
                            // prefix could be /images/small/  /images/medium/  /images/large/  or custom
                            string resizedFilePath = dir + "/" + ContentGlobals.IMAGEUPLOADDIRECTORY + "/" + filename;
                            if (System.IO.File.Exists(resizedFilePath))
                            {
                                System.IO.File.Delete(resizedFilePath);
                            }
                        }
                    }
                    System.IO.File.Delete(filepath);
                    response = "File successfully deleted.";
                    success = true;
                }
                catch (IOException err)
                {
                    response = "Could not delete. " + err;
                    success = false;
                }
            }
            else
            {
                response = "Could not delete. File does not exist.";
                success = false;
            }

            var result = new JsonResult { Data = new { response, success } };

            return result;
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Manage Media")]
        public JsonResult AddFolder(String folder)
        {
            var result = new JsonResult();

            var parentPath = Server.MapPath(ContentGlobals.IMAGEUPLOADDIRECTORY);

            if (Directory.Exists(parentPath + "/" + folder))
            {
                result.Data = new { success = false, error = "Folder already exists" };
                return result;
            }

            try
            {
                Directory.CreateDirectory(parentPath + "/" + folder);
                result.Data = new { success = true, directory = parentPath + "/" + folder };
                return result;
            }
            catch (SystemException err)
            {
                result.Data = new { success = false, error = err };
                return result;
            }
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Manage Media")]
        public JsonResult DeleteFolder(String folder)
        {
            var result = new JsonResult();

            string directory = Server.MapPath(ContentGlobals.IMAGEUPLOADDIRECTORY + "/" + folder);

            if (Directory.Exists(directory))
            {
                try
                {
                    Directory.Delete(directory, true);
                    
                    BookmarkUtil.DeleteBookmarkForUrl("/admin/media/managemedia/"+ folder + "/");

                    result.Data = new { success = true, directory };
                    return result;
                }
                catch (SystemException err)
                {
                    result.Data = new { success = false, error = err };
                    return result;
                }
            }

            result.Data = new { success = false, error = "Folder doesn't exist" };
            return result;
        }

        [UserIsLoggedIn]
        public JsonResult FileBrowser(string id = null)
        {
            var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            string directory = id != null ? Server.MapPath("~" + ContentGlobals.IMAGEUPLOADDIRECTORY + "\\" + id) : null;

            var model = new FileBrowserViewModel(directory);

            string partialHtml = ControllerContext.RenderPartialToString("~/Areas/Admin/Views/Shared/Partials/FileBrowserPartial.cshtml", model);

            result.Data = new {success = true, html = partialHtml};

            return result;
        }
    }
}
