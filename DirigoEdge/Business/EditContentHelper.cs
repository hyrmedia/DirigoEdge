﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DirigoEdgeCore.Business;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;
using EditContentViewModel = DirigoEdge.Areas.Admin.Models.ViewModels.EditContentViewModel;

namespace DirigoEdge.Business
{
    public class EditContentHelper
    {
        private readonly AdminNavigationUtils _navigationUtils;
        private readonly DataContext _context;

        public EditContentHelper(DataContext context)
        {
            _context = context;
            _navigationUtils = new AdminNavigationUtils(context);
        }

        public void LoadContentViewById(int id, EditContentViewModel model)
        {
            model.Heading = "Edit Page";
            model.EditURL = "editcontent";

            var contentLoader = new ContentLoader();
            model.ContentPage.Details = contentLoader.GetDetailById(id);

            model.ShowFieldEditor = model.ContentPage.Details.SchemaId > -1;

            // If we are editing a draft, we actually need to be editing the parent page, but keep the drafts contents (html, css, meta, etc).
            // To accomplish this, we can simply change the id of the page we're editing in memory, to the parent page.
            model.BasePageId = model.ContentPage.Details.IsRevision
                ? Convert.ToInt32(model.ContentPage.Details.ParentContentPageId)
                : model.ContentPage.Details.ContentPageId;

            var user = GetCurrentUser();
            model.UseWordWrap = user.ContentAdminWordWrap;
            model.SiteUrl = HTTPUtils.GetFullyQualifiedApplicationPath();

            // Check to see if there is a newer version available
            var newerVersionId = GetNewerVersionId(model.BasePageId, model.ContentPage.Details.PublishDate, model.ContentPage.Details.ContentPageId);
            if (newerVersionId != 0)
            {
                model.IsNewerVersion = true;
                model.NewerVersionId = newerVersionId;
            }

            model.Templates = contentLoader.GetAllContentTemplates();
            model.Revisions = GetPageRevisions(model.BasePageId);

            // Get list of schemas for drop down
            model.Schemas = contentLoader.GetAllSchemata();

            // Grab the formatted nav list for the category drop down
            model.NavList = _navigationUtils.GetNavList();

            model.ParentNavIdsToDisable = contentLoader.GetNavItemsForContentPage(model.ContentPage.Details.ContentPageId);
            model.BookmarkTitle = model.ContentPage.Details.Title;
        }

        private int GetNewerVersionId(int basePageId, DateTime? publishDate, int contentPageId)
        {
            var newerVersion = _context.ContentPages.Where(x => (x.ParentContentPageId == basePageId || x.ContentPageId == basePageId)
                                                                                     && x.PublishDate > publishDate
                                                                                     && x.ContentPageId != contentPageId)
                                                                                     .OrderByDescending(x => x.PublishDate).FirstOrDefault();

            return newerVersion != null
                ? newerVersion.ContentPageId
                : 0;
        }

        private User GetCurrentUser()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser == null)
            {
                return null;
            }

            var userName = membershipUser.UserName;
            var user = _context.Users.FirstOrDefault(x => x.Username == userName);
            return user;
        }

        private List<RevisionViewModel> GetPageRevisions(int basePageId)
        {
            return _context.ContentPages.Where(x => x.ParentContentPageId == basePageId || x.ContentPageId == basePageId).OrderByDescending(x => x.PublishDate).ToList().Select(rev => new RevisionViewModel
            {
                Date = rev.PublishDate,
                ContentId = rev.ContentPageId,
                AuthorName = rev.DraftAuthorName,
                WasPublished = rev.WasPublished
            }).ToList();
        }
    }
}