using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Waku.Data.Entities;
using Waku.Models;

namespace Waku.Data
{
    public class WakuMappingProfile : Profile
    {
        public WakuMappingProfile()
        {
            CreateMap<IdentityUser, UserModel>()
                .ForMember(u => u.Id, ex => ex.MapFrom(u => u.Id))
                .ReverseMap();

            CreateMap<BlogPost, BlogPostModel>()
                .ForMember(p => p.Id, ex => ex.MapFrom(p => p.Id))
                .ReverseMap();
        }
    }
}
