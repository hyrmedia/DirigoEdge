using System.Data.Entity;
using DirigoEdge.Data.Entities;
using DirigoEdge.Data.Entities.Extensibility;
using DirigoEdgeCore.Data.Context;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DirigoEdge.Data.Context
{
    public partial class WebDataContext : DataContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public WebDataContext()
            : base("DataContext")
        {
            Database.SetInitializer(new WebDataContextInitializer());
        }
        
        public DbSet<ContentPageExtension> ContentPageExtensions { get; set; }
    }
}