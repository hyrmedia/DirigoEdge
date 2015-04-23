using System;
using System.Collections.Generic;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Models.ViewModels;

namespace DirigoEdge.Models.ViewModels
{
    public class TagSingleViewModel : DirigoBaseModel
    {
        public List<Blog> BlogRoll;
        public Dictionary<String, String> UserNameToDisplayName; 
        public string TheTag;

        public List<BlogsCategoriesViewModel.BlogCatExtraData> Categories;
    }
}