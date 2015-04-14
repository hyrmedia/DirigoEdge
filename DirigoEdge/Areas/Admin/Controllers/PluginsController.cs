using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class PluginsController : DirigoBaseAdminController
    {
        public ActionResult Plugins()
        {
            return View( new PluginViewModel
            {
                InstalledPlugins = CachedObjects.GetRegisteredPlugins()
            });
        }
    }
}