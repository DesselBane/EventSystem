using System;
using Infrastructure.Options;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataModel
{
    public abstract class DataContextFactory
    {
        public static DatabaseOptions GetConfiguration(string predefinedSettings = "UNDEFINED")
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{envName}.json", true)
                .AddJsonFile($"appsettings.{envName}.{predefinedSettings}.json", true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var customOptions = config.GetSection("database").Get<DatabaseOptions>();

            Console.WriteLine($"Environment: {envName}");
            Console.WriteLine($"DbOptions: {customOptions}");
            return customOptions;
        }
    }
}