using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdge.CustomUtils;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Models.Shortcodes
{
    public class ResponsiveImageShortcode : IShortcode
    {
        public string GetDisplayName()
        {
            return "Responsive Image";
        }

        public string GetCSSClass()
        {
            return "responsive-image-wrapper";
        }

        public string GetHtml(Dictionary<string, string> parameters)
        {
            var model = new ResponsiveImageUtils.ResponsiveImageObject
            {
                ClassName = parameters != null && parameters.ContainsKey("class") ? parameters["class"] : "responsive-image",
                Sizes = ResponsiveImageUtils.ResponsiveImageSizes,
                ImagePath = "",
                AltText = "",
                Width = 0,
                Height = 0
            };

            if (parameters != null && parameters.ContainsKey("sizes") && !String.IsNullOrEmpty(parameters["sizes"]))
            {
                model.Sizes = parameters["sizes"];
            }

            if (parameters != null && parameters.ContainsKey("src") && !String.IsNullOrEmpty(parameters["src"]))
            {
                model.ImagePath = parameters["src"];
            }

            if (parameters != null && parameters.ContainsKey("alt") && !String.IsNullOrEmpty(parameters["alt"]))
            {
                model.AltText = parameters["alt"];
            }

            if (parameters != null && parameters.ContainsKey("width") && !String.IsNullOrEmpty(parameters["width"]))
            {
                model.Width = Convert.ToInt16(parameters["width"]);
            }

            if (parameters != null && parameters.ContainsKey("height") && !String.IsNullOrEmpty(parameters["height"]))
            {
                model.Height = Convert.ToInt16(parameters["height"]);
            }

            return DynamicModules.GetViewHtml("Partials/_ResponsiveImagePartial", model);
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


        public bool IsCacheable
        {
            get { return false; }
        }
    }
}