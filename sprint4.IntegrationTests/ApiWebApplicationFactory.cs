using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using sprint4.Data;

namespace sprint4.IntegrationTests;

internal class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            // Override config to use InMemory for tests!
            var inMemorySettings = new Dictionary<string, string>
            {
                { "UseInMemoryDatabase", "true" }
            };
            configBuilder.AddInMemoryCollection(inMemorySettings);
        });
        
        // builder.ConfigureServices(services =>
        // {
        //     services.RemoveAll(typeof(AppDbContext));
        //     services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
        //     
        //     services.AddDbContext<AppDbContext>(options =>
        //     {
        //         options.UseInMemoryDatabase("InMemoryDbForTesting");
        //     });
        //
        //     var sp = services.BuildServiceProvider();
        //     using (var scope = sp.CreateScope())
        //     {
        //         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //         db.Database.EnsureCreated();
        //     }
        // });
    }
}