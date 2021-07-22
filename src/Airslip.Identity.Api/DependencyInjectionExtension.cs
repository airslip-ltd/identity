using Airslip.Identity.Infrastructure.MongoDb;
using Airslip.Identity.Infrastructure.MongoDb.Identity;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Yapily.Client;
using Airslip.Yapily.Client.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Airslip.Identity.Api
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddMongoServices(this IServiceCollection services)
        {
            services.AddSingleton<AirslipMongoDbContext>();
            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            return services;
        }
        
        public static IServiceCollection AddYapily(this IServiceCollection services)
        {
            services.AddScoped<IYapilyClient, YapilyClient>();
            return services;
        }
    }
}