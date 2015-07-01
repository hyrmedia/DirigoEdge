using System.Collections.Generic;

namespace DirigoEdge.Areas.Admin.Models
{
    public class ImportResults
    {
        public List<ImportResult> ModuleResults { get; set; }
        public List<ImportResult> SchemaResults { get; set; }

        public ImportResults()
        {
            ModuleResults = new List<ImportResult>();
            SchemaResults = new List<ImportResult>();
        }
    }
}