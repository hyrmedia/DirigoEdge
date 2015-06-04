using AutoMapper;
using DirigoEdge.Data.Entities.Extensibility;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Business.Models;
using DirigoEdgeCore.Data.Entities;

namespace DirigoEdge
{
    public class Mapping
    {
        public static void SetAutomapperMappings()
        {
            Mapper.CreateMap<PageDetails, ContentPage>()
                .ForAllMembers(p => p.Condition(c => !c.IsSourceValueNull));

            Mapper.CreateMap<ContentPage, PageDetails>();

            Mapper.CreateMap<ContentPageComplete, ContentPageExtension>()
                .ForAllMembers(p => p.Condition(c => !c.IsSourceValueNull));

            Mapper.CreateMap<ContentPageExtension, ContentPageComplete>()
                .ForAllMembers(p => p.Condition(c => !c.IsSourceValueNull)
                );
        }
    }
}