using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceAgreementTests
{
    public class ServiceAgreementPutTests : EventControllerTestBase
    {
        [Fact]
        public async Task PutAgreement_Forbidden()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var service = new EventServiceModel
                          {
                              Location = new Location(),
                              PersonId = setup.secondUser.RealPersonId.Value,
                              TypeId = setup.firstSlot.TypeId
                          };

            _Context.EventService.Add(service);
            await _Context.SaveChangesAsync();

            
            var r = await _Client.PutAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/request/{service.Id}", "".ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task PutAgreement_NotFound_Event()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.PutAsync("api/event/9999/sps/999/request/999", "".ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PutAgreement_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps/999/request/999", "".ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PutAgreement_NotFound_Service()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/request/999", "".ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PutAgreement_Conflict_AgreementAlreadyExists()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id, setup.firstSlot.Id, setup.firstUser.RealPersonId.Value);
            

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/request/{agreementService.serivce.Id}", "".ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_ALREADY_EXISTS), error.ErrorCode);
        }

        [Fact]
        public async Task PutAgreement_Success_Host()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var service = new EventServiceModel
                          {
                              Location = new Location(),
                              PersonId = setup.secondUser.RealPersonId.Value,
                              TypeId = setup.firstSlot.TypeId
                          };

            _Context.EventService.Add(service);
            await _Context.SaveChangesAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/request/{service.Id}", "".ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAgreement>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Request,result.State);
        }
        
        //TODO add tests for helper etc
    }
}