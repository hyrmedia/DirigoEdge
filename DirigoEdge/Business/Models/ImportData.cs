using System.Collections.Generic;
using DirigoEdgeCore.Business.Models;

namespace DirigoEdge.Business.Models
{
    public class ImportData
    {
        public List<Schema> Schemas { get; set; }
        public List<Module> Modules { get; set; }
    }
}