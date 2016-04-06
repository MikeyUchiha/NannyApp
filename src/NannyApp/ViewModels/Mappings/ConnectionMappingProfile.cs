using AutoMapper;
using NannyApp.Models;
using NannyApp.ViewModels.API.Families;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.Mappings
{
    public class ConnectionMappingProfile : AutoMapper.Profile
    {
        protected override void Configure()
        {
            CreateMap<Connection, FamilyConnectionViewModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(s => s.User.UserName))
                .ForMember(dest => dest.ConnectionType, opt => opt.MapFrom(s => s.ConnectionType));
        }
    }

    public class ConnectionTypeResolver : ValueResolver<Connection, string>
    {
        protected override string ResolveCore(Connection source)
        {
            return source.ConnectionType.ToString();
        }
    }
}
