using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class EditSchemaViewModel : DirigoBaseModel
    {
        public Schema TheSchema;

        public EditSchemaViewModel(int id)
        {
            TheSchema = Context.Schemas.FirstOrDefault(x => x.SchemaId == id);
            TheSchema.JSONData = TheSchema.JSONData ?? "{ }";
            BookmarkTitle = TheSchema.DisplayName;
        }
    }
}