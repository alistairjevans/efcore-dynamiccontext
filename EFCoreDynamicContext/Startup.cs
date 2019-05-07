using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicContext;
using EFCoreDynamicContext.BusinessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EFCoreDynamicContext
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });


            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            services.AddRazorPages();

            RegisterDynamicDbContexts(services, (p, opt) => opt.UseSqlServer("connection"));

            services.AddScoped<IUserService, UserService>();
        }

        private void RegisterDynamicDbContexts(IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> opt)
        {
            // Register the DB Context - Register our generic context as well.
            services.TryAdd(new ServiceDescriptor(typeof(DynamicDbContext<>), typeof(DynamicDbContext<>), ServiceLifetime.Scoped));

            //var dbContextOptionsType = typeof(DbContextOptions<>).MakeGenericType(typeof(DynamicDbContext<>));

            //serviceCollection.TryAdd(
            //    new ServiceDescriptor(
            //        dbContextOptionsType,
            //        p => DbContextOptionsFactory<TContextImplementation>(p, optionsAction),
            //        optionsLifetime));

            services.AddSingleton(new DynamicDbContextOptions
            {
                ValidateSetAccess = true
            });

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => 
                    {
                        var builder = new DbContextOptionsBuilder();

                        builder.UseApplicationServiceProvider(p);

                        // Call configuration options.
                        opt(p, builder);

                        return builder.Options;
                    },
                    ServiceLifetime.Scoped));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
