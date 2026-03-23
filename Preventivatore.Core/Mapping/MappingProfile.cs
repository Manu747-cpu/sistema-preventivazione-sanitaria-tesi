using AutoMapper;
using Preventivatore.Core.DTOs;
using Preventivatore.Core.Entities;
using System.Text.Json;

namespace Preventivatore.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Polizza
            CreateMap<CreatePolizzaDto, Polizza>();
            CreateMap<UpdatePolizzaDto, Polizza>();
            CreateMap<Polizza, PolizzaDto>();

            CreateMap<SubCategoria, SubCategoriaDto>()
                .ForMember(dest => dest.Colonne,
                           opt => opt.MapFrom(src => src.Colonne
                                                       .OrderBy(c => c.Ordine)
                                                       .Select(c => c.Intestazione)))
                .ForMember(dest => dest.Righe,
                           opt => opt.MapFrom(src => src.Righe
                               .OrderBy(r => r.Ordine)
                               .Select(r => System.Text.Json.JsonSerializer
                                   .Deserialize<List<string>>(r.CelleJson, (JsonSerializerOptions)null)
                               )
                           ));

            CreateMap<SubCategoriaDto, SubCategoria>()
                .ForMember(dest => dest.Colonne,
                           opt => opt.Ignore()) // le gestiremo manualmente in repo/servizio
                .ForMember(dest => dest.Righe,
                           opt => opt.Ignore());

            // Preventivo
            CreateMap<CreatePreventivoDto, Preventivo>();
            CreateMap<UpdatePreventivoDto, Preventivo>();
            CreateMap<Preventivo, PreventivoDto>();

            // MacroCategoriaPolizza → MacroCategoriaDto
            CreateMap<MacroCategoriaPolizza, MacroCategoriaDto>()
                .ForMember(dest => dest.Image, opt => opt.Ignore())            // non mappare IFormFile
                .ForMember(dest => dest.UrlImmagine,
                           opt => opt.MapFrom(src => src.UrlImmagine));

            // MacroCategoriaDto → MacroCategoriaPolizza
            CreateMap<MacroCategoriaDto, MacroCategoriaPolizza>()
                .ForMember(dest => dest.UrlImmagine,
            opt => opt.MapFrom(src => src.UrlImmagine))
                .ForAllMembers(opt => opt.Condition(
            (src, dest, srcMember) => srcMember != null));
        }
    }
}
