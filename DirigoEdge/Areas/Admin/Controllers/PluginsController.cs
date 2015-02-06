using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using DirigoEdge.Controllers;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class PluginsController : DirigoBaseAdminController
    {
        private const string pluginDir = "/Plugins/";

        public ActionResult InstallPlugin(Plugin plugin)
        {
            string pluginsPath = AppDomain.CurrentDomain.BaseDirectory + "Plugins\\";

            // Install or update plugin information in database
            var pluginToUpdate = Context.Plugins.FirstOrDefault(x => x.AppStoreId == plugin.AppStoreId);

            // Update version information if it's already installed
            if (pluginToUpdate != null)
            {
                pluginToUpdate.Version = plugin.Version;
                pluginToUpdate.Description = plugin.Description;
                pluginToUpdate.PluginDisplayName = pluginToUpdate.PluginDisplayName;
            }
            // Otherwise insert record into database if new install
            else
            {
                var pluginToInsert = new Plugin();
                pluginToInsert.AppStoreId = plugin.AppStoreId;
                pluginToInsert.FileLocation = plugin.FileLocation;
                pluginToInsert.Version = plugin.Version;
                pluginToInsert.Description = plugin.Description;
                pluginToInsert.PluginDisplayName = plugin.PluginDisplayName;

                Context.Plugins.Add(pluginToInsert);
            }

            Context.SaveChanges();

            // Download the zip file, then extract it to the /Plugins directory
            using (var client = new WebClient())
            {
                string filename = Path.GetFileName(plugin.FileLocation);
                string zipFileTemp = pluginDir + filename;
                string downloadZipTo = Server.MapPath(zipFileTemp);
                var folderLocation = new DirectoryInfo(Server.MapPath(zipFileTemp).Replace(".zip", ""));

                // remove directory of plugin if exists, this means it had some views or js or css with it
                if (folderLocation.Exists)
                {
                    // delete recursive
                    folderLocation.Delete(true);
                }

                client.DownloadFile(plugin.FileLocation, downloadZipTo);

                // If the dll already exists, then we need to delete it before installing
                // This is tricky since the file is in use
                // We'll need to save the file to delete to the database, and the .zip file to install should remain in the /Plugins directory
                // * the restart the app pool to delete the file, then install the plugin..                 
                string dllLocation = downloadZipTo.Replace(".zip", ".dll");

                // get the name of the current plugin folder
                var pluginFolder = downloadZipTo.Replace(".zip", "");

                // determine if it exists on the users filesystem
                //var pluginFolderExists = Directory.Exists(pluginsPath + pluginFolder);

                // If the file exists, mark it for deletion upon next app pool restart
                if (System.IO.File.Exists(dllLocation))
                {
                    var siteSettings = Context.SiteSettings.FirstOrDefault();
                    siteSettings.RMPluginDLLPath = dllLocation;

                    Context.SaveChanges();
                }
                else
                {
                    // Extract Zip File
                    ZipFile.ExtractToDirectory(downloadZipTo, Server.MapPath(pluginDir));

                    // Delete the Zip file
                    System.IO.File.Delete(downloadZipTo);
                }

                // Reset app pool to install plugin or start removal process of existing plugin
                HttpRuntime.UnloadAppDomain();
            }

            var result = new JsonResult();
            return result;
        }

        public ActionResult RemovePlugin(Plugin plugin)
        {
            // Remove entry from database
            var pluginToRemove = Context.Plugins.Where(x => x.AppStoreId == plugin.AppStoreId).FirstOrDefault();

            if (pluginToRemove != null)
            {
                Context.Plugins.Remove(pluginToRemove);
                Context.SaveChanges();
            }

            // Mark Plugin For Deletion
            string filename = Path.GetFileName(plugin.FileLocation);
            string zipFileTemp = pluginDir + filename;
            string dllLocation = Server.MapPath(zipFileTemp).Replace(".zip", ".dll");
            var folderLocation = new DirectoryInfo(Server.MapPath(zipFileTemp).Replace(".zip", ""));

            var siteSettings = Context.SiteSettings.FirstOrDefault();
            siteSettings.RMPluginDLLPath = dllLocation;

            Context.SaveChanges();

            // remove directory of plugin if exists, this means it had some views or js or css with it
            if (folderLocation.Exists)
            {
                // delete recursive
                folderLocation.Delete(true);
            }

            // Reset app pool to istart removal process of existing plugin
            HttpRuntime.UnloadAppDomain();

            return new JsonResult(); ;
        }
    }
}