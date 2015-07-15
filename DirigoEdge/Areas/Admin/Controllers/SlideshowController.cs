using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Attributes;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class SlideshowController : WebBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Pages")]
        public ActionResult EditSlideShow(int id)
        {
            var model = new EditSlideShowViewModel(id);
            return View(model);
        }

		[HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Modules")]
		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult SaveSlideshow(SlideshowModule entity)
		{
			var result = new JsonResult();

			if (!String.IsNullOrEmpty(entity.SlideShowName))
			{
					SlideshowModule editedContent = Context.SlideshowModules.FirstOrDefault(x => x.SlideshowModuleId == entity.SlideshowModuleId);
					if (editedContent != null)
					{
						editedContent.SlideShowName = entity.SlideShowName;

						editedContent.AdvanceSpeed = entity.AdvanceSpeed;
						editedContent.Animation = entity.Animation;
						editedContent.AnimationSpeed = entity.AnimationSpeed;
						editedContent.PauseOnHover = entity.PauseOnHover;
						editedContent.ShowBullets = entity.ShowBullets;

						editedContent.UseDirectionalNav = entity.UseDirectionalNav;
						editedContent.UseTimer = entity.UseTimer;

						// In order to save slides we must first delete prior data, then come back and add them
						editedContent.Slides.ToList().ForEach(r => Context.Set<Slide>().Remove(r));
						Context.SaveChanges();

						// Now add in the new Slides
						foreach (var slide in entity.Slides)
						{
							editedContent.Slides.Add(slide);
						}
						Context.SaveChanges();
					}
			}

			return result;
		}
    }
}
