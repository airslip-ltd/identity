using Airslip.Identity.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
            .UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration))
            .UseStartup<Startup>();
    })
    .Build()
    .Run();