using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Security;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataContracts;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.Authentication
{
    public class AuthControllerTest : TestBase
    {
        [Fact]
        public async Task ChangePassword_NotFound_HashDoesntExist()
        {
            var r = await _Client.PostAsync("/api/auth/changepassword", new PasswordUpdateDTO {Password = "newPassword", ResetHash = "asdljhn"}.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.RESET_HAST_DOESNT_EXIST), error.ErrorCode);
        }

        [Fact]
        public async Task ChangePassword_Success()
        {
            const string NEW_PW = "newPassword";
            var user = CreateUser();
            user.ResetHash = "1234ResetHash";
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("/api/auth/changepassword", new PasswordUpdateDTO {Password = NEW_PW, ResetHash = user.ResetHash}.ToStringContent());

            r.EnsureSuccessStatusCode();

            await _Context.Entry(user).ReloadAsync();

            Assert.Equal(NEW_PW.HashPassword(user.Salt).Item1, user.Password);
        }

        [Fact]
        public async Task ChangePassword_422_0_NoData()
        {
            var r = await _Client.PostAsync("api/auth/changePassword", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task ChangeUsername_MustBeAuthorized()
        {
            var r = await _Client.PostAsync("/api/auth/changeusername?username=asdkösjgn@gmx.de", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task ChangeUsername_Success()
        {
            var newUsername = Guid.NewGuid() + "@gmx.de";
            var user = await SetupAuthenticationAsync();

            var r = await _Client.PostAsync($"/api/auth/changeusername?username={newUsername}", null);
            r.EnsureSuccessStatusCode();

            await _Context.Entry(user).ReloadAsync();

            Assert.Equal(newUsername, user.EMail);
        }

        [Fact]
        public async Task ChangeUsername_Conflict_1_UsernameExists()
        {
            var user = await SetupAuthenticationAsync();

            var r = await _Client.PostAsync($"/api/auth/changeusername?username={user.EMail}", null);
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USERNAME_EXISTS_ALREADY), error.ErrorCode);
        }

        [Fact]
        public async Task ChangeUsername_422_1_WrongFormat()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.PostAsync("/api/auth/changeusername?username=asdb", null);

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USERNAME_NO_EMAIL), error.ErrorCode);
        }

        [Fact]
        public async Task ChangeUsername_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/auth/changeUsername", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteAccount_MustBeAuthorized()
        {
            var r = await _Client.DeleteAsync("/api/auth/deleteaccount");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task DeleteAccount_Success()
        {
            var user = await SetupAuthenticationAsync();

            var r = await _Client.DeleteAsync("/api/auth/deleteAccount");

            r.EnsureSuccessStatusCode();

            var ctx = CreateDataContext();

            var newUser = await ctx.Users.FirstOrDefaultAsync(x => string.Equals(x.EMail, user.EMail, StringComparison.CurrentCultureIgnoreCase));

            Assert.Null(newUser);
        }

        [Fact]
        public async Task DeleteAccount_NotFound_1_UserDoesntExist()
        {
            var user = await SetupAuthenticationAsync();

            _Context.Users.Remove(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.DeleteAsync("/api/auth/deleteaccount");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }


        [Fact]
        public async Task Register_CreatesProfile()
        {
            var user = CreateUser();

            var r = await _Client.PutAsync($"/api/Auth/Register?username={user.EMail}", null);
            r.EnsureSuccessStatusCode();

            var person = await _Context.RealPeople.FirstOrDefaultAsync(x => string.Equals(x.User.EMail, user.EMail, StringComparison.CurrentCultureIgnoreCase));
            Assert.NotNull(person);
        }

        [Fact]
        public async Task Register_Success()
        {
            var username = Guid.NewGuid() + "@gmx.de";
            var response = await _Client.PutAsync($"/api/Auth/Register?username={username}", new StringContent(""));
            response.EnsureSuccessStatusCode();

            var user = await _Context.Users.FirstOrDefaultAsync(x => string.Equals(x.EMail, username,
                StringComparison.CurrentCultureIgnoreCase));
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Register_Confilict_1_UsernameExists()
        {
            var username = "existingUsername@gmx.de";
            _Context.Users.Add(new User {EMail = username});
            await _Context.SaveChangesAsync();
            var response = await _Client.PutAsync($"/api/Auth/Register?username={username}", new StringContent(""));

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await response.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USERNAME_EXISTS_ALREADY), error.ErrorCode);
        }

        [Fact]
        public async Task Register_422_1_WrongFormat()
        {
            var response = await _Client.PutAsync("/api/Auth/Register?username=bla", new StringContent(""));

            Assert.Equal((HttpStatusCode) 422, response.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await response.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USERNAME_NO_EMAIL), error.ErrorCode);
        }

        [Fact]
        public async Task Register_422_0_NoData()
        {
            var r = await _Client.PutAsync("api/auth/register", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task ResetPassword_Success()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"/api/auth/resetpassword?username={user.EMail}", null);
            r.EnsureSuccessStatusCode();

            await _Context.Entry(user).ReloadAsync();

            Assert.True(!string.IsNullOrWhiteSpace(user.ResetHash));
        }

        [Fact]
        public async Task ResetPassword_NotFound_1_WrongUsername()
        {
            var r = await _Client.PostAsync("/api/auth/resetpassword?username=notExistingUser@gmx.de", null);

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task ResetPassword_422_0_NoData()
        {
            var r = await _Client.PostAsync("api/auth/resetPassword", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}