public static IServiceProvider SetupEventSystem(this IServiceCollection services,
                                                        IHostingEnvironment env,
                                                        IConfigurationRoot config)
{
    var builder = new ContainerBuilder();

    var options = services.RegisterEventSystemServices(env, config);
    builder.RegisterEventSystemTypes(options,config);

    builder.Populate(services);
    builder.AddControllers();
    return new AutofacServiceProvider(builder.Build());
}
