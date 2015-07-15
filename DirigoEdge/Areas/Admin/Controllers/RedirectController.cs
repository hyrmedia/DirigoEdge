﻿using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Controllers.Base;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class RedirectController : WebBaseAdminController
    {
        [PermissionsFilter(Permissions = "Can Edit Settings")]
        public ActionResult Redirects()
        {
            var model = new RedirectViewModel();

            return View(model);
        }

        [PermissionsFilter(Permissions = "Can Edit Settings")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddRedirect(String source, String destination, Boolean isPermanent, Boolean rootMatching)
        {
            var result = new JsonResult();
            try
            {
                var redirect = new Redirect
                {
                    Source = source,
                    Destination = destination,
                    IsPermanent = isPermanent,
                    RootMatching = rootMatching,
                    DateModified = DateTime.UtcNow, 
                    IsActive = true
                };
                Context.Redirects.Add(redirect);
                Context.SaveChanges();

                result.Data = new 
                {
                    id = redirect.RedirectId,
                    success = true
                };
                CachedObjects.GetRedirectsList(true);
                return result;
            }
            catch
            {
                result.Data = new { success = false };
                return result;
            }
        }

        [PermissionsFilter(Permissions = "Can Edit Settings")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult EditRedirect(int id, String source, String destination, Boolean isPermanent, Boolean rootMatching)
        {
            var result = new JsonResult();
            try
            {
                var redirect = Context.Redirects.FirstOrDefault(x => x.RedirectId == id);

                if (redirect != null)
                {
                    redirect.Source = source;
                    redirect.Destination = destination;
                    redirect.IsPermanent = isPermanent;
                    redirect.RootMatching = rootMatching;
                    redirect.DateModified = DateTime.UtcNow;
                    redirect.IsActive = true;
                
                    Context.SaveChanges();

                    result.Data = new
                    {
                        id = redirect.RedirectId,
                        success = true
                    };
                }

                CachedObjects.GetRedirectsList(true);
                return result;
            }
            catch
            {
                result.Data = new { success = false };
                return result;
            }
        }

        [HttpPost]
        [PermissionsFilter(Permissions = "Can Edit Settings")]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteRedirect(string id)
        {
            var result = new JsonResult();

            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    return result;
                }

                var redirectId = Int32.Parse(id);
                var redirect = Context.Redirects.FirstOrDefault(x => x.RedirectId == redirectId);

                Context.Redirects.Remove(redirect);
                Context.SaveChanges();

                // recycle cache after save
                CachedObjects.GetRedirectsList(true);

                result.Data = new {success = true};
                return result;
            }
            catch
            {
                result.Data = new { success = false };
                return result;
            }
        }
    }
}