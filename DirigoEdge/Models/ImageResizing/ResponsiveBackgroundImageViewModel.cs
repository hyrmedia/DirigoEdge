using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DirigoEdge.Utils;

namespace DirigoEdge.Models.ImageResizing
{
    public class ResponsiveBackgroundImageViewModel
    {
        public List<BackgroundImage> Images;
        public string ClassName;
        public string RandomId;

        private static readonly Random Random = new Random((int)DateTime.UtcNow.Ticks);
        private string RandomString(int size = 8)
        {
            var builder = new StringBuilder();
            char ch;
            for (var i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));                 
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public ResponsiveBackgroundImageViewModel(string className, List<BackgroundImage> images)
        {
            Images = images;
            ClassName = className;
            RandomId = RandomString();
        }
    }
}