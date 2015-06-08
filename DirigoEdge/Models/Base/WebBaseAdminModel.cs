using DirigoEdge.Data.Context;
using DirigoEdgeCore.Models;
using DirigoEdgeCore.Utils.Logging;

namespace DirigoEdge.Models.Base
{
    public class WebBaseAdminModel : DirigoBaseModel
    {
        protected new ILog Log = LogFactory.GetLog(typeof(WebBaseAdminModel));

        private WebDataContext _context;
        protected new WebDataContext Context
        {
            get { return _context ?? (_context = new WebDataContext()); }
        }
    }
}