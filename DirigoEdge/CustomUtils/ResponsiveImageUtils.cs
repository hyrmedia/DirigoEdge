using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirigoEdge.CustomUtils
{
    public class ResponsiveImageUtils
    {
        public const string ResponsiveImageSizes = "100vw";
        public readonly static List<ResponsiveImageRoute> ResponsiveImageRoutes = new List<ResponsiveImageRoute>
        {
            new ResponsiveImageRoute
            {
                Name = "Small",
                Width = 480,
                Path = "images/small"
            },
            new ResponsiveImageRoute
            {
                Name = "Medium",
                Width = 1024,
                Path = "images/medium"
            },
            new ResponsiveImageRoute
            {
                Name = "Large",
                Width = 1920,
                Path = "images/large"
            },
            new ResponsiveImageRoute
            {
                Name = "Extreme",
                Width = 2560,
                Path = "images/extreme"
            }
        };

        public class ResponsiveImageRoute
        {
            public String Name { get; set; }
            public Int16 Width { get; set; }
            public String Path { get; set; }
        }

        public class ResponsiveImageObject
        {
            public string ClassName { get; set; }
            public string ImagePath { get; set; }
            public string AltText { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}