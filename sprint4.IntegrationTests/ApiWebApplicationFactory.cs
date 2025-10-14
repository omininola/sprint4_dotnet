using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using project.Data;

namespace sprint4.IntegrationTests;

internal class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>(); 
                services.RemoveAll<AppDbContext>();

                var connectionString = "Data Source=oracle.fiap.com.br:1521/orcl;User ID=rm554513;Password=020905";
                
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseOracle(connectionString);
                });
    
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                    db.SaveChanges();
                }
            });
        }
}