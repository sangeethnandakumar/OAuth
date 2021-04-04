using AuthServer.Configuration;
using AuthServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Westwind.AspNetCore.LiveReload;

namespace AuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = new OAuthConfig();
            Configuration.Bind("OAuth", config);
            services.AddSingleton<IUserService, UserService>();

            /*
             If "DynamicalyManagedAuth = TRUE" - Then AuthServer will switch to dynamic authentication mode from persistance stores. Else it will fetch static OAuth config from appsettings.json
             Use DynamicalyManagedAuth if you want to administrate clients, api resources and scopes on run
             DynamicalyManagedAuth also allows managing auth server directly from administration dashboard
             */
            var DynamicalyManagedAuth = true;

            if (DynamicalyManagedAuth)
            {
                services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieLifetime = TimeSpan.FromSeconds(config.IdentityServerCookieLifetime);
                })
                .AddDeveloperSigningCredential()
                .AddCorsPolicyService<MyCORSPolicy>()
                .AddResourceStore<MyResourceStore>()
                .AddClientStore<MyClientStore>()
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();
            }
            else
            {
                var clients = config.GetClients();
                var apiResources = config.GetApiResources();
                var identityResources = config.GetIdentityResources();
                var apiScopes = config.GetApiScopes();

                services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieLifetime = TimeSpan.FromSeconds(config.IdentityServerCookieLifetime);
                })
                .AddDeveloperSigningCredential()
                .AddInMemoryApiScopes(apiScopes)
                .AddInMemoryApiResources(apiResources)
                .AddInMemoryIdentityResources(identityResources)
                .AddInMemoryClients(clients)
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential();
            }

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddLiveReload();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLiveReload();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}