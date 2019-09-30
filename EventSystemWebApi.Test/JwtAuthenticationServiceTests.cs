using System;
using System.Linq;
using System.Text;
using Autofac;
using Common.Security;
using Common.Test;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace EventSystemWebApi.Test
{
    public class JwtAuthenticationServiceTests : TestBase
    {
        protected override void BuildUp(ContainerBuilder builder)
        {
            builder.RegisterInstance(new JwtTokenOptions
            {
                Issuer = "ES_AUTHORITY",
                Audience = "ES_AUDIENCE",
                Expiration = TimeSpan.FromHours(5),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("testKey")),
                        SecurityAlgorithms.HmacSha256)
            });

            builder.RegisterType<Mock<ISecurityContext>>().InstancePerLifetimeScope().OnActivated(args =>
            {
                args.Instance.SetupGet(x => x.Claims).Returns(args.Context.Resolve<DbSet<UserClaim>>());
                args.Instance.SetupGet(x => x.Users).Returns(args.Context.Resolve<DbSet<User>>());
            });

            builder.Register(x => x.Resolve<Mock<ISecurityContext>>().Object);
            builder.RegisterType<Mock<DbSet<User>>>().InstancePerLifetimeScope();
            builder.Register(x => x.Resolve<Mock<DbSet<User>>>().Object);
            builder.RegisterType<Mock<DbSet<UserClaim>>>().InstancePerLifetimeScope();
            builder.Register(x => x.Resolve<Mock<DbSet<UserClaim>>>().Object);

            builder.RegisterType<Mock<IEmailService>>().InstancePerLifetimeScope();
            builder.Register(x => x.Resolve<Mock<IEmailService>>().Object);

            builder.RegisterType<JwtAuthenticationService>().InstancePerLifetimeScope();
        }

        [Fact]
        public void CreateNewUser()
        {
            var username = "This is the new username";
            var usr = JwtAuthenticationService.CreateNewUser(username);

            Assert.Equal(username, usr.EMail);
            Assert.True(string.IsNullOrWhiteSpace(usr.Password));
            Assert.True(string.IsNullOrWhiteSpace(usr.ResetHash));
            Assert.NotNull(usr.Claims.FirstOrDefault(x => x.Type == UsernameClaim.USERNAME_CLAIM_TYPE &&
                                                          x.Value == username));
        }

        [Fact]
        public void UpdatePassword()
        {
            var user = new User();
            var pwd = "NewPassword";
            pwd.HashPassword();

            JwtAuthenticationService.UpdatePassword(user, pwd);
            Assert.False(string.IsNullOrWhiteSpace(user.Password));
            Assert.False(string.IsNullOrWhiteSpace(user.Salt));
            Assert.True(string.IsNullOrWhiteSpace(user.ResetHash));
        }

        [Fact]
        public void UpdateUsername()
        {
            const string NEW_USERNAME = "newUsername@google.com";
            var user = new User
            {
                EMail = "dbane@gmx.de"
            };

            user.Claims.Add(new UsernameClaim("dbane@gmx.de"));
            user.Claims.Add(new UsernameClaim("dbane@web.de"));

            JwtAuthenticationService.UpdateUsername(user, NEW_USERNAME);

            Assert.Equal(1, user.Claims.Count);
            Assert.Equal(UsernameClaim.USERNAME_CLAIM_TYPE, user.Claims.FirstOrDefault()?.Type);
            Assert.Equal(NEW_USERNAME, user.Claims.FirstOrDefault()?.Value);
            Assert.Equal(NEW_USERNAME, user.EMail);
        }

        [Fact]
        public void ValidateUser_EmptyPassword()
        {
            Assert.Throws<UnprocessableEntityException>(
                () => JwtAuthenticationService.ValidateUser(new User {EMail = "test"}, ""));
        }

        [Fact]
        public void ValidateUser_NoUser()
        {
            Assert.Throws<UnauthorizedException>(() => JwtAuthenticationService.ValidateUser(null, "bla"));
        }

        [Fact]
        public void ValidateUser_Success()
        {
            var usr = new User();
            var password = "Password";
            var hashTuple = password.HashPassword();

            usr.Password = hashTuple.Item1;
            usr.Salt = hashTuple.Item2;
            usr.EMail = "Username";
            usr.Claims.Add(UserClaim.FromClaim(new UsernameClaim(usr.EMail)));

            JwtAuthenticationService.ValidateUser(usr, password);
        }

        [Fact]
        public void ValidateUser_WrongPassword()
        {
            var usr = new User();
            var password = "Password";
            var hashTuple = password.HashPassword();

            usr.Password = hashTuple.Item1;
            usr.Salt = hashTuple.Item2;
            usr.EMail = "Username";
            usr.Claims.Add(UserClaim.FromClaim(new UsernameClaim(usr.EMail)));

            Assert.Throws<UnauthorizedException>(() => JwtAuthenticationService.ValidateUser(usr, "wrong password"));
        }

        [Fact]
        public void ValidateUsername_NoEmail()
        {
            Assert.Throws<UnprocessableEntityException>(
                () => JwtAuthenticationService.ValidateUsername("This is not a username"));
        }

        [Fact]
        public void ValidateUsername_Success()
        {
            JwtAuthenticationService.ValidateUsername("a@b.com");
        }
    }
}