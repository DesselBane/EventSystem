using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Misc;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.LocationTests
{
    public class EventControllerLocationGetTests : EventControllerTestBase
    {
        [Fact]
        public async Task Location_NotFound_1_Event()
        {
            await SetupAsync();

            var r = await _Client.GetAsync("api/event/999/location");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task Location_Forbidden_1_CannotReadEvent()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/location");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task Location_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/event/1/location");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task Location_Success()
        {
            var setup = await SetupEventAsync();

            setup.firstEvent.Location.Country = "Germany";
            setup.firstEvent.Location.State = "Bavaria";
            setup.firstEvent.Location.Street = "Hauptstraße 345";
            setup.firstEvent.Location.ZipCode = "98765";

            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/location");
            r.EnsureSuccessStatusCode();

            var location = JsonConvert.DeserializeObject<Location>(await r.Content.ReadAsStringAsync());
            Assert.NotNull(location);
            Assert.Equal(setup.firstEvent.Location.Country, location.Country);
            Assert.Equal(setup.firstEvent.Location.State, location.State);
            Assert.Equal(setup.firstEvent.Location.Street, location.Street);
            Assert.Equal(setup.firstEvent.Location.ZipCode, location.ZipCode);
            Assert.Equal(setup.firstEvent.Location.Id, location.Id);
        }
    }
}