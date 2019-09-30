using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EventSystemWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseConfiguration(config)
                .Build();

            host.Run();
        }
    }
}