using Airslip.Common.Services.MongoDb;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Infrastructure.MongoDb;
using Airslip.Identity.Infrastructure.MongoDb.Identity;
using Airslip.Identity.MongoDb.Contracts.Interfaces;
using Airslip.Yapily.Client;
using Airslip.Yapily.Client.Contracts;
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