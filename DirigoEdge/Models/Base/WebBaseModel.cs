using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DirigoEdge.Data.Context;
using DirigoEdge.Utils;
using DirigoEdge.Utils.Logging;
using DirigoEdgeCore.Data.Context;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.Models.Base
{
    public class WebBaseModel : DirigoBaseModel
    {
        protected new ILog Log = LogFactory.GetLog(typeof(WebBaseModel));

        private WebDataContext _context;
        protected new WebDataContext Context
        {
            get { return _context ?? (_context = new WebDataContext()); }
        }
    }
}