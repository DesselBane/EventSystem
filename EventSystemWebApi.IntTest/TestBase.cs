// Needed for Extension method GetService<>
// ReSharper disable once RedundantUsingDirective

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.Security;
using Microsoft.Extensions.DependencyInjection;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.DataContracts;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.DataModel.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EventSystemWebApi.IntTest
{
    public class TestBase : IDisposable
    {
        #region Const

        protected const string ALL_TIME_PASSWORD = "superSecretBla123";

        #endregion

        #region Vars

        protected readonly HttpClient _Client;
        protected readonly DataContext _Context;
        protected readonly TestServer _Server;

        #endregion

        #region Constructors

        public TestBase()
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT_TEST");

            if (string.IsNullOrWhiteSpace(envName))
                envName = "integrationTest";

            _Server = new TestServer(new WebHostBuilder()
                .UseEnvironment(envName)
                .UseWebRoot("../../../../EventSystemWebApi/wwwroot")
                .UseStartup<IntStartup>());

            _Client = _Server.CreateClient();

            _Context = CreateDataContext();
            _Context.Database.Migrate();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _Context.Database.EnsureDeleted();
        }

        #endregion

        #region Helper

        protected async Task<User> CreateUserAsync()
        {
            var user = CreateUser();

            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            return user;
        }
        
        protected User CreateUser()
        {
            var username = Guid.NewGuid() + "@gmx.de";
            var password = ALL_TIME_PASSWORD;
            var passwordHash = password.HashPassword();
            var user = new User
            {
                EMail = username,
                Password = passwordHash.Item1,
                Salt = passwordHash.Item2,
                Person = new RealPerson
                {
                    Firstname = Guid.NewGuid().ToString(),
                    Lastname = Guid.NewGuid().ToString()
                }
            };

            user.Claims.Add(new UsernameClaim(username));
            return user;
        }

        protected async Task SetupBasicAuthenticationAsync(HttpClient client, string username, string password = ALL_TIME_PASSWORD)
        {
            var loginResponse = await client.PostAsync("/api/auth/login",
                new LoginDTO {Username = username, Password = password}.ToStringContent());
            var loginContent = JsonConvert.DeserializeObject<TokenDTO>(await loginResponse.Content.ReadAsStringAsync());

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginContent.AccessToken);
        }

        protected virtual async Task<User> SetupAuthenticationAsync()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            await SetupBasicAuthenticationAsync(_Client, user.EMail);
            return user;
        }

        protected DataContext CreateDataContext()
        {
            return _Server.Host.Services.GetRequiredService<DataContext>();
        }

        protected async Task<(User user,UserClaim claim)> SetupSysAdminAsync()
        {
            var user = await SetupAuthenticationAsync();
            var claim = await MakeUserSysAdminAsync(user);
            await SetupBasicAuthenticationAsync(_Client, user.EMail);
            return (user,claim);
        }

        protected async Task<UserClaim> MakeUserSysAdminAsync(User user)
        {
            var sysAdminClaim = UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.System_Administrator));
            sysAdminClaim.User_Id = user.Id;
            
            user.Claims.Add(sysAdminClaim);
            await _Context.SaveChangesAsync();
            return sysAdminClaim;
        }
        
        protected async Task<(ServiceType djType, ServiceType catererType)> CreateDefaultTypesAsync()
        {
            var dj = new ServiceType{Name = "DJ"};
            var caterer = new ServiceType{Name = "Caterer"};
            
            _Context.ServiceTypes.AddRange(dj,caterer);
            await _Context.SaveChangesAsync();
            return (dj, caterer);
        }


        protected async Task<(User firstUser, EventServiceModel firstService, User secondUser, EventServiceModel secodnService)> SetupServiceAsync()
        {
            var setup = await SetupAuthenticationAsync();
            var secondUser = CreateUser();
            var types = await CreateDefaultTypesAsync();

            var firstService = new EventServiceModel
                               {
                                   Profile = Guid.NewGuid().ToString(),
                                   PersonId = setup.Person.Id,
                                   Salary = 2000,
                                   TypeId = types.djType.Id,
                                   Location =
                                   {
                                       Country = Guid.NewGuid().ToString(),
                                       State = Guid.NewGuid().ToString(),
                                       Street = Guid.NewGuid().ToString(),
                                       ZipCode = Guid.NewGuid().ToString()
                                   }
                               };


            var secondService = new EventServiceModel
                                {
                                    Profile = Guid.NewGuid().ToString(),
                                    Person = secondUser.Person,
                                    Salary = 1000,
                                    TypeId = types.catererType.Id,
                                    Location =
                                    {
                                        Country = Guid.NewGuid().ToString(),
                                        State = Guid.NewGuid().ToString(),
                                        Street = Guid.NewGuid().ToString(),
                                        ZipCode = Guid.NewGuid().ToString()
                                    }
                                };


            _Context.Users.Add(secondUser);
            _Context.EventService.Add(firstService);
            _Context.EventService.Add(secondService);
            await _Context.SaveChangesAsync();

            return (setup, firstService, secondUser, secondService);
        }
        
        #endregion Helper
    }
}