using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.ServiceTests;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests
{
    public class ServiceControllerTypeGetTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task GetProviderTypes_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/serviceTypes");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetProviderTypes_Success()
        {
            const string NAME1 = "DJ";
            const string NAME2 = "Caterer";

            var typ1 = new ServiceType
            {
                Name = NAME1
            };

            var typ2 = new ServiceType {Name = NAME2};

            await _Context.Database.ExecuteSqlCommandAsync("delete from ServiceTypes");
            _Context.ServiceTypes.AddRange(typ1, typ2);
            await _Context.SaveChangesAsync();

            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/serviceTypes");
            r.EnsureSuccessStatusCode();

            var retVal = JsonConvert.DeserializeObject<IEnumerable<ServiceType>>(await r.Content.ReadAsStringAsync()).ToList();

            Assert.Equal(2, retVal.Count);
            Assert.True(retVal.Exists(x => x.Name == NAME1));
            Assert.True(retVal.Exists(x => x.Name == NAME2));
        }

        [Fact]
        public async Task GetProviderType_NotFound_1()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.GetAsync("api/serviceTypes/999");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetProvider_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/serviceTypes/0");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetProviderType_Success()
        {
            const string NAME1 = "DJ";
            const string NAME2 = "Caterer";

            var typ1 = new ServiceType
            {
                Name = NAME1
            };

            var typ2 = new ServiceType
            {
                Name = NAME2
            };

            await _Context.Database.ExecuteSqlCommandAsync("delete from ServiceTypes");
            _Context.ServiceTypes.AddRange(typ1, typ2);
            await _Context.SaveChangesAsync();

            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync($"api/serviceTypes/{typ1.Id}");

            var type = JsonConvert.DeserializeObject<ServiceType>(await r.Content.ReadAsStringAsync());

            r.EnsureSuccessStatusCode();
            Assert.NotNull(type);
            Assert.Equal(NAME1, type.Name);
        }
    }
}