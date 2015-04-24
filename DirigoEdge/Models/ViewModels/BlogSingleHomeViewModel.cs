using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Models.ViewModels;
using DirigoEdgeCore.Utils;

namespace DirigoEdge.Models.ViewModels
{
    public class BlogSingleHomeViewModel : DirigoBaseModel
    {
        public Blog TheBlog;
        public BlogUser TheBlogUser;
        public BlogRelatedViewModel RelatedPosts;

        public bool ShowFacebookLikeButton;
        public bool ShowFacebookComments;
        public bool AllCommentsAreDisabled;
        public bool UseDisqusComments;
        public string DisqusShortName;
        public List<BlogsCategoriesViewModel.BlogCatExtraData> Categories;

        public BlogAuthorViewModel BlogAuthorModel;

        public string BlogAbsoluteUrl;

        public BlogSingleHomeViewModel()
        {
        }
    }
}