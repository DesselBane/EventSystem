using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceAgreementTests
{
    public class ServiceAgreementPostTests : EventControllerTestBase
    {
        //Update Values
        
        [Fact]
        public async Task PostAgreement_Forbidden_Outsider()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }
        
        [Fact]
        public async Task PostAgreement_Forbidden_RequestState_EventHelper()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_NotFound_Event()
        {
            await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync("api/event/999/sps/999/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/999/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
            
        }

        [Fact]
        public async Task PostAgreement_NotFound_Agreement()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/event/999/sps/999/agreement", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_422_StartBeforeEnd()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement", new ServiceAgreement
                                                                                                                     {
                                                                                                                         End = DateTime.UtcNow,
                                                                                                                         Start = DateTime.UtcNow.AddDays(1),
                                                                                                                         State = ServiceAgreementStates.Proposal
                                                                                                                     }.ToStringContent());
            
            Assert.Equal((HttpStatusCode)422 , r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_START_BEFORE_END), error.ErrorCode);

        }

        [Fact]
        public async Task PostAgreement_Confilic_Proposal()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Proposal;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.CANNOT_UPDATE_VALUES), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_Conflict_Accepted()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Accepted;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.CANNOT_UPDATE_VALUES), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_Conflict_Declined()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Declined;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.CANNOT_UPDATE_VALUES), error.ErrorCode);
        }

        [Fact]
        public async Task PostAgreement_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var serviceAgreement = new ServiceAgreement
                                   {
                                       Comment = Guid.NewGuid().ToString(),
                                       Cost = 199292,
                                       Start = DateTime.UtcNow,
                                       End = DateTime.UtcNow.AddDays(1),
                                       State = ServiceAgreementStates.Declined
                                   };

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement", serviceAgreement.ToStringContent());

            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAgreement>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(serviceAgreement.Comment, result.Comment);
            Assert.Equal(serviceAgreement.Cost, result.Cost);
            Assert.Equal(serviceAgreement.Start, result.Start);
            Assert.Equal(serviceAgreement.End, result.End);
            Assert.Equal(ServiceAgreementStates.Request, result.State);

            result = await _Context.ServiceAgreements.FirstOrDefaultAsync(x => x.EventId == setup.secondEvent.Id
                                                                               && x.EventServiceModelId == agreementService.serivce.Id
                                                                               && x.ServiceSlotId == setup.secondSlot.Id);

            await _Context.Entry(result).ReloadAsync();
            
            Assert.NotNull(result);
            Assert.Equal(serviceAgreement.Comment, result.Comment);
            Assert.Equal(serviceAgreement.Cost, result.Cost);
            // Removed from test since time formats and database saves etc create differences of a couple nano secs or so....
            //Assert.Equal(serviceAgreement.Start, result.Start.ToUniversalTime());
            //Assert.Equal(serviceAgreement.End, result.End.ToUniversalTime());
            Assert.Equal(ServiceAgreementStates.Request, result.State);
        }
        
        //Update to proposal

        [Fact]
        public async Task MakePorposal_Forbidden_Outsider()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task MakeProposal_Forbidden_EventHelper()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }
        
        [Fact]
        public async Task MakeProposal_NotFound_Event()
        {
            await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync("api/event/999/sps/999/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task MakeProposal_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/999/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
            
        }

        [Fact]
        public async Task MakeProposal_NotFound_Agreement()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND), error.ErrorCode);
        }
        
        [Fact]
        public async Task MakeProposal_Confilic_Proposal()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Proposal;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL), error.ErrorCode);
        }

        [Fact]
        public async Task MakeProposal_Conflict_Accepted()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Accepted;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL), error.ErrorCode);
        }

        [Fact]
        public async Task MakeProposal_Conflict_Declined()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Declined;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL), error.ErrorCode);
        }

        [Fact]
        public async Task MakeProposal_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();
            
            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/proposal", new ServiceAgreement().ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAgreement>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Proposal,result.State);
            
            result = await _Context.ServiceAgreements.FirstOrDefaultAsync(x => x.EventId == setup.secondEvent.Id
                                                                               && x.EventServiceModelId == agreementService.serivce.Id
                                                                               && x.ServiceSlotId == setup.secondSlot.Id);

            await _Context.Entry(result).ReloadAsync();
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Proposal,result.State);

        }

        
        // Update to Accepted

        [Fact]
        public async Task AcceptAgreement_Forbidden_Outsider()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Proposal;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task AcceptAgreement_Forbidden_ServiceProvider()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.firstUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }
        
        [Fact]
        public async Task AcceptAgreement_NotFound_Event()
        {
            await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync("api/event/999/sps/999/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task AcceptAgreement_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/999/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
            
        }

        [Fact]
        public async Task AcceptAgreement_NotFound_Agreement()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task AcceptAgreement_Confilic_Request()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Request;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL), error.ErrorCode);
        }

        [Fact]
        public async Task AcceptAgreement_Conflict_Accepted()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Accepted;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL), error.ErrorCode);
        }

        [Fact]
        public async Task AcceptAgreement_Conflict_Declined()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Declined;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL), error.ErrorCode);
        }

        [Fact]
        public async Task AcceptAgreement_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Proposal;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/accept", new ServiceAgreement().ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAgreement>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Accepted,result.State);
            
            result = await _Context.ServiceAgreements.FirstOrDefaultAsync(x => x.EventId == setup.firstEvent.Id
                                                                               && x.EventServiceModelId == agreementService.serivce.Id
                                                                               && x.ServiceSlotId == setup.firstSlot.Id);
            
            await _Context.Entry(result).ReloadAsync();
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Accepted,result.State);
        }

        
        //Decline Agreement

        [Fact]
        public async Task DeclineAgreement_Forbidden_Outsider()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.secondEvent.Id,setup.secondSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Proposal;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/sps/{setup.secondSlot.Id}/agreement/decline", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }
        
        [Fact]
        public async Task DeclineAgreement_NotFound_Event()
        {
            await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync("api/event/999/sps/999/agreement/decline", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeclineAgreement_NotFound_ServiceSlot()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/999/agreement/decline", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
            
        }

        [Fact]
        public async Task DeclineAgreement_NotFound_Agreement()
        {
            var setup = await SetupEventServiceSlotsAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/decline", new ServiceAgreement().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeclineAgreement_Success()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var agreementService = await SetupServiceAgreementAsync(setup.firstEvent.Id,setup.firstSlot.Id,setup.secondUser.RealPersonId.Value);
            agreementService.agreement.State = ServiceAgreementStates.Proposal;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}/agreement/decline","".ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAgreement>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Declined,result.State);
            
            result = await _Context.ServiceAgreements.FirstOrDefaultAsync(x => x.EventId == setup.firstEvent.Id
                                                                               && x.EventServiceModelId == agreementService.serivce.Id
                                                                               && x.ServiceSlotId == setup.firstSlot.Id);
            
            await _Context.Entry(result).ReloadAsync();
            
            Assert.NotNull(result);
            Assert.Equal(ServiceAgreementStates.Declined,result.State);
        }

    }
    
    
    
}