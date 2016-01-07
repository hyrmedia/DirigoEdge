using System;
using System.ComponentModel.DataAnnotations;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class FieldType
    {
        public virtual int FieldTypeId { get; set; }

        [MaxLength(256)]
        public virtual String Name { get; set; }
    }
}
