using AutoMapper;
using NannyApp.Models;
using NannyApp.ViewModels.API.Families;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.Mappings
{
    public class FamilyMappingProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            CreateMap<Family, FamilyViewModel>();
        }
    }
}
