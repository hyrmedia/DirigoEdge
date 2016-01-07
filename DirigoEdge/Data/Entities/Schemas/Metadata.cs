using System;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class Metadata
    {
        public virtual int MetadataId { get; set; }
        public virtual FieldDefinition Field { get; set; }
        public virtual MetadataType Type { get; set; }
        public virtual String Value { get; set; }
    }
}
