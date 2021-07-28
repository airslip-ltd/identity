using Airslip.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Auth
{
    public class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtSettings _settings;
        private readonly SigningCredentials _signingCredentials;

        public ConfigureJwtBearerOptions(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
                SecurityAlgorithms.HmacSha256);
        }

        public void Configure(JwtBearerOptions options)
        {
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Add("Token-Expired", "true");
                    else
                        Log.Error(context.Exception, "Authentication failed");

                    return Task.CompletedTask;
                }
            };
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                IssuerSigningKey = _signingCredentials.Key,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
        }

        public void Configure(string name, JwtBearerOptions options)
        {
            Configure(options);
        }
    }
}