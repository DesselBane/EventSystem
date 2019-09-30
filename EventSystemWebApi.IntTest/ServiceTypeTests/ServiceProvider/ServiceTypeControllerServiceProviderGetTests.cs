using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests.ServiceProvider
{
    public class ServiceTypeControllerServiceProviderGetTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task GetServiceProvider_Success()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupAuthenticationAsync();
            var service1 = new EventServiceModel
                          {
                              Location = new Location(),
                              PersonId = setup.RealPersonId.Value,
                              TypeId = types.djType.Id,
                              Profile = Guid.NewGuid().ToString()
                          };

            var service2 = new EventServiceModel
                           {
                               Location = new Location(),
                               PersonId = setup.RealPersonId.Value,
                               TypeId = types.djType.Id,
                               Profile = Guid.NewGuid().ToString()
                           };

            var service3 = new EventServiceModel
                           {
                               Location = new Location(),
                               PersonId = setup.RealPersonId.Value,
                               TypeId = types.catererType.Id,
                               Profile = Guid.NewGuid().ToString()
                           };

            _Context.EventService.AddRange(service1,service2,service3);
            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync($"api/serviceTypes/{types.djType.Id}/serviceProvider");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<EventServiceModel>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(2,result.Count);
            Assert.True(result.Any(x => x.Profile == service1.Profile));
            Assert.True(result.Any(x => x.Profile == service2.Profile));
        }

        [Fact]
        public async Task GetServiceProvider_NotFound_ServiceType()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/serviceTypes/999/serviceProvider");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }
    }
}