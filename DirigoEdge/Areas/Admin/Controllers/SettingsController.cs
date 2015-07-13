using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class SettingsController : WebBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Settings")]
        public ActionResult SiteSettings()
        {
            var model = SiteSettingsViewModel.LoadSiteSettings(Context);
            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Settings")]
        public ActionResult BlogSettings()
        {
            var model = new BlogSettingsViewModel();

            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Settings")]
        public ActionResult FeatureSettings()
        {
            var model = new FeatureSettingsViewModel();

            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Settings")]
		public JsonResult SaveBlogSettings(BlogSettings entity)
		{
            var result = new JsonResult
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

				var blogSettings = Context.BlogSettings.FirstOrDefault();
				if (blogSettings != null)
				{
					blogSettings.BlogTitle = entity.BlogTitle;
					blogSettings.DisableAllCommentsGlobal = entity.DisableAllCommentsGlobal;
					blogSettings.DisqusShortName = entity.DisqusShortName;
					blogSettings.FacebookAppId = entity.FacebookAppId;
					blogSettings.MaxBlogsOnHomepageBeforeLoad = entity.MaxBlogsOnHomepageBeforeLoad;
				    blogSettings.MaxArchives = entity.MaxArchives;
					blogSettings.ShowDisqusComents = entity.ShowDisqusComents;
					blogSettings.ShowFacebookComments = entity.ShowFacebookComments;
					blogSettings.ShowFacebookLikeButton = entity.ShowFacebookLikeButton;

					success = Context.SaveChanges();
				}

            if (success > 0)
            {
                result.Data = new
                {
                    success = true,
                    message = "Settings saved successfully."
                };
            }

			return result;
		}

        [PermissionsFilter(Permissions = "Can Edit Settings")]
        public JsonResult SaveSiteSettings(SiteSettings entity)
		{
            var result = new JsonResult()
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };

            var success = 0;

				var siteSettings = Context.SiteSettings.FirstOrDefault();
				if (siteSettings != null)
				{
					siteSettings.ContactEmail = entity.ContactEmail;
					siteSettings.SearchIndex = entity.SearchIndex;
					siteSettings.GoogleAnalyticsId = entity.GoogleAnalyticsId;
                    siteSettings.GoogleAnalyticsType = entity.GoogleAnalyticsType;
					siteSettings.ContentPageRevisionsRetensionCount = entity.ContentPageRevisionsRetensionCount;
                    siteSettings.DefaultUserRole = entity.DefaultUserRole;

					success = Context.SaveChanges();
				}

            if (success > 0)
            {
            	// refresh the cache
            	SettingsUtils.GetSiteSettings(true);
                result.Data = new
                {
                    success = true,
                    message = "Settings saved successfully."
                };
            }

			return result;
		}

        [PermissionsFilter(Permissions = "Can Edit Settings")]
        public JsonResult SaveFeatureSettings(FeatureSettings entity)
		{
			var result = new JsonResult()
			{
			    Data = new
			    {
			        success = false,
                    message = "There was an error processing your request."
			    }
			};

		    var success = 0;

				var featureSettings = Context.FeatureSettings.FirstOrDefault();
				if (featureSettings != null)
				{
					featureSettings.EventsEnabled = entity.EventsEnabled;

					success = Context.SaveChanges();

					// Bust the site settings cache for events since we modified it's value
                    SettingsUtils.EventsEnabled(true);
				}

		    if (success > 0)
		    {
		        result.Data = new
		        {
		            success = true,
		            message = "Settings saved successfully."
		        };
		    }

			return result;
		}
    }
}
