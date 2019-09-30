using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.PermissionTests
{
    public class PermissionControllerDeleteTests : PermissionControllerTestBase
    {
        [Fact]
        public async Task DeletePermission_NotFound_User()
        {
            var setup = await SetupSysAdminAsync();

            var r = await _Client.DeleteAsync($"api/user/999/permission/{setup.claim.Id}");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(UserErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeletePermission_NotFound_Claim()
        {
            var setup = await SetupSysAdminAsync();

            var r = await _Client.DeleteAsync($"api/user/{setup.user.Id}/permission/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.CLAIM_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeletePermission_Forbidden()
        {
            await SetupAuthenticationAsync();

            var user = await CreateUserAsync();
            var claim = await MakeUserSysAdminAsync(user);

            var r = await _Client.DeleteAsync($"api/user/{user.Id}/permission/{claim.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.NO_DELETE_PERMISSION), error.ErrorCode);
        }

        [Fact]
        public async Task DeletePermission_Success()
        {
            var setup = await SetupSysAdminAsync();

            var r = await _Client.DeleteAsync($"api/user/{setup.user.Id}/permission/{setup.claim.Id}");
            r.EnsureSuccessStatusCode();
            
            Assert.False(await CreateDataContext().Claims.AnyAsync(x => x.Id == setup.claim.Id && x.User_Id == setup.user.Id));
        }
    }
}