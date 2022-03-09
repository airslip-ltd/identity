using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Services.MongoDb;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Services.MongoDb;
using Airslip.Identity.Services.MongoDb.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Airslip.Identity.Api
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddMongoServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAirslipMongoDb<MongoDbContext>(configuration, Initialisation.InitialiseSettings)
                .services
                .AddSingleton<IIdentityContext>(provider => provider.GetService<MongoDbContext>()!)
                .AddScoped<IUserManagerService, UserManagerService>()
                .AddSingleton<IMongoDbMigrator, MongoDbMigrator>()
                .AddSingleton<SearchContext>()
                .AddSingleton<ISearchContext>(provider => provider.GetService<SearchContext>()!);

            return services;
        }
    }
}