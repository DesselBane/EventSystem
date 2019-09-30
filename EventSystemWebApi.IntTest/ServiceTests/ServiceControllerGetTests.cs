using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTests
{
    public class ServiceControllerGetTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task GetService_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/service/1");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetService_NotFound_2_Service()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/service/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetService_Success()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<EventServiceModel>(await r.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            Assert.Equal(setup.firstService.Id, result.Id);
            Assert.Equal(setup.firstService.LocationId, result.LocationId);
            Assert.Equal(setup.firstService.PersonId, result.PersonId);
            Assert.Equal(setup.firstService.Profile, result.Profile);
            Assert.Equal(setup.firstService.Salary, result.Salary);
            Assert.Equal(setup.firstService.TypeId, result.TypeId);
        }

        [Fact]
        public async Task GetAgreements_Success()
        {
            var setup = await SetupServiceAsync();
            var myEvent = new Event
                          {
                              HostId = setup.secondUser.RealPersonId.Value,
                              Location = new Location()
                          };

            _Context.Events.Add(myEvent);
            await _Context.SaveChangesAsync();

            var slot = new ServiceSlot
                       {
                           EventId = myEvent.Id,
                           TypeId = setup.firstService.TypeId
                       };

            var badSlot = new ServiceSlot
                          {
                              EventId = myEvent.Id,
                              TypeId = setup.secodnService.TypeId
                          };

            var otherSlot = new ServiceSlot
                            {
                                EventId = myEvent.Id,
                                TypeId = setup.secodnService.Id
                            };
            
            _Context.ServiceSlots.AddRange(slot,badSlot,otherSlot);
            await _Context.SaveChangesAsync();

            var otherService = new EventServiceModel
                               {
                                   Location = new Location(),
                                   PersonId = setup.firstService.PersonId,
                                   TypeId = setup.secodnService.TypeId
                               };

            _Context.EventService.Add(otherService);

            var firstAgreement = new ServiceAgreement
                                 {
                                     EventId = myEvent.Id,
                                     EventServiceModelId = setup.firstService.Id,
                                     ServiceSlotId = slot.Id,
                                     Comment = Guid.NewGuid().ToString()
                                 };

            var badAgreement = new ServiceAgreement
                               {
                                   EventId = myEvent.Id,
                                   EventServiceModelId = setup.secodnService.Id,
                                   ServiceSlotId = badSlot.Id,
                                   Comment = Guid.NewGuid().ToString()
                               };

            var otherAgreement = new ServiceAgreement
                                 {
                                     EventId = myEvent.Id,
                                     EventServiceModelId = otherService.Id,
                                     ServiceSlotId = otherSlot.Id,
                                     Comment = Guid.NewGuid().ToString()
                                 };
            
            _Context.ServiceAgreements.AddRange(firstAgreement,badAgreement,otherAgreement);
            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync("api/service/my/agreements");
            r.EnsureSuccessStatusCode();
            
            var result = JsonConvert.DeserializeObject<List<ServiceAgreement>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(2,result.Count);
            Assert.True(result.Any(x => x.Comment == otherAgreement.Comment));
            Assert.True(result.Any(x => x.Comment == firstAgreement.Comment));
            Assert.False(result.Any(x => x.Comment == badAgreement.Comment));
        }

        [Fact]
        public async Task GetAgreements_None()
        {
            await SetupServiceAsync();

            var r = await _Client.GetAsync("api/service/my/agreements");
            r.EnsureSuccessStatusCode();
            
            var result = JsonConvert.DeserializeObject<List<ServiceAgreement>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(0,result.Count);
        }

        [Fact]
        public async Task GetAgreements_NoService()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/service/my/agreements");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ServiceAgreement>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(0,result.Count);
        }
    }
}