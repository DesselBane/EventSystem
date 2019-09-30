using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceSlotTests
{
    public class EventControllerSpsDeleteTests : EventControllerTestBase
    {
        [Fact]
        public async Task DeleteServiceProviderSlot_Forbidden_2()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.DeleteAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}");
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteServiceProviderSlot_MustBeAuthenticated()
        {
            var r = await _Client.DeleteAsync("api/event/1/sps/1");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task DeleteServiceProviderSlot_NotFound_4_Slot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/sps/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteServiceProviderSlot_NotFound_1_Event()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.DeleteAsync($"api/event/999/sps/{setup.firstSlot.Id}");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteServiceProviderSlot_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}");
            r.EnsureSuccessStatusCode();

            var ctx = CreateDataContext();
            var slot = await ctx.ServiceSlots.FirstOrDefaultAsync(x => x.Id == setup.firstSlot.Id && x.EventId == setup.firstEvent.Id);

            Assert.Null(slot);
        }
    }
}