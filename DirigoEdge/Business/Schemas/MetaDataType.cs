using System;
using System.ComponentModel.DataAnnotations;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class MetadataType
    {
        public virtual int MetadataTypeId { get; set; }

        [MaxLength(256)]
        public virtual String Name { get; set; }
    }
}
