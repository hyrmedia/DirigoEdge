using System.Collections.Generic;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Business.Models;
using DirigoEdgeCore.Models.DataModels;
using DirigoEdgeCore.Models.ViewModels;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class EditContentViewModel : DraftableModel
    {
        public override int? ParentId()
        {
            return ContentPage.Details.ParentContentPageId;
        }

        public ContentPageComplete ContentPage { get; set; }

        public Dictionary<string, ContentTemplate> Templates { get; set; }
        public bool UseWordWrap { get; set; }
        public List<RevisionViewModel> Revisions { get; set; }
        public string SiteUrl { get; set; }
        public int BasePageId { get; set; }

        public List<Schema> Schemas { get; set; }
        public bool ShowSchemaSelector { get; set; }
        public bool ShowFieldEditor { get; set; }

        // Nav Id, Label
        public Dictionary<int, string> NavList { get; set; }
        public List<int> ParentNavIdsToDisable { get; set; }

        public EditContentViewModel()
        {
            ShowFieldEditor = false;
            ShowSchemaSelector = false;
            ContentPage = new ContentPageComplete();
        }
    }
}