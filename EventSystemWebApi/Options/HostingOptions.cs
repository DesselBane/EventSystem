using Infrastructure.Options;

namespace EventSystemWebApi.Options
{
    public class HostingOptions : IHostingOptions
    {
        public string Url { get; set; }
    }
}