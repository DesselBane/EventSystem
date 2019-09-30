using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTests
{
    public class ServiceControllerDeleteTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task DeleteService_MustBeAuthenticated()
        {
            var r = await _Client.DeleteAsync("api/service/1");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task DeleteService_NotFound_2_ServiceModel()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.DeleteAsync("api/service/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteService_Forbidden_3_UserCantDeleteServiceModel()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.DeleteAsync($"api/service/{setup.secodnService.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteService_Success()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.DeleteAsync($"api/service/{setup.firstService.Id}");

            r.EnsureSuccessStatusCode();
            Assert.False(await CreateDataContext().EventService.AnyAsync(x => x.Id == setup.firstService.Id));
        }
    }
}