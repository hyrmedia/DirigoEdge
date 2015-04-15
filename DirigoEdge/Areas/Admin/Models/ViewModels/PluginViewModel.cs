using System.Collections.Generic;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class PluginViewModel : DirigoBaseModel
    {
        public IEnumerable<KeyValuePair<string, string>> InstalledPlugins;
    }
}