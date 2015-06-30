using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirigoEdge.Business.Models
{
    public class Module
    {
        public String ModuleName { get; set; }
        public String HTMLContent { get; set; }
        public String HTMLUnparsed { get; set; }
        public String CSSContent { get; set; }
        public String JSContent { get; set; }
        public String SchemaName { get; set; }
        public int? ParentContentModuleId { get; set; }
        public String DraftAuthorName { get; set; }
        public string SchemaEntryValues { get; set; }
    }
}