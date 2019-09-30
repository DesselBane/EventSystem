private static ContainerBuilder AddPeople(this ContainerBuilder builder)
{
    builder.RegisterType<PersonService>()
           .As<IPersonService>()
           .EnableInterfaceInterceptors()
           .InterceptedBy(typeof(PersonServiceInterceptor));

    builder.RegisterType<PersonServiceInterceptor>();
    builder.RegisterType<PersonValidator>();

    return builder;
}
