using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockApi.Tests.Helpers;
using Xunit.Abstractions;

namespace StockApi.Tests.IntegrationTests;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public ITestOutputHelper? TestOutputHelper { get; set; }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                if (TestOutputHelper is not null)
                {
                    loggingBuilder.AddProvider(new TestOutputLoggerProvider(TestOutputHelper));
                }
            });

        });
    }
}
