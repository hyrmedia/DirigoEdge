using System.Web.Mvc;
using DirigoEdge.Attributes;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.Controllers.Base
{
    [TimeConvert]
    public class WebBaseAdminController : DirigoBaseAdminController
    {
        protected new ILog Log = LogFactory.GetLog(typeof(WebBaseController));

        private WebDataContext _context;
        protected new WebDataContext Context
        {
            get { return _context ?? (_context = new WebDataContext()); }
        }

        protected static JsonResult GenericJsonError
        {
            get
            {
                return new JsonResult
                {
                    Data = new { success = false, message = "There was an error processing your request." }
                };
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Unhandled Exception in Controller. Rerouting to 500", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}