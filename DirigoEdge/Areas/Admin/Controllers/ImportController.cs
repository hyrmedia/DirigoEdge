using System;
using System.Collections.Generic;
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
            var moduleResults = new List<ImportTools.ImportResult>();
            var schemaResults = new List<ImportTools.ImportResult>();

            try
            {
                moduleResults = ImportTools.AddModules(data.Modules);
                schemaResults = ImportTools.AddSchemas(data.Schemas);

                return new JsonResult
                {
                    Data = new
                    {
                        schemaResults,
                        moduleResults,
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
                        schemaResults,
                        moduleResults,
                        Success = false,
                        Error = ex.Message
                    }
                };
            }
        }
    }
}