using AuthServer.Configuration;
using AuthServer.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Westwind.AspNetCore.LiveReload;

namespace AuthServer {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            var config = new OAuthConfig();
            Configuration.Bind("OAuth", config);

            services.AddSingleton<IUserService, UserService>();
            services.Configure<DbConfig>(Configuration.GetSection("ConnectionStrings"));

            services.AddIdentityServer(options => {
                //Set cookie lifetime
                options.Authentication.CookieLifetime = TimeSpan.FromSeconds(config.IdentityServerCookieLifetime);
            })
            //For local testing only
            .AddDeveloperSigningCredential()
            //Configure CORS policy
            .AddCorsPolicyService<MyCORSPolicy>()
            //Fetch OAuth v2 resources from SQLServer
            .AddResourceStore<MyResourceStore>()
            //Fetch OAuth v2 clients from SQLServer
            .AddClientStore<MyClientStore>()
            //Fetch user profiles from SQLServer
            .AddProfileService<ProfileService>();

            //Set cookie policy
            services.ConfigureApplicationCookie(options => {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });


            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddLiveReload();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseLiveReload();

            app.UseStaticFiles();

            app.UseRouting();

            //Reverse Proxy Settings
            app.Use(async (ctx, next) =>
            {
                ctx.SetIdentityServerOrigin("https://auth.twileloop.com");
                await next();
            });

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}