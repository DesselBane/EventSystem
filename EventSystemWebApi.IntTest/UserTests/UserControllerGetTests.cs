using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.UserTests
{
    public class UserControllerGetTests : UserControllerTestBase
    {
        [Fact]
        public async Task GetUsers_Success()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;
            
            var user1 = CreateUser();
            var user2 = CreateUser();
            
            _Context.Users.AddRange(user1,user2);
            await _Context.SaveChangesAsync();

            await MakeUserSysAdminAsync(user2);
            
            var r = await _Client.GetAsync("api/user");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());

            Assert.Equal(3,result.Count);
            Assert.True(result.Any(x => x.id == user.Id && x.eMail == user.EMail && x.realPersonId == user.RealPersonId));
            Assert.True(result.Any(x => x.id == user1.Id && x.eMail == user1.EMail && x.realPersonId == user1.RealPersonId));
            Assert.True(result.Any(x => x.id == user2.Id && x.eMail == user2.EMail && x.realPersonId == user2.RealPersonId));
        }

        [Fact]
        public async Task GetUsers_DefaultUserEmptyList()
        {
            await SetupAuthenticationAsync();
            
            var user1 = CreateUser();
            var user2 = CreateUser();
            
            _Context.Users.AddRange(user1,user2);
            await _Context.SaveChangesAsync();
            await MakeUserSysAdminAsync(user2);

            var r = await _Client.GetAsync("api/user");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<User>>(await r.Content.ReadAsStringAsync());
            
            Assert.Equal(0,result.Count);
        }

        [Fact]
        public async Task GetSingleUser_NotFound()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/user/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(UserErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingleUser_Forbidden()
        {
            await SetupAuthenticationAsync();
            var user = await CreateUserAsync();

            var r = await _Client.GetAsync($"api/user/{user.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(UserErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingleUser_Success()
        {
            await SetupSysAdminAsync();
            var user = await CreateUserAsync();

            var r = await _Client.GetAsync($"api/user/{user.Id}");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<User>(await r.Content.ReadAsStringAsync());
            var dynamicResult = JsonConvert.DeserializeObject<dynamic>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(user.EMail,result.EMail);
            Assert.Equal(user.Id,result.Id);
            
            Assert.NotNull(dynamicResult);
            Assert.Equal(user.RealPersonId.Value,(int)dynamicResult.realPersonId);
        }

    }
}