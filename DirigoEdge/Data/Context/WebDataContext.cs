﻿using System.Data.Entity;
using DirigoEdgeCore.Data.Context;

namespace DirigoEdge.Data.Context
{
    public class WebDataContext : DataContext
    {
        public WebDataContext()
            : base("DataContext")
        {
            Database.SetInitializer(new WebDataContextInitializer());
        }

  //      public DbSet<ContentPageExtension> ContentPageExtensions { get; set; }
    }
}