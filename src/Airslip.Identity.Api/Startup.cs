using Airslip.Identity.Api.Application;
using Airslip.Identity.Api.Auth;
using Airslip.Identity.Api.Middleware;
using Airslip.Identity.Infrastructure.MongoDb.Identity;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace Airslip.Identity.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors()
                .AddControllers()
                .AddNewtonsoftJson();

            services
                .AddHttpClient()
                .AddHttpContextAccessor();
            
            services
                .AddOptions()
                .Configure<MongoDbSettings>(Configuration.GetSection(nameof(MongoDbSettings)));

            services
                .AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>()
                .AddScoped<Token>()
                .AddScoped<JwtBearerToken>()
                .Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)))
                .AddAuthorization()
                //.AddAuthorization(options => { AddPolicy(options, new TenantPolicies().Policies); })
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
                    Configuration["MongoDbSettings:ConnectionString"],
                    bool.Parse(Environment.GetEnvironmentVariable("DBCONTEXT_INMEMORY") ?? "false")
                        ? "airslipTestDb"
                        : Configuration["MongoDbSettings:DatabaseName"])
                .AddDefaultTokenProviders();
            
            services.AddMediatR(ApplicationAssembly.Reference);
            // For all the validators, register them with dependency injection as scoped
            AssemblyScanner.FindValidatorsInAssembly(ApplicationAssembly.Reference)
                .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            services.AddMongoServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<ErrorHandlingMiddleware>()
                .UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers()
                        .RequireAuthorization();
                });
        }
    }
}