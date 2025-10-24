using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace TestProject2
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
              base.ConfigureWebHost(builder);
            // This is the key change to prevent default services from being registered.
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                // Re-add your required services (controllers, etc.) that would normally be in Program.cs
                //services.AddControllersWithViews();

                // Add your in-memory database for testing
                //services.AddDbContext<PersonDbContext>(options =>
                //{
                //    options.UseInMemoryDatabase("DatabaseForTesting");
                //});
            });
        }
    }
}