using AutoMapper;
using VkInternship.App.Models;
using VkInternship.Data.Entities;

namespace VkInternship.App.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserInfo>()
            .ForMember(dest => dest.Group, act => act.MapFrom(src => src.Group.Code.ToString()));
    }
}