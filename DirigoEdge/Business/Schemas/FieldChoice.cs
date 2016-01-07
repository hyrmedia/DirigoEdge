using System;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class FieldChoice
    {
        public virtual int FieldChoiceId { get; set; }
        public virtual FieldDefinition FieldDefinition { get; set; }
        public String Value { get; set; }
    }
}
