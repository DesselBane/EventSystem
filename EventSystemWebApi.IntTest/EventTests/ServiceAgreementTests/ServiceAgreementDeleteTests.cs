using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceAgreementTests
{
    public class ServiceAgreementDeleteTests : EventControllerTestBase
    {
        [Fact]
        public async Task DeleteAgreement_Forbidden()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id, setup.secondSlot.Id, setup.firstUser.RealPersonId.Value);

            var r = await _Client.DeleteAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement");
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteAgreement_NotFound_Event()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.DeleteAsync("api/event/999/sps/999/agreement");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteAgreement_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/sps/999/agreement");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteAgreement_NotFound_Agreement()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteAgreement_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();
            await SetupServiceAgreementAsync(setup.firstEvent.Id, setup.firstSlot.Id, setup.secondUser.RealPersonId.Value);

            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement");
            r.EnsureSuccessStatusCode();

            using (var ctx = CreateDataContext())
            {
                Assert.False(await ctx.ServiceAgreements.AnyAsync(x => x.EventId == setup.firstEvent.Id && x.ServiceSlotId == setup.firstSlot.Id));
            }
        }
    }
}