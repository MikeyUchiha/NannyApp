using NannyApp.Models;
using NannyApp.ViewModels.API.Families;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.Mappings
{
    public class FamilyPhotoMappingProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            ShouldMapField = fi => false;

            CreateMap<FamilyPhoto, FamilyPhotoViewModel>()
                .ConstructUsing(src => new FamilyPhotoViewModel(src));
        }
    }
}
