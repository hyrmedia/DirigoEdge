using System;
using System.Collections.Generic;

namespace DirigoEdge.Business.Schemas
{
    public class FieldDefinition
    {
        public int FieldDefinitionId { get; set; }
        public FieldType FieldType { get; set; }
        public String Name { get; set; }
        public List<Metadata> Metadata { get; set; }

        // Properties from this point down are for specific field types.
        // i.e. Select needs choices, a List needs other fields below it.
        // At some point (prior to this project going live with a client) we should consider
        // whether these should be derived classes rather than optional properties. For the time
        // being this seems easier for (de)serialization, but it should be reevaluated when 
        // more functionality is in place.

        public List<FieldChoice> Choices { get; set; } 
        public List<FieldDefinition> ListFields { get; set; }
    }
}
