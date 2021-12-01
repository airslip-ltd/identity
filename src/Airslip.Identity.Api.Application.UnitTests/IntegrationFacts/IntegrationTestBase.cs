using Airslip.Common.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog;
using System;
using System.Net.Http;

namespace Airslip.Identity.Api.Application.UnitTests.IntegrationFacts
{
    public class IntegrationTestBase
    {
        protected readonly Uri BaseUri = new("http://localhost/v1/");
        protected const string HeartbeatEndpoint = "heartbeat";
        internal static readonly Mock<ILogger> _mockLogger = new();
        private static readonly TransactionsApiWebApplicationFactory<Startup> TransactionsFactory = new(_mockLogger.Object);

        static IntegrationTestBase()
        {
            TransactionsFactory.WithWebHostBuilder(builder =>
            {
                builder.UseSerilog(_mockLogger.Object);
            });
        }

        protected static HttpClient GetUnauthorizedHttpClient()
        {
            HttpClient client = TransactionsFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Accept", Json.MediaType);
            return client;
        }
    }
    
    public class TransactionsApiWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly ILogger _logger;

        public TransactionsApiWebApplicationFactory(ILogger logger)
        {
            _logger = logger;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            IHostBuilder builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.ConfigureServices(services =>
                    {
                        services.AddSingleton(_logger);
                    });
                    
                    webHostBuilder.UseStartup(typeof(Startup));
                });

            return builder;
        }
    }
}