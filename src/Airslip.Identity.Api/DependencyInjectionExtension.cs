using Airslip.Identity.Infrastructure.MongoDb;
using Airslip.Identity.Infrastructure.MongoDb.Identity;
using Airslip.Identity.MongoDb.Contracts;
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
            return services;
        }
    }
}