using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Common.AspCore.Spa
{
    public class SpaMiddleware
    {
        #region Vars

        private readonly string _indexPath;
        private readonly RequestDelegate _next;

        #endregion

        #region Constructors

        public SpaMiddleware(RequestDelegate next, SpaMiddlewareOptions options)
        {
            _next = next;

            _indexPath = options.PathToIndex;
        }

        #endregion

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api") &&
                !context.Request.Path.StartsWithSegments("/swagger") && !Path.HasExtension(context.Request.Path.Value))
            {
                context.Response.StatusCode = (int) HttpStatusCode.OK;
                await context.Response.WriteAsync(File.ReadAllText(_indexPath));
            }
            else
            {
                await _next(context);
            }
        }
    }
}