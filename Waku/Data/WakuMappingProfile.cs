using AutoMapper;
using Waku.Data.Entities;
using Waku.Models;

namespace Waku.Data
{
    public class WakuMappingProfile : Profile
    {
        public WakuMappingProfile()
        {
            CreateMap<BlogPost, BlogPostModel>()
                .ForMember(p => p.Id, ex => ex.MapFrom(p => p.Id))
                .ReverseMap();
        }
    }
}
