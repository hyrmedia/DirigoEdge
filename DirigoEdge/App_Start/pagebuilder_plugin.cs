using System.Web.Mvc;
using DirigoEdgeCore.PluginFramework;
using DirigoEdgeCore.Utils.Logging;

[assembly: WebActivatorEx.PreApplicationStartMethod (
    typeof(DirigoEdgePagebuilder.App_Start.DirigoEdgePagebuilder), "PreStart")]

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof(DirigoEdgePagebuilder.App_Start.DirigoEdgePagebuilder), "PostStart")]

namespace DirigoEdgePagebuilder.App_Start {
    public static class DirigoEdgePagebuilder {
        public static void PreStart() {
        }

        public static void PostStart()
        {
            var log = LogFactory.GetLog(typeof(DirigoEdgePagebuilder));
            log.Debug("Pagebuilder Loaded");
            ViewEngines.Engines.Add(new NuGetPluginViewEngine("PageBuilder_Plugin"));
            var plugin = new PageBuilder_Plugin.Plugin.PageBuilder_Plugin();
            plugin.RegisterPlugin();
        }
    }
}