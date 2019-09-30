using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Misc;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.LocationTests
{
    public class EventControllerLocationPostTests : EventControllerTestBase
    {
        [Fact]
        public async Task Location_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/event/1/location", new Location().ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task Location_422_0_NoData()
        {
            await SetupAsync();
            var r = await _Client.PostAsync("api/event/1/location", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task Location_NotFound_1_Event()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.PostAsync("api/event/999/location", new Location().ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task Location_Forbidden_2_CannotUpdate()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/location", new Location().ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task Location_Success_Return()
        {
            var setup = await SetupEventAsync();
            var loc = new Location
            {
                Id = -1,
                Country = Guid.NewGuid().ToString(),
                State = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ZipCode = Guid.NewGuid().ToString()
            };

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/location", loc.ToStringContent());

            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<Location>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.Equal(loc.Country, result.Country);
            Assert.Equal(loc.State, result.State);
            Assert.Equal(loc.Street, result.Street);
            Assert.Equal(loc.ZipCode, result.ZipCode);
            Assert.Equal(loc.City, result.City);
        }

        [Fact]
        public async Task Location_Success_Database()
        {
            var setup = await SetupEventAsync();
            var loc = new Location
            {
                Id = -1,
                Country = Guid.NewGuid().ToString(),
                State = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ZipCode = Guid.NewGuid().ToString()
            };

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/location", loc.ToStringContent());

            r.EnsureSuccessStatusCode();

            var result = await CreateDataContext().Locations.FirstOrDefaultAsync(x => x.Id == setup.firstEvent.LocationId);

            Assert.NotNull(result);
            Assert.Equal(loc.Country, result.Country);
            Assert.Equal(loc.State, result.State);
            Assert.Equal(loc.Street, result.Street);
            Assert.Equal(loc.ZipCode, result.ZipCode);
            Assert.Equal(loc.City, result.City);
        }
    }
}