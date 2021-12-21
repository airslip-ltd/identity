using Airslip.Common.Auth.Data;
using Airslip.Common.Repository.Entities;
using Airslip.Common.Services.MongoDb;
using Airslip.Common.Services.MongoDb.Extensions;
using Airslip.Identity.Api.Contracts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Services.MongoDb
{
    public static class Initialisation
    {
        public static Task InitialiseSettings(IMongoDatabase mongoDatabase)
        {
            // // Map classes
            AirslipMongoDbBase.MapEntity<RefreshToken>();
            AirslipMongoDbBase.MapEntity<OpenBankingProvider>();
            AirslipMongoDbBase.MapEntity<BasicAuditInformation>();
            
            mongoDatabase.CreateCollectionForEntity<ApiKey>();
            mongoDatabase.CreateCollectionForEntity<QrCode>();
            mongoDatabase.CreateCollectionForEntity<User>();
            
            return Task.CompletedTask;
        }

        public static async Task InitialiseRoles(this IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            RoleManager<ApplicationRole> RoleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roleNames = { ApplicationRoles.UserManager };

            foreach (var roleName in roleNames)
            {
                bool roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            //Here you could create a super user who will maintain the web app
            // var poweruser = new ApplicationUser
            // {
            //
            //     UserName = Configuration["AppSettings:UserName"],
            //     Email = Configuration["AppSettings:UserEmail"],
            // };
            // //Ensure you have these values in your appsettings.json file
            // string userPWD = Configuration["AppSettings:UserPassword"];
            // var _user = await UserManager.FindByEmailAsync(Configuration["AppSettings:AdminUserEmail"]);
            //
            // if(_user == null)
            // {
            //     var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
            //     if (createPowerUser.Succeeded)
            //     {
            //         //here we tie the new user to the role
            //         await UserManager.AddToRoleAsync(poweruser, "Admin");
            //
            //     }
            // }
        }
    }
}