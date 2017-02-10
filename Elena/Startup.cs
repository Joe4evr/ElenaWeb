﻿using Boilerplate.AspNetCore;
using Boilerplate.AspNetCore.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Elena.Models;
using Elena.Settings;

namespace Elena
{
    /// <summary>
    /// The main start-up class for the application.
    /// </summary>
    public partial class Startup
    {
        #region Fields

        /// <summary>
        /// Gets or sets the application configuration, where key value pair settings are stored. See
        /// http://docs.asp.net/en/latest/fundamentals/configuration.html
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The environment the application is running under. This can be Development, Staging or Production by default.
        /// To set the hosting environment on Windows:
        /// 1. On your server, right click 'Computer' or 'My Computer' and click on 'Properties'.
        /// 2. Go to 'Advanced System Settings'.
        /// 3. Click on 'Environment Variables' in the Advanced tab.
        /// 4. Add a new System Variable with the name 'ASPNETCORE_ENVIRONMENT' and value of Production, Staging or
        /// whatever you want. See http://docs.asp.net/en/latest/fundamentals/environments.html
        /// </summary>
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Gets or sets the port to use for HTTPS. Only used in the development environment.
        /// </summary>
        private readonly int? sslPort;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="hostingEnvironment">The environment the application is running under. This can be Development,
        /// Staging or Production by default.</param>
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                // Add configuration from the config.json file.
                .AddJsonFile("config.json")
                // Add configuration from an optional config.development.json, config.staging.json or
                // config.production.json file, depending on the environment. These settings override the ones in the
                // config.json file.
                .AddJsonFile($"config.{hostingEnvironment.EnvironmentName}.json", optional: true)
                // This reads the configuration keys from the secret store. This allows you to store connection strings
                // and other sensitive settings, so you don't have to check them into your source control provider.
                // Only use this in Development, it is not intended for Production use. See
                // http://docs.asp.net/en/latest/security/app-secrets.html
                .AddIf(hostingEnvironment.IsDevelopment(), x => x.AddUserSecrets())
                // Add configuration specific to the Development, Staging or Production environments. This config can
                // be stored on the machine being deployed to or if you are using Azure, in the cloud. These settings
                // override the ones in all of the above config files.
                // Note: To set environment variables for debugging navigate to:
                // Project Properties -> Debug Tab -> Environment Variables
                // Note: To get environment variables for the machine use the following command in PowerShell:
                // [System.Environment]::GetEnvironmentVariable("[VARIABLE_NAME]", [System.EnvironmentVariableTarget]::Machine)
                // Note: To set environment variables for the machine use the following command in PowerShell:
                // [System.Environment]::SetEnvironmentVariable("[VARIABLE_NAME]", "[VARIABLE_VALUE]", [System.EnvironmentVariableTarget]::Machine)
                // Note: Environment variables use a colon separator e.g. You can override the site title by creating a
                // variable named AppSettings:SiteTitle. See http://docs.asp.net/en/latest/security/app-secrets.html
                .AddEnvironmentVariables()
                .Build();

            if (hostingEnvironment.IsDevelopment())
            {
                var launchConfiguration = new ConfigurationBuilder()
                    .SetBasePath(hostingEnvironment.ContentRootPath)
                    .AddJsonFile(@"Properties\launchSettings.json")
                    .Build();
                sslPort = launchConfiguration.GetValue<int>("iisSettings:iisExpress:sslPort");
            }

            this.hostingEnvironment = hostingEnvironment;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures the services to add to the ASP.NET MVC 6 Injection of Control (IoC) container. This method gets
        /// called by the ASP.NET runtime. See
        /// http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx
        /// </summary>
        /// <param name="services">The services collection or IoC container.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgerySecurely()
                .AddCaching()
                .AddOptions(this.configuration)
                .AddRouting(options =>
                    {
                        // Improve SEO by stopping duplicate URL's due to case differences or trailing slashes.
                        // See http://googlewebmastercentral.blogspot.co.uk/2010/04/to-slash-or-not-to-slash.html
                        // All generated URL's should append a trailing slash.
                        options.AppendTrailingSlash = true;
                        // All generated URL's should be lower-case.
                        options.LowercaseUrls = true;
                    })
                // Add useful interface for accessing the ActionContext outside a controller.
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                // Add useful interface for accessing the HttpContext outside a controller.
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                // Add useful interface for accessing the IUrlHelper outside a controller.
                .AddScoped(x => x.GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext))
                // Adds a filter which help improve search engine optimization (SEO).
                .AddSingleton<RedirectToCanonicalUrlAttribute>()
                // Add many MVC services to the services container.
                .AddMvc(options =>
                    {
                        // Controls how controller actions cache content from the config.json file.
                        var cacheProfileSettings = this.configuration.GetSection<CacheProfileSettings>();
                        foreach (var keyValuePair in cacheProfileSettings.CacheProfiles)
                        {
                            options.CacheProfiles.Add(keyValuePair);
                        }

                        // Require HTTPS to be used across the whole site. Also set a custom port to use for SSL in
                        // Development. The port number to use is taken from the launchSettings.json file which Visual
                        // Studio uses to start the application.
                        options.Filters.Add(new RequireHttpsAttribute());
                        options.SslPort = sslPort;

                        // Adds a filter which help improve search engine optimization (SEO).
                        options.Filters.AddService(typeof(RedirectToCanonicalUrlAttribute));
                    })
                // Configures the JSON output formatter to use camel case property names like 'propertyName' instead of
                // pascal case 'PropertyName' as this is the more common JavaScript/JSON style.
                .AddJsonOptions(x => x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .Services
                .AddCustomServices(configuration);
        }

        /// <summary>
        /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
        /// called by the ASP.NET runtime.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="loggerfactory">The logger factory.</param>
        public void Configure(IApplicationBuilder application, ILoggerFactory loggerfactory)
        {
            // Configure application logging. See http://docs.asp.net/en/latest/fundamentals/logging.html
            loggerfactory
                // Log to Serilog (A great logging framework). See https://github.com/serilog/serilog-framework-logging.
                // Add the Serilog package to project.json before uncommenting the line below.
                // .AddSerilog()
                .AddIf(hostingEnvironment.IsDevelopment(), x => x.AddConsole(configuration.GetSection("Logging"))
                        .AddDebug());

            application
                // Removes the Server HTTP header from the HTTP response for marginally better security and performance.
                .UseNoServerHttpHeader()
                // Add static files to the request pipeline e.g. hello.html or world.css.
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseIfElse(hostingEnvironment.IsDevelopment(), x => x.UseDebugging()
                        .UseDeveloperErrorPages(), x => x.UseErrorPages())
                .UseStrictTransportSecurityHttpHeader()
                .UsePublicKeyPinsHttpHeader()
                .UseContentSecurityPolicyHttpHeader(sslPort, hostingEnvironment)
                .UseSecurityHttpHeaders()
                .UseLocalizationOptions()
                // Add MVC to the request pipeline.
                .UseMvc();
#if DEBUG
            using (var ctx = (ElenaDbContext)application.ApplicationServices.GetService(typeof(ElenaDbContext)))
            {
                ctx.Database.EnsureCreated();
            }
#endif
        }

        #endregion
    }
}