using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers;
using DirigoEdgeCore.Controllers;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class AdminController : DirigoBaseAdminController 
    {
        // Not authorized to view this page
        public ActionResult NotAuthorized()
        {
            return View();
        }

        // Dashboard      
        [UserIsLoggedIn]
        public ActionResult Index()
        {
	        var model = new DashBoardViewModel();

            return View(model);
        }
    }
}