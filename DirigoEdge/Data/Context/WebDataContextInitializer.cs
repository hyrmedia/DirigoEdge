using System.Data.Entity;
using DirigoEdgeCore.Data.Context;

namespace DirigoEdge.Data.Context
{
    public class WebDataContextInitializer : CreateDatabaseIfNotExists<DataContext>
    {
        protected override void Seed(DataContext context)
        {
           DataContextInitializer.Init(context);
        }
    }
}