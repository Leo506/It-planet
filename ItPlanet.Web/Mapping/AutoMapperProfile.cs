using AutoMapper;
using ItPlanet.Domain.Dto;
using ItPlanet.Domain.Models;

namespace ItPlanet.Web.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AccountDto, Account>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Animals, opt => opt.Ignore());

        CreateMap<AnimalDto, Animal>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.LifeStatus, opt => opt.Ignore())
            .ForMember(x => x.Types, opt => opt.Ignore())
            .ForMember(x => x.VisitedLocations, opt => opt.Ignore())
            .ForMember(x => x.VisitedPoints, opt => opt.Ignore())
            .ForMember(x => x.DeathDateTime, opt => opt.Ignore())
            .ForMember(x => x.ChippingDateTime, opt => opt.Ignore())
            .ForMember(x => x.Chipper, opt => opt.Ignore())
            .ForMember(x => x.AnimalTypes, opt => opt.Ignore());

        CreateMap<UpdateAnimalDto, Animal>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.DeathDateTime, opt => opt.Ignore())
            .ForMember(x => x.Types, opt => opt.Ignore())
            .ForMember(x => x.VisitedLocations, opt => opt.Ignore())
            .ForMember(x => x.VisitedPoints, opt => opt.Ignore())
            .ForMember(x => x.Chipper, opt => opt.Ignore())
            .ForMember(x => x.ChippingDateTime, opt => opt.Ignore())
            .ForMember(x => x.AnimalTypes, opt => opt.Ignore());
    }
}