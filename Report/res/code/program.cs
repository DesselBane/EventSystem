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
