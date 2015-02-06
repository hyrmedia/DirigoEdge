using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class EditSlideShowViewModel : DirigoBaseModel
    {
        public SlideshowModule TheSlideShow;

        public EditSlideShowViewModel(int slideId)
        {
            TheSlideShow = Context.SlideshowModules.FirstOrDefault(x => x.SlideshowModuleId == slideId);

            // Make sure all of the inner slides are enumerated through before disposing the connection
            TheSlideShow.Slides = TheSlideShow.Slides.ToList();
        }
    }
}