using AuthServer.Configuration;
using AuthServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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
            var clients = config.GetClients();
            var apiResources = config.GetApiResources();
            var identityResources = config.GetIdentityResources();
            var apiScopes = config.GetApiScopes();

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<MyCORSPolicy>();

            services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieLifetime = TimeSpan.FromSeconds(config.IdentityServerCookieLifetime);
                })
                .AddDeveloperSigningCredential()
                .AddProfileService<ProfileService>()
                .AddInMemoryApiScopes(apiScopes)
                .AddInMemoryApiResources(apiResources)
                .AddResourceStore<MyResourceStore>()
                //.AddInMemoryClients(clients)
                .AddClientStore<MyClientStore>()
                .AddInMemoryIdentityResources(identityResources)
                .AddDeveloperSigningCredential();
            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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