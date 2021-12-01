using Airslip.Common.Services.MongoDb;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Services.MongoDb;
using Airslip.Identity.Services.MongoDb.Identity.Interfaces;
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
                .AddSingleton<IIdentityContext>(provider => provider.GetService<MongoDbContext>()!)
                .AddScoped<IUserManagerService, UserManagerService>();
            return services;
        }
    }
}