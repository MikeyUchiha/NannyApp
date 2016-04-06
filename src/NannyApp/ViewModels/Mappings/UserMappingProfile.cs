using AutoMapper;
using NannyApp.Models;
using NannyApp.ViewModels.API.Families;
using NannyApp.ViewModels.API.Users;
using NannyApp.ViewModels.Users;

namespace NannyApp.ViewModels.Mappings
{
    public class UserMappingProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(s => s.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(s => s.Country))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(s => s.JoinDate))
                .ForMember(dest => dest.LastLoginDate, opt => opt.MapFrom(s => s.LastLoginDate))
                .ForMember(dest => dest.ProfilePhoto, opt => opt.MapFrom(s => Mapper.Map<FilePath, ProfilePhotoViewModel>(s.ProfilePhoto)));
        }
    }
}
