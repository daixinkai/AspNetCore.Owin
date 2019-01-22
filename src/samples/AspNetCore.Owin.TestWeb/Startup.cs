using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AspNetCore.Owin.TestWeb
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

            //            string machineKey = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
            //<machineKey decryptionKey=""AA058CC55FAD77E074B093A59683D81143C6570CD3257536F35351549E29B578"" validation=""HMACSHA256"" validationKey=""E13D8EED8DD0AE3ACCB638D0785A4AEE711804E3BEEFD83A564A64AFA642709FBC086F86D312CD6029368F2CAC4BE83253A0EFE21F1DCB6424B4386CCC91E5E1"" />";
            string machineKey = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
    <machineKey decryptionKey=""B26E215CAA3132BD61AD2934EC5181D28556E8FF73FCF99A"" validation=""SHA1"" validationKey=""C1CA64B83323BF1ED49C665975665ADE1F5875614CA2773DED5435090A5751894FF890F1951249548276A8FAE69E61F3F0BE82EFAD27451AF425840C8AB14AED"" />";
            services.AddMachineKeyDataProtection().WithXml(machineKey);

            services.AddAuthentication("ApplicationCookie").AddOwinCookie("ApplicationCookie", 3, options =>
             {
                 options.Cookie.Name = OwinCookieAuthenticationDefaults.CookiePrefix + "ApplicationCookie";
                 options.LoginPath = new PathString("/account/login");
                 options.ReturnUrlParameter = options.ReturnUrlParameter.ToLower();
                 options.Cookie.HttpOnly = true;
             });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IStartupFilter, TestStartupFilter>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
