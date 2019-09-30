using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTests
{
    public class ServiceControllerPutTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task PutService_MustBeAuthenticated()
        {
            var r = await _Client.PutAsync("api/service", new EventServiceModel().ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task PutService_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PutAsync("api/service", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task PutService_NotFound_1_ServiceType()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.PutAsync("api/service", new EventServiceModel
            {
                Profile = Guid.NewGuid().ToString(),
                Salary = 1000,
                TypeId = 999
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PutService_422_1_SalaryCantBeNegativ()
        {
            var types = await CreateDefaultTypesAsync();
            await SetupAuthenticationAsync();

            var r = await _Client.PutAsync("api/service", new EventServiceModel
            {
                Profile = Guid.NewGuid().ToString(),
                Salary = -1,
                TypeId = types.djType.Id
            }.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SALARY_MUST_BE_GREATER_OR_EQUAL_ZERO), error.ErrorCode);
        }

        [Fact]
        public async Task PutService_Created_Success_Result()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupAuthenticationAsync();

            var model = new EventServiceModel
            {
                Id = -1,
                PersonId = -1,
                Profile = Guid.NewGuid().ToString(),
                Salary = 1000,
                TypeId = types.djType.Id
            };

            var r = await _Client.PutAsync("api/service", model.ToStringContent());

            Assert.Equal(HttpStatusCode.Created, r.StatusCode);

            var result = JsonConvert.DeserializeObject<EventServiceModel>(await r.Content.ReadAsStringAsync());
            var dbEntry = await CreateDataContext().EventService.FirstOrDefaultAsync(x => x.Profile == model.Profile);

            Assert.NotNull(dbEntry);
            Assert.Equal(dbEntry.Id, result.Id);

            Assert.NotNull(result);
            Assert.Equal(model.Profile, result.Profile);
            Assert.Equal(model.Salary, result.Salary);
            Assert.Equal(model.TypeId, result.TypeId);
            Assert.Equal(setup.Person.Id, result.PersonId);
        }

        [Fact]
        public async Task PutService_Created_Success_Database()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupAuthenticationAsync();

            var model = new EventServiceModel
            {
                Id = -1,
                PersonId = -1,
                Profile = Guid.NewGuid().ToString(),
                Salary = 1000,
                TypeId = types.djType.Id
            };

            var r = await _Client.PutAsync("api/service", model.ToStringContent());

            Assert.Equal(HttpStatusCode.Created, r.StatusCode);

            var result = await CreateDataContext().EventService.FirstOrDefaultAsync(x => x.Profile == model.Profile);

            Assert.NotNull(result);
            Assert.Equal(model.Profile, result.Profile);
            Assert.Equal(model.Salary, result.Salary);
            Assert.Equal(model.TypeId, result.TypeId);
            Assert.Equal(setup.Person.Id, result.PersonId);
        }
    }
}