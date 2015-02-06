using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
	public class ManageMediaViewModel :DirigoBaseModel
	{
		public List<Media> MediaListing;
	    public string Category;
		public const string supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpe,*.jpeg,*.svg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff,*.pdf,*.doc,*.docx";

		public ManageMediaViewModel(string directory)
		{
			MediaListing = new List<Media>();
		    BookmarkTitle = "Media";

		    Category = HttpContext.Current.Request.RequestContext.RouteData.Values["id"] != null
		                   ? HttpContext.Current.Request.RequestContext.RouteData.Values["id"].ToString()
		                   : "";

            BookmarkTitle = String.IsNullOrEmpty(Category) ? "Media" : "Media / " + Category;

			var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly).Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
            var webPath = Category == null
                              ? ContentGlobals.IMAGEUPLOADDIRECTORY
                              : ContentGlobals.IMAGEUPLOADDIRECTORY + Category + "/";

			foreach (var file in files)
			{
                var fileName = Path.GetFileName(file);
                var cdate = File.GetCreationTime(file);

				// Skip over placeholder element
				if (fileName == "ph") continue;

				var imgSrc = webPath + fileName;
                MediaListing.Add(new Media { Path = imgSrc, CreateDate = cdate, FileExtension = Path.GetExtension(imgSrc) });
			}
		}
	}

    public class Media
    {
        public String Path { get; set; }
        public String ThumbnailPath {
            get
            {
                if (IsImage)
                {
                    // try thumbnail
                    if (!File.Exists(ImageUtils.GetImageThumbPath(Path)))
                    {
                        ImageUtils.GenerateThumbnail(Path);
                    }

                    return ImageUtils.GetImageThumbPath(Path, true);
                }
                
                return Path;
            }
        }
        public DateTime CreateDate { get; set; }
        public bool IsImage { get { return ImageUtils.IsFileAnImage(Path); } }
        public string FileExtension;
        public string FileExtensionClass { get { return FileExtension.ToLower().Replace(".", ""); } }
    }
}