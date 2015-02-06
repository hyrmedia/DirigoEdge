using System.Web.Mvc;

namespace DirigoEdge.Areas.Admin
{
	public class AdminAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Admin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
            //// Vacation Packages Sample Route
            //context.MapRoute(
            //    name: "VacationPackages",
            //    url: "admin/packages/",
            //    defaults: new { controller = "Admin", action = "ManageEntity", schemaId = 1, heading = "Manage Vacation Packages", buttonText = "New Vacation Package +", editHeading = "Edit Vacation Package", sort = "sort-listings" }
            //);

            context.MapRoute(
                "Admin",
                "Admin/",
                new { Controller = "Admin", action = "Index", id = UrlParameter.Optional },
                new[] { "DirigoEdge.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin Not Authorized",
                "Admin/NotAuthorized",
                new { Controller = "Admin", action = "NotAuthorized", id = UrlParameter.Optional },
                new[] { "DirigoEdge.Areas.Admin.Controllers" }
            );

			context.MapRoute(
				"Admin_default",
				"Admin/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional },
                new[] { "DirigoEdge.Areas.Admin.Controllers" }
			);
		}
	}
}
