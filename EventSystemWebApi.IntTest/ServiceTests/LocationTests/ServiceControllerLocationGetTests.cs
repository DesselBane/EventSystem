using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Misc;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTests.LocationTests
{
    public class ServiceControllerLocationGetTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task GetLocation_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/service/1/location");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetLocation_NotFound_2_EventServiceModel()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/service/999/location");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetLocation_Success()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}/location");

            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<Location>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.Equal(setup.firstService.Location.Country, result.Country);
            Assert.Equal(setup.firstService.Location.State, result.State);
            Assert.Equal(setup.firstService.Location.Street, result.Street);
            Assert.Equal(setup.firstService.Location.ZipCode, result.ZipCode);
            Assert.Equal(setup.firstService.Location.Id, result.Id);
        }
    }
}