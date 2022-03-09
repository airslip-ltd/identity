using Airslip.Common.AppIdentifiers;
using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Middleware;
using Airslip.Common.Repository.Extensions;
using Airslip.Common.Services.AutoMapper.Extensions;
using Airslip.Common.Types.Configuration;
using Airslip.Email.Client;
using Airslip.Identity.Api.Application;
using Airslip.Identity.Api.Application.Implementations;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Application.Validators;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Infrastructure.BlobStorage;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
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
using System;
using System.IO;
using System.Reflection;
using Serilog;
using Airslip.Common.Auth.AspNetCore.Extensions;
using Airslip.Common.Auth.AspNetCore.Middleware;
using Airslip.Common.Auth.Enums;
using Airslip.Common.Monitoring;
using Airslip.Common.Monitoring.Implementations.Checks;
using Airslip.Common.Repository.Implementations;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Identity.Services.MongoDb;

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
                .Configure<PublicApiSettings>(Configuration.GetSection(nameof(PublicApiSettings)))
                .Configure<EmailConfigurationSettings>(Configuration.GetSection(nameof(EmailConfigurationSettings)))
                .Configure<WelcomeSettings>(Configuration.GetSection(nameof(WelcomeSettings)))
                .Configure<EnvironmentSettings>(Configuration.GetSection(nameof(EnvironmentSettings)))
                .Configure<ApiKeyValidationSettings>(Configuration.GetSection(nameof(ApiKeyValidationSettings)));

            services
                .AddScoped<IUserLifecycle, UserLifecycle>()
                .AddScoped<IUserService, UserService>();

            services
                .AddSendGrid(Configuration)
                .AddAppIdentifiers(Configuration);

            services
                .AddScoped<ITokenDecodeService<QrCodeToken>, TokenDecodeService<QrCodeToken>>()
                .AddScoped<ITokenGenerationService<GenerateApiKeyToken>, TokenGenerationService<GenerateApiKeyToken>>()
                .AddScoped<ITokenGenerationService<GenerateQrCodeToken>, TokenGenerationService<GenerateQrCodeToken>>()
                .AddScoped<ITokenGenerationService<GenerateUserToken>, TokenGenerationService<GenerateUserToken>>()
                .AddAirslipJwtAuth(Configuration, AuthType.All)?
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
                    options.Password.RequireNonAlphanumeric = false;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddRoles<ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
                    Configuration["MongoDbSettings:ConnectionString"],
                    Configuration["MongoDbSettings:DatabaseName"])
                .AddDefaultTokenProviders();

            services.AddBlobStorage(Configuration);

            services.AddMediatR(ApplicationAssembly.Reference);
            // For all the validators, register them with dependency injection as scoped
            FluentValidation.AssemblyScanner.FindValidatorsInAssembly(ApplicationAssembly.Reference)
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
            
            // Add repository content
            services
                .AddRepositories();
            
            // Customised per app
            services.AddScoped<IModelValidator<ApiKeyModel>, ApiKeyModelValidator>();
            services.AddScoped<IModelValidator<QrCodeModel>, QrCodeModelValidator>();
            services.AddScoped<IModelValidator<UserModel>, UserModelValidator>();
            services.AddScoped<IModelValidator<UserRoleUpdateModel>, UserRoleUpdateModelValidator>();
            
            services
                .AddScoped<ITokenValidator<QrCodeToken>, TokenValidator<QrCodeToken>>();
            
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<ApiKey, ApiKeyModel>();
                cfg.CreateMap<ApiKeyModel, ApiKey>();
                cfg.CreateMap<CreateApiKeyModel, ApiKeyModel>();
                cfg.CreateMap<QrCode, QrCodeModel>();
                cfg.CreateMap<QrCodeModel, QrCode>();
                cfg.CreateMap<CreateQrCodeModel, QrCodeModel>();
                cfg.CreateMap<DataConsentModel, DataConsent>();
                cfg.CreateMap<CreateUnregisteredUserModel, UserModel>();
                cfg.CreateMap<UserRoleUpdateModel, User>()
                    .ReverseMap();
                cfg.IgnoreUnmapped<UserRoleUpdateModel, User>();
                cfg.IgnoreUnmapped<User, UserRoleUpdateModel>();
                cfg
                    .CreateMap<UserModel, User>()
                    .ForMember(o => o.EntityId, 
                        opt => opt.Ignore())
                    .ForMember(o => o.AirslipUserType, 
                        opt => opt.Ignore())
                    .ForMember(o => o.RefreshTokens, 
                        opt => opt.Ignore())
                    .ForMember(o => o.UserRole, 
                        opt => opt.Ignore())
                    .ReverseMap()
                    .ForMember(o => o.RefreshTokens, 
                        opt => opt.Ignore())
                    .ForMember(o => o.OpenBankingProviders, 
                        opt => opt.Ignore());
            });

            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
            services.AddScoped<IDataConsentService, DataConsentService>();
            services.AddScoped<IUnregisteredUserService, UnregisteredUserService>();
            
            services.AddScoped(typeof(IEntitySearch<>), typeof(EntitySearch<>) );

            services
                .AddApiVersioning(options => { options.ReportApiVersions = true; })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services
                .AddMongoServices(Configuration);

            services
                .UseHealthChecks()
                .AddHealthCheck<MongoDbCheck>();
            
            services
                .AddApiAccessValidation(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider,  ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            // Set environments for tokens
            IOptions<EnvironmentSettings>? environment = serviceProvider.GetService<IOptions<EnvironmentSettings>>();
            AirslipSchemeOptions.ThisEnvironment = environment?.Value.EnvironmentName ?? "Development";

            // Initialise roles
            serviceProvider.InitialiseRoles().GetAwaiter().GetResult();
            
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
                .UseCors(builder => builder
                    .WithOrigins(Configuration["AllowedHosts"].Split(";"))
                    .AllowAnyHeader()
                    .AllowAnyMethod())
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}