using AuthServer.Configuration;
using AuthServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
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
            services.Configure<DbConfig>(Configuration.GetSection("ConnectionStrings"));
            
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}