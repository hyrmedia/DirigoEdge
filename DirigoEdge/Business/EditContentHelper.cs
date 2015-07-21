using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using AutoMapper;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Business;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Logging;
using EditContentViewModel = DirigoEdge.Areas.Admin.Models.ViewModels.EditContentViewModel;

namespace DirigoEdge.Business
{
    public class EditContentHelper
    {
        private readonly ILog _log = LogFactory.GetLog(typeof (EditContentHelper));
        private readonly AdminNavigationUtils _navigationUtils;
        private readonly WebDataContext _context;

        public EditContentHelper(WebDataContext context)
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

            var ext = _context.ContentPageExtensions.FirstOrDefault(ex => ex.ContentPageId == id);
            Mapper.Map(ext, model.ContentPage);

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

        public Boolean DeleteContentPage(int id)
        {
            try
            {
                var page = _context.ContentPages.First(x => x.ContentPageId == id);
                var revisions = _context.ContentPages.Where(x => x.ParentContentPageId == page.ContentPageId);
                var extenstion = _context.ContentPageExtensions.FirstOrDefault(ext => ext.ContentPageId == id);

                _context.ContentPages.Remove(page);

                if (extenstion != null)
                {
                    _context.ContentPageExtensions.Remove(extenstion);
                }

                if (revisions.Any())
                {
                    _context.ContentPages.RemoveRange(revisions);
                }

                var success = _context.SaveChanges();

                var util = new BookmarkUtil(_context);
                util.DeleteBookmarkForUrl("/admin/pages/editcontent/" + id + "/");

                return success > 0;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return false;
            }
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