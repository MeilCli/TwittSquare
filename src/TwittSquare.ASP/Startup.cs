using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using TwittSquare.ASP.Model;
using TwittSquare.ASP.Utils;
using TwittSquare.Core.Twitter;
using TwittSquare.Hangfire.Task;

namespace TwittSquare.ASP {
    public class Startup {
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json",optional: true,reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Add framework services.
            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddMvc(config => config.Filters.Add(typeof(TwittSquareAuthorizationFilter)));

            services.AddDbContext<TwitterContext>(x => x.UseNpgsql(Constant.DatabaseConnectionString,b => b.MigrationsAssembly("TwittSquare.ASP.Model")));

            var storage = new PostgreSqlStorage(Configuration.GetConnectionString("DefaultConnection"));
            services.AddHangfire(c => c.UseStorage(storage));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,IHostingEnvironment env,ILoggerFactory loggerFactory) {
            app.UseHangfireDashboard();
            /*app.UseHangfireServer(new BackgroundJobServerOptions() {
                ServerName = "Host"
            });*/

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
