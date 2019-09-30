using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Common.AspCore.Interception;
using Common.AspCore.Spa;
using Common.Events;
using Common.Events.Attendees;
using Common.Events.Locations;
using Common.Events.ServiceAgreements;
using Common.Events.ServiceSlots;
using Common.EventService;
using Common.EventServiceModels;
using Common.EventServiceModels.Locations;
using Common.EventServiceModels.ServiceAttributes;
using Common.Mail;
using Common.People;
using Common.Permissions;
using Common.Security;
using Common.ServiceTypes;
using Common.ServiceTypes.ServiceAttributes;
using Common.Users;
using EventSystemWebApi.Controllers;
using EventSystemWebApi.Options;
using Infrastructure.DataModel;
using Infrastructure.Email;
using Infrastructure.Exceptions;
using Infrastructure.Options;
using Infrastructure.Services;
using Infrastructure.Services.Events;
using Infrastructure.Services.EventServiceModels;
using Infrastructure.Services.EventServiceModels.ServiceAttributes;
using Infrastructure.Services.Permissions;
using Infrastructure.Services.ServiceTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MsSqlContext;
using MySqlContext;
using Newtonsoft.Json;

namespace EventSystemWebApi.DependencyInjection
{
    public static class ServiceExtensions
    {
        #region Const

        // The secret key every token will be signed with.
        // Keep this safe on the server!
        private static readonly string SecretKey = "mysupersecret_secretkey!123";

        #endregion

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

        public static DatabaseOptions RegisterEventSystemServices(this IServiceCollection services,
                                                                  IHostingEnvironment env,
                                                                  IConfigurationRoot config)
        {
            //Creating Global Authorization Filter
            var globalAuthFilter = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Determining Index Path
            var index = new FileInfo(env.WebRootPath + "/index.html");
            if (!index.Exists)
            {
                index = new FileInfo(env.WebRootPath + "/Index.html");
                if (!env.IsDevelopment() && !index.Exists)
                    throw new FileNotFoundException($"Could not find Index.html in WebRootPath: {env.WebRootPath}");
            }

            // Register Spa Classes
            services.AddSpaMiddleware(index.FullName)
                    // Registers the IOption Classes of ASP Core
                    .AddOptions()
                    //Adding MVC Classes
                    .AddMvc(options => { options.Filters.Add(new AuthorizeFilter(globalAuthFilter)); })
                    .AddControllersAsServices()
                    .AddJsonOptions(options => { options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc; });

            var dboptions = services.AddDatabaseOptions(config);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

            var tokenValidationParams = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ES_AUTHORITY",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ES_AUDIENCE",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParams; });

            services.AddSingleton(tokenValidationParams);

            return dboptions;
        }

        public static ContainerBuilder RegisterEventSystemTypes(this ContainerBuilder builder, DatabaseOptions options, IConfigurationRoot configurationRoot)
        {
            builder
                .AddAuth()
                .AddDatabase(options)
                .AddPeople()
                .AddEvents()
                .AddProvider()
                .AddLocation()
                .AddPermissions()
                .AddUsers()
                .AddLocation()
                .AddAttributeSpecifications()
                .AddServiceAttributes()
                .AddAgreements()
                .AddEMail(configurationRoot)
                .AddHostingOptions(configurationRoot);

            return builder;
        }

        private static ContainerBuilder AddAuth(this ContainerBuilder builder)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

            builder.RegisterType<JwtAuthenticationService>()
                   .As<IAuthenticationService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(AuthenticationServiceInterceptor));

            builder.RegisterType<AuthenticationServiceInterceptor>();
            builder.RegisterType<AuthenticationValidator>();

            builder.RegisterInstance(new JwtTokenOptions
            {
                Issuer = "ES_AUTHORITY",
                Audience = "ES_AUDIENCE",
                Expiration = TimeSpan.FromHours(1),
                RefreshTokenExpiration = TimeSpan.FromDays(30),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            }).SingleInstance();

            //Injecting Current ClaimsIdentity
            builder.RegisterType<HttpContextAccessor>()
                   .As<IHttpContextAccessor>()
                   .SingleInstance();

            builder.Register(x => x.Resolve<IHttpContextAccessor>().HttpContext.User.Identity as ClaimsIdentity ??
                                  new ClaimsIdentity());

            return builder;
        }

        private static DatabaseOptions AddDatabaseOptions(this IServiceCollection services, IConfigurationRoot config)
        {
            var dbOptions = config.GetSection("database").Get<DatabaseOptions>();

            if (dbOptions == null)
                throw new MissingConfigurationException("Database Configuration not found");
            
            services.AddSingleton(dbOptions);
            services.AddSingleton(dbOptions.BuildOptions());
            return dbOptions;
        }

        private static ContainerBuilder AddDatabase(this ContainerBuilder builder, DatabaseOptions options)
        {
            switch (options.Provider)
            {
                case DatabaseProviders.MySql:
                    builder.RegisterType<MySqlDataContext>()
                           .As<DataContext>();
                    break;
                case DatabaseProviders.MsSql:
                    builder.RegisterType<MsSqlDataContext>()
                           .As<DataContext>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return builder;
        }

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

        private static ContainerBuilder AddEvents(this ContainerBuilder builder)
        {
            builder.RegisterType<EventService>()
                   .As<IEventService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(EventServiceInterceptor));

            builder.RegisterType<EventServiceInterceptor>();
            builder.RegisterType<EventValidator>();

            builder.RegisterType<AttendeeService>()
                   .As<IAttendeeService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(AttendeeServiceInterceptor));

            builder.RegisterType<AttendeeServiceInterceptor>();
            builder.RegisterType<AttendeeValidator>();

            builder.RegisterType<ServiceSlotService>()
                   .As<IServiceSlotService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(ServiceSlotServiceInterceptor));

            builder.RegisterType<ServiceSlotServiceInterceptor>();
            builder.RegisterType<ServiceSlotValidator>();

            builder.RegisterType<EventLocationService>()
                   .As<IEventLocationService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(EventLocationServiceInterceptor));

            builder.RegisterType<EventLocationServiceInterceptor>();

            return builder;
        }

        private static ContainerBuilder AddProvider(this ContainerBuilder builder)
        {
            builder.RegisterType<EventServiceModelService>()
                   .As<IEventServiceModelService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(EventServiceModelInterceptor));

            builder.RegisterType<EventServiceModelInterceptor>();


            builder.RegisterType<ServiceTypesService>()
                   .As<IServiceTypeService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(ServiceTypeServiceInterceptor));

            builder.RegisterType<ServiceTypeServiceInterceptor>();

            builder.RegisterType<ServiceLocationService>()
                   .As<IServiceLocationService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(ServiceLocationServiceInterceptor));

            builder.RegisterType<ServiceLocationServiceInterceptor>();

            builder.RegisterType<EventServiceModelValidator>();
            builder.RegisterType<ServiceTypeValidator>();


            return builder;
        }

        public static ContainerBuilder AddControllers(this ContainerBuilder builder)
        {
            builder.RegisterType<ControllerInterceptor>();

            builder.RegisterType<AuthController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            builder.RegisterType<EventController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            builder.RegisterType<PersonController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            builder.RegisterType<ServiceController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            builder.RegisterType<ServiceTypesController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            builder.RegisterType<PermissionController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            builder.RegisterType<UserController>()
                   .EnableClassInterceptors()
                   .InterceptedBy(typeof(ControllerInterceptor));

            return builder;
        }

        private static ContainerBuilder AddLocation(this ContainerBuilder builder)
        {
            return builder;
        }

        private static ContainerBuilder AddAttributeSpecifications(this ContainerBuilder builder)
        {
            builder.RegisterType<AttributeSpecificationService>()
                .As<IAttributeSpecificationService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(AttributeSpecificationServiceInterceptor));

            builder.RegisterType<AttributeSpecificationServiceInterceptor>();
            builder.RegisterType<AttributeSpecificationValidator>();

            return builder;
        }

        private static ContainerBuilder AddServiceAttributes(this ContainerBuilder builder)
        {
            builder.RegisterType<ServiceAttributeService>()
                   .As<IServiceAttributeService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(ServiceAttributeServiceInterceptor));

            builder.RegisterType<ServiceAttributeServiceInterceptor>();
            builder.RegisterType<ServiceAttributeValidator>();

            return builder;
        }

        private static ContainerBuilder AddPermissions(this ContainerBuilder builder)
        {
            builder.RegisterType<PermissionService>()
                   .As<IPermissionService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(PermissionServiceInterceptor));

            builder.RegisterType<PermissionServiceInterceptor>();
            builder.RegisterType<PermissionValidator>();

            return builder;
        }

        private static ContainerBuilder AddUsers(this ContainerBuilder builder)
        {
            builder.RegisterType<UserService>()
                   .As<IUserService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(UserServiceInterceptor));

            builder.RegisterType<UserServiceInterceptor>();
            builder.RegisterType<UserValidator>();

            return builder;
        }

        private static ContainerBuilder AddAgreements(this ContainerBuilder builder)
        {
            builder.RegisterType<AgreementService>()
                   .As<IAgreementService>()
                   .EnableInterfaceInterceptors()
                   .InterceptedBy(typeof(AgreementServiceInterceptor));

            builder.RegisterType<AgreementServiceInterceptor>();
            builder.RegisterType<AgreementValidator>();

            return builder;
        }

        private static ContainerBuilder AddEMail(this ContainerBuilder builder, IConfigurationRoot configurationRoot)
        {
            var mailOptions = configurationRoot.GetSection("eMail").Get<EmailServiceConfiguration>();
            mailOptions.ThrowIfMisconfigured();

            if (mailOptions.UseDummyService)
            {
                builder.RegisterType<DummyMailService>()
                       .As<IEmailService>()
                       .SingleInstance();
            }
            else
            {
                builder.RegisterType<MailKitEmailService>()
                       .As<IEmailService>()
                       .SingleInstance();
            }

            builder.RegisterInstance(mailOptions)
                   .As<IEmailServiceConfiguration>();
            
            

            return builder;
        }

        private static ContainerBuilder AddHostingOptions(this ContainerBuilder builder, IConfigurationRoot configurationRoot)
        {
            var hostingOptions = configurationRoot.GetSection("hosting").Get<HostingOptions>();

            builder.RegisterInstance(hostingOptions)
                   .As<IHostingOptions>();
            
            return builder;
        }
    }
    
   
}
