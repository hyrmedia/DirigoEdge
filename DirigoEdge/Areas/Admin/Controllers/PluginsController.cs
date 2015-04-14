using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdgeCore.Controllers;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class PluginsController : DirigoBaseAdminController
    {
        public ActionResult Plugins()
        {
            return View( new PluginViewModel
            {
                InstalledPlugins = DirigoEdgeCore.Utils.CachedObjects.GetRegisteredPlugins()
            });
        }
    }
}