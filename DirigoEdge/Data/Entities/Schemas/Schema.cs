using System;
using System.ComponentModel.DataAnnotations;

namespace DirigoEdge.Data.Entities.Schemas
{
    public class Schema
    {
        public virtual int SchemaId { get; set; }

        [MaxLength(256)]
        public virtual String Name { get; set; }
    }
}
