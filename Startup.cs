using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Darren.Security.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Darren.Security
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("DarrenSecurityScheme")
                .AddCookie("DarrenSecurityScheme", options => {
                    options.AccessDeniedPath = new PathString("/Security/Access");
                    options.LoginPath = new PathString("/Security/Login");
                });

            services.AddAuthorization(options => {
                options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());

                options.AddPolicy("Member", policy => policy.RequireClaim("MembershipId"));

                options.AddPolicy("PaidMember", policy => policy.RequireClaim("HasCreditCard", "Y"));

                options.AddPolicy("Over18", policy => policy.Requirements.Add(new AgeRequirement(18)));

                options.AddPolicy("CanRentNewRelease", policy => policy.Requirements.Add(new RentNewReleaseRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, AgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, RentNewReleaseRequirementHandler>();

            services.AddMvc(options => {
                options.Filters.Add(new AuthorizeFilter("Authenticated"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
