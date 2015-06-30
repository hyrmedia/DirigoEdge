using System;
using System.Web.Mvc;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Business;
using DirigoEdge.Business.Models;
using DirigoEdge.Controllers.Base;

namespace DirigoEdge.Areas.Admin.Controllers
{
    public class ImportController : WebBaseAdminController
    {
        private ImportTools _importTools;
        protected ImportTools ImportTools
        {
            get { return _importTools ?? (_importTools = new ImportTools(Context)); }
        }

        [HttpPost]
        [UserIsLoggedIn]
        public JsonResult Content(ImportData data)
        {
            try
            {
                var moduleIds = ImportTools.AddModules(data.Modules);
                var schemaIds = ImportTools.AddSchemas(data.Schemas);

                return new JsonResult
                {
                    Data = new
                    {
                        schemaIds,
                        moduleIds,
                        Success = true
                    }
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        Success = false,
                        Error = ex.Message
                    }
                };
            }
        }
    }
}