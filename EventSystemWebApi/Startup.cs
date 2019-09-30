using System;
using Common.AspCore.Exceptions;
using Common.AspCore.Spa;
using EventSystemWebApi.DependencyInjection;
using EventSystemWebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventSystemWebApi
{
    public class Startup
    {
        #region Properties

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        #endregion

        #region Constructors

        public Startup(IHostingEnvironment env)
        {
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.SetupEventSystem(Environment, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsCustomDevelopment())
                app.UseMySwagger();

            app.UseSpaMiddleware();

            // Add Authentication to the pipeline
            app.UseAuthentication();

            app.UseMvc();

            app.UseStaticFiles();
        }
    }
}