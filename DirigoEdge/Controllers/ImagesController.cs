using System;
using System.Web.Mvc;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Controllers
{
    public class ImagesController : WebBaseController
    {
        public ActionResult RenderWithResize(string path, int width, int height, string directory)
        {
            try
            {
                return ImageUtils.GenerateThumbnail(path, directory, width, height);
            }
            catch (Exception ex)
            {
                Log.Warn("Failed to generate thumbnail for " + path, ex);
                return instantiate404ErrorResult(path);
            }
        }

        private HttpNotFoundResult instantiate404ErrorResult(string file)
        {
            return new HttpNotFoundResult(string.Format("The file {0} does not exist.", file));
        }
    }
}
