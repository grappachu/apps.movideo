using AutoMapper;
using Grappachu.Movideo.Core.Models;
using Grappachu.Movideo.Data.LocalDb.Models;

namespace Grappachu.Movideo.Data.Config
{
    public static class AutoMapperConfiguration
    {
        public static void Extend(IMapperConfigurationExpression cfg)
        { 
            cfg.CreateMap<Movie, TmdbMovie>();
            cfg.CreateMap<TmdbMovie, Movie>();

            //cfg.CreateMap<Permission, PermissionDTO>()
            //    .ForMember(x => x.RequiredParents,
            //        o => o.MapFrom(m => m.RequiredParent == null ? new int[0] : new[] {m.RequiredParent.Id}));
            //cfg.CreateMap<PermissionDTO, Permission>();


        }
    }
}
