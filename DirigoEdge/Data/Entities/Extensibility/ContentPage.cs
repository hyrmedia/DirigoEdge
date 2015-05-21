using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge.Data.Entities.Extensibility
{
    public class ContentPageExtension
    {
        [Key]
        [ForeignKey("ContentPage")]
        public virtual int ContentPageId { get; set; }

        [Index]
        public virtual ContentPage ContentPage { get; set; }
    }
}