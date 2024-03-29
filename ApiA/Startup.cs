using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ApiA {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiA", Version = "v1" });
            });
            services.AddAuthentication("Bearer")
           .AddJwtBearer("Bearer", options => {
               options.Authority = "https://localhost:5005";
               options.TokenValidationParameters = new TokenValidationParameters {
                   ValidateAudience = false
               };
           });
            services.AddAuthorization(options => {
                options.AddPolicy("ApiScope", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(JwtClaimTypes.Scope, "Api1");
                });

                //options.AddPolicy("Account", policy => policy.RequireClaim(JwtClaimTypes.Scope, "account"));
                //options.AddPolicy("AccountRead", policy => policy.RequireClaim(JwtClaimTypes.Scope, "account.read"));
                //[Authorize(Policy = "Account")]
            });
            services.AddCors(options => {
                options.AddPolicy("default", policy => {
                    policy.WithOrigins("https://localhost:44334")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiA v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors("default");
            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers().RequireAuthorization("ApiScope");
            });
        }
    }
}