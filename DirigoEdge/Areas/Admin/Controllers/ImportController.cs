using System;
using System.Web;
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
            return TryImportContent(data);
        }

        [HttpPost]
        [UserIsLoggedIn]
        public JsonResult ContentFile(HttpPostedFileBase file)
        {
            var data = ImportTools.TryPaseFileAsImportData(file);

            if (data == null)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        Success = false,
                        Error = "Unable to parse file"
                    }
                };
            }

            return TryImportContent(data);
        }

        public JsonResult TryImportContent(ImportData data)
        {
            try
            {
                var results = ImportTools.AddContent(data);

                return new JsonResult
                {
                    Data = new
                    {
                        results,
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

        public ActionResult Index()
        {
            return View();
        }
    }
}