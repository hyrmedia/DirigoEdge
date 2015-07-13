using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.Attributes;
using DirigoEdge.CustomUtils;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.Controllers.Base
{
    [TimeConvert]
    public class WebBaseController : DirigoBaseController
    {
        protected new ILog Log = LogFactory.GetLog(typeof(WebBaseController));

        private WebDataContext _context;
        protected new WebDataContext Context
        {
            get { return _context ?? (_context = new WebDataContext()); }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Unhandled Exception in Controller. Rerouting to 500", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}