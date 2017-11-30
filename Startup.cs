using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Darren.Security
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("DarrenSecurityScheme")
                .AddCookie("DarrenSecurityScheme", options => {
                    options.AccessDeniedPath = new PathString("/Security/Access");//authorization失败的时候到这里来
                    options.LoginPath = new PathString("/Security/Login");//authentication失败的时候到这里来

                    options.Cookie = new CookieBuilder {
                        HttpOnly = true,//cookie accessed via htttp
                        Name = ".Darren.Security.Cookie",
                        Path = "/",//cookie的存放位置
                        SameSite = SameSiteMode.Lax, 
                        SecurePolicy = CookieSecurePolicy.SameAsRequest,//cookie是否accessed by https
                        Domain = ""//存放cookie的domain
                    };

                    options.Events = new CookieAuthenticationEvents {
                        //刚登陆的时候，用户名从context.Principal.Identity.Name中获得
                        OnSignedIn = context => {
                            Console.WriteLine("{0} - {1}:{2}", DateTime.Now, "OnSignedIn", context.Principal.Identity.Name);
                            return Task.CompletedTask;
                        },

                        //登陆进来以后，用户名从context.HttpContext.User.Identity.Name中获得
                        OnSigningOut = context => {
                            Console.WriteLine("{0} - {1}:{2}", DateTime.Now, "OnSignedOut", context.HttpContext.User.Identity.Name);
                            return Task.CompletedTask;
                        },

                        OnValidatePrincipal = context => {
                            Console.WriteLine("{0} - {1}:{2}", DateTime.Now, "OnValidPrincipal", context.Principal.Identity.Name);
                            return Task.CompletedTask;
                        }
                    };

                    options.ReturnUrlParameter = "ReturnUrl";
                    options.SlidingExpiration = true; // keep the cookie alive once close to expiry time
                    //options.ExpireTimeSpan

                });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
