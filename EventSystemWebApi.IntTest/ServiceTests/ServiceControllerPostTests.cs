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
    public class ServiceControllerPostTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task PostService_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/service/1", "".ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task PostSerivce_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/service/1", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task PostService_NotFound_1_ServiceType()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}", new EventServiceModel
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
        public async Task PostService_NotFound_2_ServiceModel()
        {
            var types = await CreateDefaultTypesAsync();
            await SetupAuthenticationAsync();

            var r = await _Client.PostAsync("api/service/999", new EventServiceModel
            {
                Profile = Guid.NewGuid().ToString(),
                Salary = 1000,
                TypeId = types.djType.Id
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PostService_Forbidden_2_UserCantUpdate()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupServiceAsync();

            var r = await _Client.PostAsync($"api/service/{setup.secodnService.Id}", new EventServiceModel
            {
                Profile = Guid.NewGuid().ToString(),
                Salary = 1000,
                TypeId = types.djType.Id
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE), error.ErrorCode);
        }

        [Fact]
        public async Task PostService_422_1_SalaryMustBeGreaterOrEqualToZero()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupServiceAsync();

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}", new EventServiceModel
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
        public async Task PostService_Success_Result()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupServiceAsync();
            var model = new EventServiceModel
            {
                Id = -1,
                PersonId = -1,
                Profile = Guid.NewGuid().ToString(),
                Salary = 10008,
                TypeId = types.catererType.Id
            };

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}", model.ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<EventServiceModel>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.Equal(model.Profile, result.Profile);
            Assert.Equal(model.Salary, result.Salary);
            Assert.Equal(model.TypeId, result.TypeId);
            Assert.Equal(setup.firstService.Id, result.Id);
            Assert.Equal(setup.firstService.PersonId, result.PersonId);
        }

        [Fact]
        public async Task PostService_Success_Database()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupServiceAsync();
            var model = new EventServiceModel
            {
                Id = -1,
                PersonId = -1,
                Profile = Guid.NewGuid().ToString(),
                Salary = 10008,
                TypeId = types.catererType.Id
            };

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}", model.ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = await CreateDataContext().EventService.FirstOrDefaultAsync(x => x.Id == setup.firstService.Id);

            Assert.NotNull(result);
            Assert.Equal(model.Profile, result.Profile);
            Assert.Equal(model.Salary, result.Salary);
            Assert.Equal(model.TypeId, result.TypeId);
            Assert.Equal(setup.firstService.Id, result.Id);
            Assert.Equal(setup.firstService.PersonId, result.PersonId);
        }
    }
}