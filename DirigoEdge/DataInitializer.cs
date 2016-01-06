using System;
using System.Linq;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge
{
    public static class DataInitiializer
    {
        public static void EnsureRequiredDataIsPresent()
        {
            using (var context = new WebDataContext())
            {
                EnsureAdminExists(context);
            }
        }

        private static void EnsureAdminExists(DataContext context)
        {
            if (context.Roles.Any(r => r.RoleName == "Administrators"))
            {
                return;
            }

            context.Roles.Add(
                new Role
                {
                    RoleId = Guid.NewGuid(),
                    RoleName ="Administrators"
                });

            context.SaveChanges();
        }
    }
}