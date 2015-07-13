using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.CustomUtils;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Controllers;
using DirigoEdgeCore.Utils.Logging;
using log4net;
using ILog = DirigoEdgeCore.Utils.Logging.ILog;

namespace DirigoEdge.Controllers.Base
{
    [ConvertToLocal]
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

    public class ConvertToLocalAttribute : FilterAttribute, IActionFilter
    {
        protected ILog Log = LogFactory.GetLog(typeof(ConvertToLocalAttribute));

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                foreach (var param in filterContext.ActionParameters.Where(param => param.Value != null))
                {
                    TimeUtils.ConvertAllMembersToUTC(param.Value);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error in timezone post-processing", ex);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var type = filterContext.Result.GetType();

                if (type == typeof(ActionResult))
                {
                }

                if (type == typeof(ViewResult))
                {
                    TimeUtils.ConvertAllMembersToLocal(((ViewResult)filterContext.Result).Model);
                }

                if (type == typeof(JsonResult))
                {
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error in timezone post-processing", ex);
            }
        }
    }
}