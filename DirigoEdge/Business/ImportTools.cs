using System;
using System.Linq;
using AutoMapper;
using DirigoEdge.Business.Models;
using DirigoEdge.Data.Context;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;
using Schema = DirigoEdgeCore.Business.Models.Schema;

namespace DirigoEdge.Business
{
    public class ImportTools
    {
        private readonly WebDataContext _context;

        public ImportTools(WebDataContext context)
        {
            _context = context;
        }

        public int AddContentModule(Module module)
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
            return mod.ContentModuleId;
        }

        public int AddSchema(Schema schemaModel)
        {
            var schema = Mapper.Map<Schema, DirigoEdgeCore.Data.Entities.Schema>(schemaModel);
            var existingSchema = _context.Schemas.FirstOrDefault(s => s.DisplayName == schemaModel.DisplayName);
            _context.Schemas.Add(schema);
            _context.SaveChanges();

            if (existingSchema != null)
            {
                schema.DisplayName = schema.DisplayName + " " + schema.SchemaId;
            }

            var schemaId = schema.SchemaId;
            return schemaId;
        }
    }
}