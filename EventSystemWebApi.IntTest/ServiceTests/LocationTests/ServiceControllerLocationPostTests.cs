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

namespace EventSystemWebApi.IntTest.ServiceTests.LocationTests
{
    public class ServiceControllerLocationPostTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task UpdateLocation_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/service/1/location", "".ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task UpdateLocation_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/service/1/location", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateLocation_NotFound_2_ServiceModel()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/service/999/location", new Location
            {
                Country = Guid.NewGuid().ToString(),
                State = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                
                ZipCode = Guid.NewGuid().ToString()
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateLocation_Forbidden_2_UserDoesntHavePermission()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.PostAsync($"api/service/{setup.secodnService.Id}/location", new Location
            {
                Country = Guid.NewGuid().ToString(),
                State = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                ZipCode = Guid.NewGuid().ToString()
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateLocation_Success_Result()
        {
            var setup = await SetupServiceAsync();
            var model = new Location
            {
                Country = Guid.NewGuid().ToString(),
                State = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ZipCode = Guid.NewGuid().ToString()
            };

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}/location", model.ToStringContent());

            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<Location>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.Equal(model.Country, result.Country);
            Assert.Equal(model.State, result.State);
            Assert.Equal(model.Street, result.Street);
            Assert.Equal(model.ZipCode, result.ZipCode);
            Assert.Equal(setup.firstService.LocationId, result.Id);
            Assert.Equal(model.City, result.City);
        }

        [Fact]
        public async Task UpdateLocation_Success_Database()
        {
            var setup = await SetupServiceAsync();
            var model = new Location
            {
                Country = Guid.NewGuid().ToString(),
                State = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                City = Guid.NewGuid().ToString(),
                ZipCode = Guid.NewGuid().ToString()
            };

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}/location", model.ToStringContent());

            r.EnsureSuccessStatusCode();

            var result = await CreateDataContext().Locations.FirstOrDefaultAsync(x => x.Id == setup.firstService.LocationId);

            Assert.NotNull(result);
            Assert.Equal(model.Country, result.Country);
            Assert.Equal(model.State, result.State);
            Assert.Equal(model.Street, result.Street);
            Assert.Equal(model.ZipCode, result.ZipCode);
            Assert.Equal(model.City, result.City);
        }
    }
}