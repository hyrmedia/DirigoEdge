using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DirigoEdge.Data.Context;
using DirigoEdge.Models.Shortcodes;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.PluginFramework;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Logging;
using log4net.Config;

namespace DirigoEdge
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            XmlConfigurator.Configure();
            var log = LogFactory.GetLog(typeof(MvcApplication));
            log.Debug("Starting edge");

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            var rc = RouteTable.Routes;

            using (var context = new WebDataContext())
            {
                context.Database.Initialize(false);
            }

            // Registers Containing Area, such as /Admin
            AreaRegistration.RegisterAllAreas();

            // Register the stock routes plus the plugin routes
            RouteConfig.RegisterRoutes(rc);

            // Add the new View Engine for our plugins to use
            ViewEngines.Engines.Add(new PluginRazorViewEngine());

            // Register new dynamic modules/shortcodes
            DynamicModules.Instance.AddDynamicModule("responsive_image", new ResponsiveImageShortcode());
            DynamicModules.Instance.AddDynamicModule("featured_events", new EventsModule());

            Mapping.SetAutomapperMappings();
        }

        // May need to store host in distributed or multi-tenant applications
        protected void Application_BeginRequest()
        {
            // Find out if the redirect exists for this request path

            var allRedirects = CachedObjects.GetRedirectsList(false);

            if (allRedirects == null)
            {
                return;

            }

            Redirect redirect = allRedirects
                .FirstOrDefault(
                    x => (x.Source == Request.Path || x.Source + '/' == Request.Path || x.Source == Request.Path + '/')
                         || (x.RootMatching && Request.Path.StartsWith(x.Source)));

            // Check whether the browser remains connected to the server.
            if (!Response.IsClientConnected)
            {
                // If the browser is not connected stop all response processing.
                Response.End();
                return;
            }
            
            if (redirect == null)
            {
                return;
            }

            if (redirect.IsPermanent)
            {
                Response.RedirectPermanent(redirect.Destination);
            }
            else
            {
                // If still connected, redirect to another page.
                Response.Redirect(redirect.Destination);
            }
        }
    }
}