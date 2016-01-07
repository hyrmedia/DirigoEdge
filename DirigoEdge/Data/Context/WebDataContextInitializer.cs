using System.Data.Entity;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;
using System;

namespace DirigoEdge.Data.Context
{
    public partial class WebDataContextInitializer : CreateDatabaseIfNotExists<WebDataContext>
    {
        protected override void Seed(WebDataContext context)
        {
            DataContextInitializer.Init(context);

            context.Roles.Add(new Role { RoleId = Guid.NewGuid(), RoleName = "Administrators" });

            CustomSeed(context);
        }

        partial void CustomSeed(WebDataContext context);
    }
}