using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.PermissionTests
{
    public class PermissionControllerPutTests : PermissionControllerTestBase
    {
        [Fact]
        public async Task CreatePermission_NotFound_User()
        {
            await SetupSysAdminAsync();

            var r = await _Client.PutAsync("api/user/999/permission", new UserClaim().ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(UserErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task CreatePermission_Forbidden()
        {
            await SetupAuthenticationAsync();
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            await MakeUserSysAdminAsync(user);

            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.ServiceProviderTyp_Administrator)).ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.NO_UPDATE_PERMISSION), error.ErrorCode);
        }

        [Fact]
        public async Task CreatPermission_422_InvalidType()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;

            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", new UserClaim {Type = "my invalid type goes here"}.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.INVALID_CLAIM_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task CreatePermission_422_InvalidValue_String()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;


            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", new UserClaim
            {
                Type = RoleClaim.ROLE_CLAIM_TYPE,
                Value = "this is an invalid value :D"
            }.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.INVALID_CLAIM_VALUE), error.ErrorCode);
        }
        
        [Fact]
        public async Task CreatePermission_422_InvalidValue_Int()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;


            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", new UserClaim
            {
                Type = RoleClaim.ROLE_CLAIM_TYPE,
                Value = "-1"
            }.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.INVALID_CLAIM_VALUE), error.ErrorCode);
        }

        [Fact]
        public async Task CreatePermission_Conflict_ClaimAlreadyCreated()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;


            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.System_Administrator)).ToStringContent());

            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PermissionErrorCodes.CLAIM_ALREADY_CREATED), error.ErrorCode);
        }
        
        [Fact]
        public async Task CreatePermission_Success_Result()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;


            var claim = UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.ServiceProviderTyp_Administrator));

            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", claim.ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<UserClaim>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(RoleClaim.ROLE_CLAIM_TYPE,result.Type);
            Assert.Equal(user.Id,result.User_Id);
            Assert.Equal(((int)RoleClaimTypes.ServiceProviderTyp_Administrator).ToString(),result.Value);
        }
        
        [Fact]
        public async Task CreatePermission_Success_Database()
        {
            var userAndClaim = await SetupSysAdminAsync();
            var user = userAndClaim.user;


            var claim = UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.ServiceProviderTyp_Administrator));

            var r = await _Client.PutAsync($"api/user/{user.Id}/permission", claim.ToStringContent());
            r.EnsureSuccessStatusCode();

            var json = JsonConvert.DeserializeObject<UserClaim>(await r.Content.ReadAsStringAsync());
            var result = await CreateDataContext().Claims.FirstOrDefaultAsync(x => x.Id == json.Id);
            
            Assert.NotNull(result);
            Assert.Equal(RoleClaim.ROLE_CLAIM_TYPE,result.Type);
            Assert.Equal(user.Id,result.User_Id);
            Assert.Equal(((int)RoleClaimTypes.ServiceProviderTyp_Administrator).ToString(),result.Value);
        }
    }
}