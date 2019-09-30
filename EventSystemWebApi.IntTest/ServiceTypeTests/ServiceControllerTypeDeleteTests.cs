using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.ServiceTests;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests
{
    public class ServiceControllerTypeDeleteTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task SPType_Success()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();

            var r = await _Client.DeleteAsync($"api/serviceTypes/{type.Id}");
            r.EnsureSuccessStatusCode();

            var dbType = await CreateDataContext().ServiceTypes.FirstOrDefaultAsync(x => x.Id == type.Id);

            Assert.Null(dbType);
        }

        [Fact]
        public async Task SPType_Forbidden_1()
        {
            await SetupAuthenticationAsync();

            var type = await CreateTypeAsync();

            var r = await _Client.DeleteAsync($"api/serviceTypes/{type.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var dbType = await CreateDataContext().ServiceTypes.FirstOrDefaultAsync(x => x.Id == type.Id);

            Assert.NotNull(dbType);
            Assert.Equal(type.Name, dbType.Name);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task SPType_NotFound_1_Type()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.DeleteAsync("api/serviceTypes/222");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task SPType_MustBeAuthenticated()
        {
            var r = await _Client.DeleteAsync("api/serviceTypes/0");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }
    }
}