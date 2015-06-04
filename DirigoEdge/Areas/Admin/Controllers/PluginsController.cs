using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class PluginsController : WebBaseAdminController
    {
        public ActionResult Index()
        {
            return View( new PluginViewModel
            {
                InstalledPlugins = CachedObjects.GetRegisteredPlugins()
            });
        }
    }
}