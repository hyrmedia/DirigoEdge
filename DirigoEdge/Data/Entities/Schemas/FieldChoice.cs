using System;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class FieldChoice
    {
        public virtual int FieldChoiceId { get; set; }
        public virtual FieldDefinition FieldDefinition { get; set; }
        public virtual String Value { get; set; }
    }
}
