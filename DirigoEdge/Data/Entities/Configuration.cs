using System;
using System.ComponentModel.DataAnnotations;

namespace DirigoEdge.Data.Entities
{
    public class SiteConfiguration
    {
        [Key]
        public virtual int ConfigId { get; set; }

        public virtual String Key { get; set; }
        public virtual String Value { get; set; }
    }
}