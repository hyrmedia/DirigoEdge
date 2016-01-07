using System;
using System.Collections.Generic;

namespace DirigoEdge.Business.Schemas
{
    public class Schema
    {
        public int SchemaId { get; set; }
        public String Name { get; set; }

        public List<FieldDefinition>  Fields { get; set; }
    }
}
