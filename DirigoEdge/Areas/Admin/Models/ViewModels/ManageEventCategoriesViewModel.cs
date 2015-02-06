using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class ManageEventCategoriesViewModel : DirigoBaseModel
    {
        public List<EventCategory> EventCategories;

        public ManageEventCategoriesViewModel()
        {
            EventCategories = Context.EventCategories.ToList();
        }
    }
}