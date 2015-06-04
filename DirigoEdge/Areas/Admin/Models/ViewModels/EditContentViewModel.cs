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

        public ContentPageComplete ContentPage;

        public Dictionary<string, ContentTemplate> Templates;
        public bool UseWordWrap;
        public List<RevisionViewModel> Revisions;
        public string SiteUrl;
        public int BasePageId;

        public List<Schema> Schemas;
        public bool ShowSchemaSelector = false;
        public bool ShowFieldEditor = false;

        // Nav Id, Label
        public Dictionary<int, string> NavList;
        public List<int> ParentNavIdsToDisable;

        public EditContentViewModel()
        {
            ContentPage = new ContentPageComplete();
        }
    }
}