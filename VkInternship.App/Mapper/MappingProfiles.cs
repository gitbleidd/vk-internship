using AutoMapper;
using VkInternship.App.Models;
using VkInternship.Data.Entities;

namespace VkInternship.App.Mapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserInfo>()
            .ForMember(dest => dest.Group, act => act.MapFrom(src => src.UserGroup.Code.ToString()));
    }
}