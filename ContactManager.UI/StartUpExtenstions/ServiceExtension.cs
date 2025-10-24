using Entities;
using IRepositoryContract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Repository;
using Serilog;
using ServiceContracts;
using Services;

namespace CRUDExample.StartUpExtenstions
{
    public static class ServiceExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,IConfiguration configuration,IHostBuilder host)
        {
            services.AddControllersWithViews();
            services.AddScoped<ICountriesGetterService, CountriesGetterService>();
            services.AddScoped<IPersonsGetterService, PersonGetterService>();
            services.AddScoped<IPersonsAddService, PersonAddService>();
            services.AddScoped<IPersonsDeleteService, PersonDeleteService>();
            services.AddScoped<IPersonsSortService, PersonSortService>();
            services.AddScoped<IPersonUpdateService, PersonUpdateService>();
            services.AddScoped<ICountriesAddService, CountriesAddService>();
            services.AddScoped<ICountriesUploadService, CountriesUploadService>();


            //builder.Host.ConfigureLogging(loggingProvider =>
            //{
            //    loggingProvider.ClearProviders();
            //    loggingProvider.AddDebug();
            //    loggingProvider.AddConsole();
            //});
            host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

                loggerConfiguration
                .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
                .ReadFrom.Services(services); //read out current app's services and make them available to serilog
            });
            ;
            services.AddHttpLogging(options =>
            {
                options.LoggingFields=Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
            });
            services.AddDbContext<PersonDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:Default"],
                    SqlServerOptions =>
                    {
                        // Implement retry logic for transient errors
                        SqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 5, // Number of retries
                            maxRetryDelay: TimeSpan.FromSeconds(30), // Max delay between retries
                            errorNumbersToAdd: null); // Specify specific SQL error numbers if needed
                    });

            });
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            return services;
        }
    }
}
