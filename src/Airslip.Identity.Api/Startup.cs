using Airslip.Common.Contracts;
using Airslip.Email.Client;
using Airslip.Identity.Api.Application;
using Airslip.Identity.Api.Auth;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Middleware;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Identity;
using Airslip.Infrastructure.BlobStorage;
using Airslip.Security.Jwt;
using Airslip.Yapily.Client.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System;
using System.IO;
using System.Reflection;
using Polly;
using Serilog;

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
                .Configure<MongoDbSettings>(Configuration.GetSection(nameof(MongoDbSettings)))
                .Configure<PublicApiSettings>(Configuration.GetSection(nameof(PublicApiSettings)))
                .Configure<YapilySettings>(Configuration.GetSection(nameof(YapilySettings)))
                .Configure<EmailConfigurationSettings>(Configuration.GetSection(nameof(EmailConfigurationSettings)));

            services.AddSingleton(serviceProvider =>
            {
                IOptions<EmailConfigurationSettings> EmailConfigurationOptions =
                    serviceProvider.GetRequiredService<IOptions<EmailConfigurationSettings>>();

                EmailConfigurationSettings emailConfigurationSettings = EmailConfigurationOptions.Value;
    
                return EmailSenderFactory.Setup(
                    emailConfigurationSettings.FromName,
                    emailConfigurationSettings.FromEmail,
                    emailConfigurationSettings.ApiKey);
            });
            
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                IOptions<MongoDbSettings> mongoDbSettingsOptions =
                    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>();

                return new MongoClient(mongoDbSettingsOptions.Value.ConnectionString);
            });

            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                IOptions<MongoDbSettings> mongoDbSettingsOptions =
                    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>();

                IMongoClient mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

                return mongoClient.GetDatabase(mongoDbSettingsOptions.Value.DatabaseName);
            });

            services
                .AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>()
                .AddScoped<Token>()
                .Configure<JwtSettings>(Configuration.GetSection(nameof(JwtSettings)))
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer()
                .AddCookie(options => { options.LoginPath = new PathString("/v1/identity/google-login"); })
                .AddGoogle(GoogleDefaults.AuthenticationScheme,
                    options =>
                    {
                        IConfigurationSection googleAuthNSection =
                            Configuration.GetSection("IdentityExternalProviders:Google");

                        options.ClientId = googleAuthNSection["ClientId"];
                        options.ClientSecret = googleAuthNSection["ClientSecret"];
                    });

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
                    Configuration["MongoDbSettings:ConnectionString"],
                    bool.Parse(Environment.GetEnvironmentVariable("DBCONTEXT_INMEMORY") ?? "false")
                        ? "airslipTestDb"
                        : Configuration["MongoDbSettings:DatabaseName"])
                .AddDefaultTokenProviders();

            services.AddHttpClient<IYapilyClient>(nameof(IYapilyClient),
                    (serviceProvider, yapilyHttpClient) =>
                    {
                        IOptions<YapilySettings> yapilySettingsOptions =
                            serviceProvider.GetRequiredService<IOptions<YapilySettings>>();

                        yapilyHttpClient.AddDefaults(
                            yapilySettingsOptions.Value.BaseUri,
                            "Basic",
                            $"{yapilySettingsOptions.Value.ApiKey}:{yapilySettingsOptions.Value.ApiSecret}");
                    })
                .AddTransientHttpErrorPolicy(p =>
                    p.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddSingleton<IStorage<BlobStorageModel>, BlobStorageRepository>(_ => new BlobStorageRepository(
                Configuration["BlobStorageSettings:ConnectionString"],
                Configuration["BlobStorageSettings:Container"]));

            services.AddMediatR(ApplicationAssembly.Reference);
            // For all the validators, register them with dependency injection as scoped
            AssemblyScanner.FindValidatorsInAssembly(ApplicationAssembly.Reference)
                .ForEach(item => services.AddScoped(item.InterfaceType, item.ValidatorType));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<BasePathDocumentFilter>();

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string filePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(filePath);
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Identity API",
                        Version = "1.0.0",
                        Description = "Includes all API endpoints for authorisation."
                    }
                );
            });

            services
                .AddApiVersioning(options => { options.ReportApiVersions = true; })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services
                .AddMongoServices()
                .AddYapily();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider,  ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            loggerFactory.AddSerilog();
            
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airslip.Identity.Api v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<ErrorHandlingMiddleware>()
                .UseMiddleware<JwtTokenMiddleware>()
                .UseMiddleware<CorrelationIdMiddleware>()
                .UseCors(builder => builder
                    .WithOrigins(Configuration["AllowedHosts"])
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}