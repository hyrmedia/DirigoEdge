using System;
using System.Collections.Generic;
using System.Linq;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;

namespace DirigoEdge.Areas.Admin.Models.ViewModels
{
    public class EditModuleViewModel : DraftableModel
    {
        public ContentModule TheModule;

        public List<Schema> Schemas;
        public List<RevisionViewModel> Revisions;

        public override int? ParentId()
        {
            return TheModule.ParentContentModuleId;
        }

        public EditModuleViewModel(int id)
        {
            Heading = "Edit Module";
            EditURL = "editmodule";

            TheModule = Context.ContentModules.FirstOrDefault(x => x.ContentModuleId == id);

            if (TheModule == null)
            {
                return;
            }

            BookmarkTitle = TheModule.ModuleName;
            // Set Unparsed Html on Legacy Modules
            if (String.IsNullOrEmpty(TheModule.HTMLUnparsed) && !String.IsNullOrEmpty(TheModule.HTMLContent))
            {
                TheModule.HTMLUnparsed = TheModule.HTMLContent;
            }

            var newerVersion = Context.ContentModules.Where(x => (x.ParentContentModuleId == id || x.ContentModuleId == id)
                && x.CreateDate > TheModule.CreateDate
                && x.ContentModuleId != TheModule.ContentModuleId).OrderByDescending(x => x.CreateDate).FirstOrDefault();

            if (newerVersion != null)
            {
                NewerVersionId = newerVersion.ContentModuleId;
            }

            var parentId = TheModule.ParentContentModuleId ?? TheModule.ContentModuleId;

            Revisions = Context.ContentModules.Where(x => x.ParentContentModuleId == parentId || x.ContentModuleId == parentId).OrderByDescending(x => x.CreateDate).ToList().Select(rev => new RevisionViewModel
            {
                Date = rev.CreateDate,
                ContentId = rev.ContentModuleId,
                AuthorName = rev.DraftAuthorName,
                WasPublished = rev.WasPublished
            }).ToList();
            Schemas = Context.Schemas.ToList();
        }
    }
}