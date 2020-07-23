using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CertTutorial1
{
    public class Startup
    {
        public Startup(IConfiguration configuration,  IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //add the cert validation service out here
            services.AddSingleton<CertValidationService>();
            services.AddRazorPages();
            //1: Add Microsoft.AspNetCoreAuthentication.Certificate
            //2: configure authentication Serive
            services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate(options =>
        {
            options.Events = new CertificateAuthenticationEvents
            {
                OnCertificateValidated = context =>
                {
                    var validationService = context.HttpContext.RequestServices.GetService<CertValidationService>();

                    if (validationService.ValidateCertificate(context.ClientCertificate, _env))
                    {
                        context.Success();
                    }
                    else
                    {
                        context.Fail("invalid cert");
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    context.Fail("invalid cert");
                    return Task.CompletedTask;
                }
            };
        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //4:Add use cert forwarding
            app.UseCertificateForwarding();
            //5: and that we are using authentication to get into the application
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

    }
}
