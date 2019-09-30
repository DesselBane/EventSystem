using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.PermissionTests
{
    public class PermissionControllerGetTests : PermissionControllerTestBase
    {
        
        [Fact]
        public async Task GetPermissions_Forbidden()
        {
            await SetupAuthenticationAsync();

            var otherUser = CreateUser();
            _Context.Users.Add(otherUser);
            await _Context.SaveChangesAsync();

            await MakeUserSysAdminAsync(otherUser);

            var r = await _Client.GetAsync($"api/user/{otherUser.Id}/permission");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.NO_GET_PERMISSION), error.ErrorCode);
        }

        [Fact]
        public async Task GetPermissions_NotFound_User()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/user/999/permission");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(UserErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetPermissions_Success()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;

            var r = await _Client.GetAsync($"api/user/{user.Id}/permission");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<UserClaim>>(await r.Content.ReadAsStringAsync());
            
            Assert.True(result.Any(x => x.Type == RoleClaim.ROLE_CLAIM_TYPE && x.Value == "1"));
            Assert.True(result.All(x => x.User_Id == user.Id));
        }

        [Fact]
        public async Task GetSinglePermission_NotFound_User()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;

            var r = await _Client.GetAsync($"api/user/999/permission/{user.Claims[0].Id}");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(UserErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSinglePermission_NotFound_Claim()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;
            var r = await _Client.GetAsync($"api/user/{user.Id}/permission/999");


            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.CLAIM_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSinglePermission_Forbidden()
        {
            await SetupAuthenticationAsync();

            var user = CreateUser();
            _Context.Add(user);
            await _Context.SaveChangesAsync();
            
            await MakeUserSysAdminAsync(user);

            var r = await _Client.GetAsync($"api/user/{user.Id}/permission/{user.Claims[0].Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.NO_GET_PERMISSION), error.ErrorCode);

        }

        [Fact]
        public async Task GetSinglePermission_Success()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;
            var claim = user.Claims.First(x => x.Type == RoleClaim.ROLE_CLAIM_TYPE && x.Value == ((int) RoleClaimTypes.System_Administrator).ToString());

            var r = await _Client.GetAsync($"api/user/{user.Id}/permission/{claim.Id}");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<UserClaim>(await r.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            
            Assert.Equal(RoleClaim.ROLE_CLAIM_TYPE,result.Type);
            Assert.Equal(((int)RoleClaimTypes.System_Administrator).ToString(),result.Value);
        }
    }
}