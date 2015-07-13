using System;
using System.Linq;
using System.Web.Mvc;
using DirigoEdge.CustomUtils;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.Attributes
{
    public class TimeConvertAttribute : FilterAttribute, IActionFilter
    {
        protected ILog Log = LogFactory.GetLog(typeof(TimeConvertAttribute));

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
                    // TODO - do we need to process these?
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Error in timezone post-processing", ex);
            }
        }
    }
}