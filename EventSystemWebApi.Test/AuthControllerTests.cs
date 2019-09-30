using System.Collections.Generic;
using System.Security.Claims;
using Autofac;
using Common.Test;
using EventSystemWebApi.Controllers;
using Infrastructure.DataContracts;
using Infrastructure.Services;
using Moq;
using Xunit;

namespace EventSystemWebApi.Test
{
    public class AuthControllerTests : TestBase
    {
        protected override void BuildUp(ContainerBuilder builder)
        {
            builder.Register(x => new Mock<IAuthenticationService>()).InstancePerLifetimeScope();
            builder.Register(x => x.Resolve<Mock<IAuthenticationService>>().Object);
            builder.RegisterType<AuthController>().InstancePerLifetimeScope();
        }

        [Fact]
        public void Login_NotAuthenticated()
        {
            StartLifetime();

            var login = new LoginDTO {Username = "user", Password = "Password"};
            var ident = new ClaimsIdentity();
            var authString = "This is an auth string";
            var controller = CurrentLifetime.Resolve<AuthController>();
            var authMock = CurrentLifetime.Resolve<Mock<IAuthenticationService>>();
            authMock.Setup(x => x.AuthenticateAsync(login.Username, login.Password)).ReturnsAsync(ident);
            authMock.Setup(x => x.GenerateTokenAsync(ident)).ReturnsAsync(authString);

            Assert.NotEqual(authString, controller.Login(login).Result);
        }

        [Fact]
        public void LoginSuccess()
        {
            StartLifetime();

            var login = new LoginDTO {Username = "user", Password = "Password"};
            var ident = new ClaimsIdentity(new List<Claim>(), "MyAuthenticationType");
            var authString = "This is an auth string";
            var controller = CurrentLifetime.Resolve<AuthController>();
            var authMock = CurrentLifetime.Resolve<Mock<IAuthenticationService>>();
            authMock.Setup(x => x.AuthenticateAsync(login.Username, login.Password)).ReturnsAsync(ident);
            authMock.Setup(x => x.GenerateTokenAsync(ident)).ReturnsAsync(authString);

            Assert.Equal(authString, controller.Login(login).Result);
        }
    }
}