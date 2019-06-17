using System;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ValidationTest.Models.Config;

namespace ValidationTest
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get config settings from appsettings/user secrets and bind to model
            services.Configure<EmailConfig>(Configuration.GetSection("Emails"));

            // Add Response compression services
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.EnableForHttps = true; // security implications breach/crime ensure CSRF Tokens are used
            });

            // Cache on server based on client cache rules
            services.AddResponseCaching();

            // Setup cookie policy
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });
            
            // Add MVC + setup cache profiles
            services.AddMvc(options => {
                options.CacheProfiles.Add("Default",
                    new CacheProfile()
                    {
                        VaryByHeader = "Accept-Encoding",
                        Location = ResponseCacheLocation.Any,
                        Duration = 60
                    });
                options.CacheProfiles.Add("Never",
                    new CacheProfile()
                    {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Add any application services
            //services.AddSession(); // don't add unless needed
            //services.AddSingleton<ICacheService, CacheService>();
            //services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Add security headers
            app.UseSecurityHeaders(policies =>
                policies
                    .AddDefaultSecurityHeaders()
                    //.AddCustomHeader("X-My-Test-Header", "Header value")
                    .AddContentSecurityPolicy(csp =>
                    {
                        // https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP
                        csp.AddDefaultSrc().Self();
                    }));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // friendly error pages
                app.UseStatusCodePagesWithReExecute("/error/{0}");

                // output caching in production
                app.UseResponseCaching();

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Gzip
            app.UseResponseCompression();
            
            // Force https
            app.UseHttpsRedirection();

            // allows serving of static files    
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    // add cache control header to static resources
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(365),
                        Public = true
                    };
                }
            });

            // Use cookie policy
            app.UseCookiePolicy();

            // Enable sessions (only if necessary)
            //app.UseSession();

            // in order to use attribute routing just use app.UseMvc();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            // Add redirects
            var rewriteOptions = new RewriteOptions()
                .AddRedirect("favicon.ico", "assets/images/favicon.ico");

            app.UseRewriter(rewriteOptions);
        }
    }
}
