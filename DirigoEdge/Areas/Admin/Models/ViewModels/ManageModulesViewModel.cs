using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageModulesViewModel : DirigoBaseModel
    {
        // Label Customization
        public string Heading = "Manage Modules";
        public string NewButtonText = "New Module +";
        public string EditContentHeading = "Edit Module";  // Passed to controller for display purposes
        public int SchemaId = -1;
        public string Sort = "";

        public List<ContentModule> Modules;

        public ManageModulesViewModel(int schemaId = 0)
        {
            BookmarkTitle = "Content Modules";
            if (schemaId > 0)
            {
                SchemaId = schemaId;
                Modules = Context.ContentModules.Where(x => x.SchemaId == schemaId && x.ParentContentModuleId == null).ToList();
            }
            else
            {
                Modules = Context.ContentModules.Where(mod => mod.ParentContentModuleId == null).ToList();
            }
        }
    }
}