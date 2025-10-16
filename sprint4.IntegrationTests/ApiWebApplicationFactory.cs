using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using sprint4.Data;

namespace sprint4.IntegrationTests;

internal class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(AppDbContext));
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            var connectionString = "Data Source=oracle.fiap.com.br:1521/orcl;User ID=rm554513;Password=020905";
            services.AddOracle<AppDbContext>(connectionString);
            
            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = "TestScheme";
            //     options.DefaultChallengeScheme = "TestScheme";
            // })
            // .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
            //     "TestScheme", options => { });

            var db = CreateDbContext(services);
            db.Database.EnsureDeleted();
        });
    }

    private static AppDbContext CreateDbContext(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return db;
    }
}