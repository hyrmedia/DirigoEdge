using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageModulesViewModel : DirigoBaseModel
    {
        public List<ContentModule> Modules;

        public ManageModulesViewModel()
        {
            BookmarkTitle = "Content Modules";
            Modules = Context.ContentModules.Where(mod => mod.ParentContentModuleId == null).ToList();
        }
    }
}