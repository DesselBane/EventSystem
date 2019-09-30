using System.Net;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,typeof(string),Description = "An API just see if a webserver is there.")]
        public string Ping() => "Pong";
    }
}
