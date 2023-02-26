using AutoMapper;
using ItPlanet.Domain.Models;
using ItPlanet.Dto;

namespace ItPlanet.Web.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AccountDto, Account>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Animals, opt => opt.Ignore());
    }
}