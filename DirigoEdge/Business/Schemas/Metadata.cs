using System;

namespace DirigoEdge.Business.Schemas
{
    public class Metadata
    {
        public int MetadataId { get; set; }
        public MetadataType Type { get; set; }
        public String Value { get; set; }
    }
}
