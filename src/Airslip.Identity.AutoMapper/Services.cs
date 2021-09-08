using Airslip.Common.Repository.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Identity.AutoMapper.Extensions;
using Airslip.Identity.AutoMapper.Implementations;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Airslip.Identity.AutoMapper
{
    public static class Services
    {
        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(IModelMapper<>), typeof(AutoMapperModelMapper<>));
            
            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.IgnoreUnmapped();
                cfg.CreateMap<ApiKey, ApiKeyModel>();
                cfg.CreateMap<ApiKeyModel, ApiKey>();
            });

            mappingConfig.AssertConfigurationIsValid();
            
            var mapper = mappingConfig.CreateMapper();
            serviceCollection.AddSingleton(mapper);
        }
    }
}