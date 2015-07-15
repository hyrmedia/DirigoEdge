using AutoMapper;
using DirigoEdge.Areas.Admin.Models.DataModels;
using DirigoEdge.Areas.Admin.Controllers;
using DirigoEdge.Business.Models;
using DirigoEdge.Data.Entities.Extensibility;
using DirigoEdge.Models.ViewModels;
using DirigoEdgeCore.Business.Models;
using DirigoEdgeCore.Data.Entities;
using DirigoEdgeCore.Utils;

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
                .ForAllMembers(p => p.Condition(c => !c.IsSourceValueNull));

            Mapper.CreateMap<Module, ContentModule>().ReverseMap();
            Mapper.CreateMap<Settings, SiteSettings>();
            Mapper.CreateMap<BlogController.EditBlogModel, Blog>()
               .ForMember(dest => dest.Title,
                           opts => opts.MapFrom(src => ContentUtils.ScrubInput(src.Title)))
               .ForMember(dest => dest.ImageUrl,
                           opts => opts.MapFrom(src => ContentUtils.ScrubInput(src.ImageUrl)))
               .ForMember(dest => dest.PermaLink,
                           opts => opts.MapFrom(src => ContentUtils.GetFormattedUrl(src.PermaLink)));
         }
    }
}