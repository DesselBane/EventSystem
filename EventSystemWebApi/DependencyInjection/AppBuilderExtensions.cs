using System.Reflection;
using Microsoft.AspNetCore.Builder;
using NSwag.AspNetCore;

namespace EventSystemWebApi.DependencyInjection
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseMySwagger(this IApplicationBuilder app)
        {
            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, new SwaggerUiSettings());

            return app;
        }
    }
}