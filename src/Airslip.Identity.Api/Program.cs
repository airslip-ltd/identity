using Airslip.Common.Types;
using Airslip.Identity.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
            .UseStartup<Startup>();
    })
    .UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration)
        .Enrich.WithCorrelationIdHeader(ApiConstants.CorrelationIdName))
    .Build()
    .Run();