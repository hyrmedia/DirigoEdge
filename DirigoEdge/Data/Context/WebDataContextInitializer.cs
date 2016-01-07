using System.Data.Entity;
using DirigoEdgeCore.Data.Context;

namespace DirigoEdge.Data.Context
{
    public partial class WebDataContextInitializer : CreateDatabaseIfNotExists<WebDataContext>
    {
        protected override void Seed(WebDataContext context)
        {
            DataContextInitializer.Init(context);

            CustomSeed(context);
        }

        partial void CustomSeed(WebDataContext context);
    }
}