using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge
{
	public class RouteConfig
	{
        readonly ILog _log = LogFactory.GetLog(typeof(RouteConfig));

		public static void RegisterRoutes(RouteCollection routes)
		{
		    routes.MapMvcAttributeRoutes();
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.LowercaseUrls = true;
		    routes.AppendTrailingSlash = true;

            // Homepage
            routes.MapRoute(
                name: "HomePage",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "HomeController",
                url: "home/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "The Blog",
                url: "blog/{category}/{title}",
                defaults: new { controller = "Blog", action = "Index", category = UrlParameter.Optional, title = UrlParameter.Optional },
                namespaces: new [] { "DirigoEdge.Controllers" }
            );

			// Blog Categories View
			// Ex: dirigodev.com/blog/categories/seo/
			routes.MapRoute(
				"Blog Categories View", // Route name
				"blog/categories/{category}", // URL with parameters
				new { controller = "Blog", action = "Categories", category = UrlParameter.Optional }, // Parameter defaults
                namespaces: new[] { "DirigoEdge.Controllers" }
			);

            // Blog Tags View
            // Ex: dirigodev.com/blog/tags/seo/
            routes.MapRoute(
                "Blog Tags View", // Route name
                "blog/tags/{tag}", // URL with parameters
                new { controller = "Blog", action = "Tags", tag = UrlParameter.Optional }, // Parameter defaults
                namespaces: new[] { "DirigoEdge.Controllers" }
            );

			// Blog By Users View
			// Ex: dirigodev.com/blog/user/jberry/
			routes.MapRoute(
				"Blog By User View", // Route name
				"blog/user/{username}", // URL with parameters
				new { controller = "Blog", action = "User", username = UrlParameter.Optional }, // Parameter defaults
                namespaces: new[] { "DirigoEdge.Controllers" }
			);

			// Blog RSS Feed
			routes.MapRoute(
				"Blog RSS New Feeds", // Route name
				"Blog/NewPosts/", // URL with parameters
				new { controller = "Blog", action = "NewPosts", id = UrlParameter.Optional }, // Parameter defaults
                namespaces: new[] { "DirigoEdge.Controllers" }
			);

            routes.MapRoute(
                name: "BlogActions",
                url: "BlogActions/{action}/{id}",
                defaults: new { controller = "BlogActions", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Event",
                url: "event/{title}",
                defaults: new { controller = "Event", action = "Index", title = UrlParameter.Optional }
            );

            // Event Categories View
            // Ex: dirigodev.com/event/categories/seo/
            routes.MapRoute(
                "Event Categories View", // Route name
                "event/categories/{category}", // URL with parameters
                new { controller = "Event", action = "Categories", category = UrlParameter.Optional } // Parameter defaults
            );

			routes.MapRoute(
				name: "Content",
				url: "content/{title}",
				defaults: new { controller = "Content", action = "Index", title = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Account",
				url: "account/{action}/{id}",
				defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
			);

            routes.MapRoute(
                name: "Tweets",
                url: "tweet/",
                defaults: new { controller = "Home", action = "Tweet"}
            );

			// Generated Site Map
			routes.MapRoute(
				name: "Sitemap",
				url: "sitemap.xml",
				defaults: new { controller = "Home", action = "SitemapXML" }
			);

            #region Base Admin Routes
            /*routes.MapRoute(
                name: "admin",
                url: "admin/{action}/{id}",
                defaults: new { controller = "admin", action = "index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "blogadmin",
                url: "blogadmin/{action}/{id}",
                defaults: new { controller = "blogadmin", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "category",
                url: "category/{action}/{id}",
                defaults: new { controller = "category", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "contentadmin",
                url: "contentadmin/{action}/{id}",
                defaults: new { controller = "contentadmin", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "eventadmin",
                url: "eventadmin/{action}/{id}",
                defaults: new { controller = "eventadmin", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "eventcategory",
                url: "eventcategory/{action}/{id}",
                defaults: new { controller = "eventcategory", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "mediaupload",
                url: "mediaupload/{action}/{id}",
                defaults: new { controller = "mediaupload", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "plugins",
                url: "plugins/{action}/{id}",
                defaults: new { controller = "plugins", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "settings",
                url: "settings/{action}/{id}",
                defaults: new { controller = "settings", action = "index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "slideadmin",
                url: "slideadmin/{action}/{id}",
                defaults: new { controller = "slideadmin", action = "index", id = UrlParameter.Optional }
            );*/

            #endregion

            #region DynamicImageResizing

            routes.MapRoute(
                name: "DynamicImagesSmall",
                url: "images/small/{*path}",
                defaults: new { controller = "Images", action = "RenderWithResize", width = 480, height = 320, directory = "small", }
            );

            routes.MapRoute(
                name: "DynamicImagesMed",
                url: "images/medium/{*path}",
                defaults: new { controller = "Images", action = "RenderWithResize", width = 1024, height = 800, directory = "medium" }
            );           

            #endregion

            // Master Content Controller
            // Decides whether to serve up a content page or 404. This allows short url's for content pages
            routes.MapRoute(
                name: "Master",
                url: "{*url}",
                defaults: new { controller = "Content", action = "Index", title = UrlParameter.Optional }
            );
		}
	}
}