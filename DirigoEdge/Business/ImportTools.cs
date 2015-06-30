using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DirigoEdge.Business.Models;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;
using DirigoEdgeCore.Utils.Logging;
using Schema = DirigoEdgeCore.Business.Models.Schema;

namespace DirigoEdge.Business
{
    public class ImportTools
    {
        private readonly ILog Log = LogFactory.GetLog(typeof(ImportTools));

        private readonly WebDataContext _context;

        public ImportTools(WebDataContext context)
        {
            _context = context;
        }

        public class ImportResult
        {
            public String Name { get; set; }
            public int? Id { get; set; }
            public String Message { get; set; }
        }

        public ImportResult AddModule(Module module)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(usr => usr.Username == module.DraftAuthorName) ??
                           UserUtils.GetCurrentUser(_context);

                var mod = Mapper.Map<Module, ContentModule>(module);
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
            var schemaIds = new List<ImportResult>();

            foreach (var schema in schemas)
            {
                var schemaId = AddSchema(schema);
                schemaIds.Add(schemaId);
            }

            return schemaIds;
        }
    }
}