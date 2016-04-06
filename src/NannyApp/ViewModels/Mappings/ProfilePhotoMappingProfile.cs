using NannyApp.Models;
using NannyApp.ViewModels.API.Users;
using NannyApp.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.Mappings
{
    public class ProfilePhotoMappingProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            ShouldMapField = fi => false;

            CreateMap<ProfilePhoto, ProfilePhotoViewModel>()
                .ConstructUsing(src => new ProfilePhotoViewModel(src));
        }
    }
}
