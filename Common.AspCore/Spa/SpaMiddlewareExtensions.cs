using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Common.AspCore.Spa
{
    public static class SpaMiddlewareExtensions
    {
        public static IServiceCollection AddSpaMiddleware(this IServiceCollection services, string pathToIndex)
        {
            services.AddSingleton(new SpaMiddlewareOptions {PathToIndex = pathToIndex});

            return services;
        }

        public static IApplicationBuilder UseSpaMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<SpaMiddleware>();

            return app;
        }
    }
}