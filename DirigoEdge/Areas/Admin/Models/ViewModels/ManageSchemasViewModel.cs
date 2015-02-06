using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageSchemasViewModel : DirigoBaseModel
    {
        public List<Schema> Schemas;

        public ManageSchemasViewModel()
        {
            BookmarkTitle = "Manage Schemas";
            Schemas = Context.Schemas.ToList();
        }
    }
}