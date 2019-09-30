using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceAgreementTests
{
    public class ServiceAgreementGetTests : EventControllerTestBase
    {
        [Fact]
        public async Task GetSingle_Forbidden()
        {
            var setup = await SetupEventServiceSlotsAsync();
            await SetupServiceAgreementAsync(setup.secondEvent.Id, setup.secondSlot.Id, setup.secondUser.Person.Id);

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement");
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_Event()
        {
            await SetupAuthenticationAsync();
            
            var r = await _Client.GetAsync("api/event/999/sps/999/agreement");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/sps/999/agreement");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_Agreement()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"/api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_Success()
        {
            var setupEventSlot = await SetupEventServiceSlotsAsync();
            var setupAgreementService = await SetupServiceAgreementAsync(setupEventSlot.firstEvent.Id, setupEventSlot.firstSlot.Id, setupEventSlot.firstUser.Person.Id);

            var agreement = setupAgreementService.agreement;
            agreement.Comment = Guid.NewGuid().ToString();
            agreement.Cost = 200;
            agreement.End = DateTime.UtcNow.AddDays(2);
            agreement.Start = DateTime.UtcNow.AddDays(1);
            agreement.State = ServiceAgreementStates.Proposal;

            await _Context.SaveChangesAsync();
            
            var r = await _Client.GetAsync($"api/event/{setupEventSlot.firstEvent.Id}/sps/{setupEventSlot.firstSlot.Id}/agreement");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAgreement>(await r.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            Assert.Equal(agreement.Comment,result.Comment);
        }
        

        [Fact]
        public async Task GetAll_NotFound_Event()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/event/999/agreements");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetAll_Forbidden()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/agreements");
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task GetAll_Success()
        {
            var setup = await SetupEventAsync();
            var types = await CreateDefaultTypesAsync();

            var slot1 = new ServiceSlot
                        {
                            EventId = setup.firstEvent.Id,
                            TypeId = types.djType.Id
                        };

            var slot2 = new ServiceSlot
                        {
                            EventId = setup.firstEvent.Id,
                            TypeId = types.catererType.Id
                        };
            
            _Context.ServiceSlots.AddRange(slot1,slot2);
            await _Context.SaveChangesAsync();

            var agreement1 = (await SetupServiceAgreementAsync(setup.firstEvent.Id, slot1.Id, setup.first.RealPersonId.Value)).agreement;
            var agreement2 = (await SetupServiceAgreementAsync(setup.firstEvent.Id, slot2.Id, setup.first.RealPersonId.Value)).agreement;

            agreement1.Comment = Guid.NewGuid().ToString();
            agreement2.Comment = Guid.NewGuid().ToString();
            
            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/agreements");

            var result = JsonConvert.DeserializeObject<List<ServiceAgreement>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(2,result.Count);
            Assert.True(result.Any(x => x.Comment == agreement1.Comment));
            Assert.True(result.Any(x => x.Comment == agreement2.Comment));
        }
    }
}