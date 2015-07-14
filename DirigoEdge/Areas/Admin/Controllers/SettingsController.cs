using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Areas.Admin.Models.DataModels;
using DirigoEdge.Areas.Admin.Models.ViewModels;
using DirigoEdge.Controllers.Base;
using DirigoEdge.Models;
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
        public JsonResult SaveSiteSettings(Settings entity)
        {
            var siteSettings = Context.SiteSettings.FirstOrDefault();

            if (siteSettings == null)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "There was an finding the site settings."
                    }
                };
            }


            Mapper.Map(entity, siteSettings);

            var timeZone = Context.Configurations.First(config => config.Key == ConfigSettings.TimeZone.ToString());
            timeZone.Value = entity.TimeZoneId;

            var success = Context.SaveChanges();

            if (success > 0)
            {
                // refresh the cache
                SettingsUtils.GetSiteSettings(true);
                return new JsonResult
                  {
                      Data = new
                      {
                          success = true,
                          message = "Settings saved successfully."
                      }
                  };
            }

            return new JsonResult
            {
                Data = new
                {
                    success = false,
                    message = "There was an error processing your request."
                }
            };
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
