using Airslip.Identity.Api.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public static class DatabaseSetup
    {
        public static async Task Warm(IServiceProvider serviceProvider)
        {
            await WarmUsers(serviceProvider);
        }

        private static Task WarmUsers(IServiceProvider serviceProvider)
        {
            IUserService userService = serviceProvider.GetRequiredService<IUserService>();

            return userService.Get(string.Empty);
        }
    }
}
