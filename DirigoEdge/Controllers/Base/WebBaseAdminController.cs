using System.Web.Mvc;
using DirigoEdge.Business;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.Controllers.Base
{
    public class WebBaseAdminController : DirigoBaseAdminController
    {
        protected new ILog Log = LogFactory.GetLog(typeof(WebBaseController));

        private WebDataContext _context;
        protected new WebDataContext Context
        {
            get { return _context ?? (_context = new WebDataContext()); }
        }

        private ImportTools _importTools;
        protected ImportTools ImportTools
        {
            get { return _importTools ?? (_importTools = new ImportTools(Context)); }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Unhandled Exception in Controller. Rerouting to 500", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}