using System;
using System.ComponentModel.DataAnnotations;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class FieldDefinition
    {
        public virtual int FieldDefinitionId { get; set; }
        public virtual Schema Schema { get; set; }
        public virtual FieldType FieldType { get; set; }
        
        [MaxLength(256)]
        public virtual String Name { get; set; }
    }
}
