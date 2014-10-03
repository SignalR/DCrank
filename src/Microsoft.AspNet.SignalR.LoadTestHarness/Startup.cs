using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.SignalR.DCrank.PerfCounterHarness;
using Microsoft.AspNet.SignalR.LoadTestHarness.Models;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.SignalR.LoadTestHarness
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            // Setup configuration sources
            var configuration = new Framework.ConfigurationModel.Configuration();
            configuration.AddJsonFile("config.json");
            configuration.AddEnvironmentVariables();
            var connectionString = configuration.Get("Data:DefaultConnection:ConnectionString");

            // Set up application services
            app.UseServices(services =>
            {
                // Add EF services to the services container
                services.AddEntityFramework()
                    .AddSqlServer();

                // Configure DbContext
                services.SetupOptions<DbContextOptions>(options =>
                {
                    options.UseSqlServer(configuration.Get("Data:DefaultConnection:ConnectionString"));
                });

                // Add Identity services to the services container
                services.AddDefaultIdentity<ApplicationDbContext, ApplicationUser, IdentityRole>(configuration);

                // Add MVC services to the services container
                services.AddMvc();

                // Add SignalR services to the services container
                services.AddSignalR().SetupOptions(options =>
                {
                    options.Transports.DisconnectTimeout = TimeSpan.FromSeconds(60);
                });

                services.AddDCrankHarness(connectionString);

                services.AddSingleton<TimerManager>();
            });

            // Enable Browser Link support
            //app.UseBrowserLink();

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add SignalR to the request pipeline
            app.UseSignalR<TestConnection>("/TestConnection");
            app.UseSignalR();

            app.UseDCrankEndpoint();

            // Add cookie-based authentication to the request pipeline
            app.UseIdentity();

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "api",
                    template: "{controller}/{id?}");
            });
        }
    }
}
