using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DiaggregatorApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDispatcher();
            services.AddDiaggregator();
            services.AddMvc();

            services.AddMvc(options => options.Conventions.Add(new TurnOffMvcAuthorizationSupport()));

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login/");
                    options.AccessDeniedPath = new PathString("/Account/Forbidden/");
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Members", p => p.RequireAuthenticatedUser());
                options.AddPolicy("Admins", p => p.RequireRole("admin"));

                options.DefaultPolicy = options.GetPolicy("Members");
            });
            services.AddAuthorizationPolicyEvaluator();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDispatcher();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseGraphiQl("graphiql");
        }
    }
}
