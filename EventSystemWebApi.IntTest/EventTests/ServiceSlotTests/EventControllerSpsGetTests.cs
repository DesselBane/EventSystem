using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceSlotTests
{
    public class EventControllerSpsGetTests : EventControllerTestBase
    {
        [Fact]
        public async Task GetServiceProviderSlot_Forbidden_1()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task GetServiceProviderSlot_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/event/1/sps/1");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetServiceProviderSlot_NotFound_4_Slot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/sps/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetServiceProviderSlot_NotFound_1_Event()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/999/sps/{setup.firstSlot.Id}");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetServiceProviderSlot_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}");
            r.EnsureSuccessStatusCode();

            var slot = JsonConvert.DeserializeObject<ServiceSlot>(await r.Content.ReadAsStringAsync());

            Assert.Equal(setup.firstSlot.Id, slot.Id);
            Assert.Equal(setup.firstSlot.EventId, slot.EventId);
            Assert.Equal(setup.firstSlot.TypeId, slot.TypeId);
        }

        [Fact]
        public async Task GetAllServiceProviderSlots_NotFound_1_Event()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.GetAsync("api/event/999/sps");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetAllServiceProviderSlots_Forbidden_1_NoEventReadPermissions()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/sps");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task GetAllServiceProviderSlot_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/sps");

            r.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<List<ServiceSlot>>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Equal(setup.firstSlot.Id, result[0].Id);
            Assert.Equal(setup.firstSlot.EventId, result[0].EventId);
            Assert.Equal(setup.firstSlot.TypeId, result[0].TypeId);
            Assert.Equal(setup.firstSlot.BudgetTarget, result[0].BudgetTarget);
            Assert.Equal(setup.firstSlot.End, result[0].End);
            Assert.Equal(setup.firstSlot.Start, result[0].Start);
        }

        [Fact]
        public async Task GetAllServiceProviderSlots_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/event/1/sps");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }
    }
}