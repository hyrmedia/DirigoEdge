using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Models.Shortcodes
{
    public class ResponsiveImageShortcode : IShortcode
    {
        public string ClassName = "responsive-image";
        public string ImagePath = "";
        public string AltText = "";
        public int Width = 0;
        public int Height = 0;

        public string GetDisplayName()
        {
            return "Responsive Image";
        }

        public string GetCSSClass()
        {
            return ClassName;
        }

        public string GetHtml(Dictionary<string, string> parameters)
        {
            ClassName = parameters != null && parameters.ContainsKey("class") ? parameters["class"] : "responsive-image";

            if (parameters != null && parameters.ContainsKey("src") && !String.IsNullOrEmpty(parameters["src"]))
            {
                ImagePath = parameters["src"];
            }

            if (parameters != null && parameters.ContainsKey("alt") && !String.IsNullOrEmpty(parameters["alt"]))
            {
                AltText = parameters["alt"];
            }

            if (parameters != null && parameters.ContainsKey("width") && !String.IsNullOrEmpty(parameters["width"]))
            {
                Width = Convert.ToInt16(parameters["width"]);
            }

            if (parameters != null && parameters.ContainsKey("height") && !String.IsNullOrEmpty(parameters["height"]))
            {
                Height = Convert.ToInt16(parameters["height"]);
            }

            return DynamicModules.GetViewHtml("Partials/_ResponsiveImagePartial", this);
        }

        public object GetViewModel()
        {
            return null;
        }

        public string GetPartialView()
        {
            return null;
        }

        public bool PageBuilderEnabled()
        {
            return false;
        }
    }
}