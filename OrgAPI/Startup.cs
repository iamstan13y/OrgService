using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrgDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgAPI
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
            //services.AddCors(
            //    x => x.AddPolicy("herPolicy",
            //    p => p.AllowAnyHeader()
            //          .AllowAnyMethod()
            //          .WithOrigins("")
            //          .AllowCredentials()
            //         ));

            services.AddMvc(options => options.EnableEndpointRouting = false);
            //services.AddMvc(config => config.Filters.Add(new OrgExceptionFilter()));
            services.AddMvc(x => x.Filters.Add(new AuthorizeFilter())).AddXmlSerializerFormatters()
                             .AddXmlDataContractSerializerFormatters();

            services.AddDbContext<OrganizationDbContext>();
            services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );
            services.ConfigureApplicationCookie(opt =>
            {
                opt.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<OrganizationDbContext>()
                    .AddDefaultTokenProviders();
            services.AddSwaggerDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseExceptionHandler(
                options =>
                {
                    options.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            await context.Response.WriteAsync(ex.Error.Message);
                        }
                    });
                }
                );
            //app.UseCors("herPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
