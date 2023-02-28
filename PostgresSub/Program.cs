using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using PostgresSub;

var builder = new HostBuilder()
    .UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureHostConfiguration(builder=> {
        builder.AddCommandLine(args);
    })
    .ConfigureAppConfiguration((ctx, builder) =>
    {
        builder.AddJsonFile("appsettings.json");

        if (ctx.HostingEnvironment.IsDevelopment())
        {
            builder.AddUserSecrets<PostgresqlListener>();
        }
    })
    .ConfigureLogging((ctx, logging) =>
    {
        logging.AddConsole();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddTransient<PostgresqlListener>();
    })
    .UseConsoleLifetime();

var host = builder.Build();

using (var servicesScope = host.Services.CreateScope())
{
    var listener = servicesScope.ServiceProvider.GetRequiredService<PostgresqlListener>();
    await listener.Listen();
}