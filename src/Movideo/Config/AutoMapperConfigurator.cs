using AutoMapper;
using Grappachu.Movideo.Data.Config;

namespace Grappachu.Apps.Movideo.Config
{
    public static class AutoMapperConfigurator
    {
        public static void Configure()
        {
            Mapper.Initialize(ExtendMappingConfiguration);
        }


        private static void ExtendMappingConfiguration(IMapperConfigurationExpression cfg)
        {
            AutoMapperConfiguration.Extend(cfg);
        }
    }
}