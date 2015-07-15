using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using AutoMapper;
using DirigoEdge.Areas.Admin.Models;
using DirigoEdge.Business.Models;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Logging;
using Newtonsoft.Json;
using Schema = DirigoEdgeCore.Business.Models.Schema;

namespace DirigoEdge.Business
{
    public class ImportTools
    {
        private static readonly ILog Log = LogFactory.GetLog(typeof(ImportTools));
        private readonly WebDataContext _context;

        public ImportTools(WebDataContext context)
        {
            _context = context;
        }

        public ImportResult AddModule(Module module)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(usr => usr.Username == module.DraftAuthorName) ??
                           UserUtils.GetCurrentUser(_context);

                var mod = Mapper.Map<Module, ContentModule>(module);
                var existingModule = _context.ContentModules.FirstOrDefault(m => m.ModuleName == module.ModuleName);

                mod.DraftAuthorName = user.Username;
                mod.CreateDate = DateTime.UtcNow;

                if (!String.IsNullOrEmpty(module.SchemaName))
                {
                    var schema = _context.Schemas.FirstOrDefault(s => s.DisplayName == module.SchemaName);
                    if (schema != null)
                    {
                        mod.SchemaId = schema.SchemaId;
                    }
                }

                _context.ContentModules.Add(mod);
                _context.SaveChanges();

                if (existingModule != null)
                {
                    mod.ModuleName = mod.ModuleName + " " + mod.ContentModuleId;
                }

                return new ImportResult
                {
                    Id = mod.ContentModuleId,
                    Name = mod.ModuleName,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                Log.Error("Error Importing Module named " + module.ModuleName, ex);
                return new ImportResult
                {
                    Name = module.ModuleName,
                    Message = "Error: " + ex.Message
                };
            }
        }


        public List<ImportResult> AddModules(List<Module> modules)
        {
            if (modules == null)
            {
                return null;
            }

            var moduleIds = new List<ImportResult>();

            foreach (var module in modules)
            {
                var contentModuleId = AddModule(module);
                moduleIds.Add(contentModuleId);
            }

            return moduleIds;

        }

        public ImportResult AddSchema(Schema schemaModel)
        {
            try
            {
                var schema = Mapper.Map<Schema, DirigoEdgeCore.Data.Entities.Schema>(schemaModel);
                var existingSchema = _context.Schemas.FirstOrDefault(s => s.DisplayName == schemaModel.DisplayName);
                
                _context.Schemas.Add(schema);
                _context.SaveChanges();

                if (existingSchema != null)
                {
                    schema.DisplayName = schema.DisplayName + " " + schema.SchemaId;
                }

                return new ImportResult
                {
                    Id = schema.SchemaId,
                    Name = schema.DisplayName,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                Log.Error("Error Importing Schema named " + schemaModel.DisplayName, ex);
                return new ImportResult
                {
                    Name = schemaModel.DisplayName,
                    Message = "Error: " + ex.Message
                };
            }
        }

        public List<ImportResult> AddSchemas(List<Schema> schemas)
        {
            if (schemas == null)
            {
                return null;
            }

            var schemaIds = new List<ImportResult>();

            foreach (var schema in schemas)
            {
                var schemaId = AddSchema(schema);
                schemaIds.Add(schemaId);
            }

            return schemaIds;
        }

        public static ImportData TryParseFileAsImportData(HttpPostedFileBase file)
        {
            try
            {
                return JsonConvert.DeserializeObject<ImportData>((new StreamReader(file.InputStream)).ReadToEnd());
            }
            catch(Exception ex)
            {
                Log.Error("Error Parsing uploaded file as ImportData", ex);
                return null;
            }
        }

        public ImportResults AddContent(ImportData data)
        {
            var results = new ImportResults();

            results.SchemaResults = AddSchemas(data.Schemas);
            results.ModuleResults = AddModules(data.Modules);

            return results;
        }
    }
}