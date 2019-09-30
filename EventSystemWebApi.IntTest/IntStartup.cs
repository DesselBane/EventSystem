using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventSystemWebApi.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventSystemWebApi.IntTest
{
    public class IntStartup : Startup
    {
        #region Constructors

        public IntStartup(IHostingEnvironment env)
            : base(env)
        {
        }

        #endregion

        #region Overrides of Startup

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            var options = services.RegisterEventSystemServices(Environment, Configuration);
            options.Databasename += Guid.NewGuid();
            services.AddSingleton(options.BuildOptions());

            builder.RegisterEventSystemTypes(options,Configuration);


            builder.Populate(services);
            builder.AddControllers();
            return new AutofacServiceProvider(builder.Build());
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var loggerFactory2 = new LoggerFactory();
            base.Configure(app, env, loggerFactory2);
        }

        #endregion
    }
}