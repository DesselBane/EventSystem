using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using EventSystemWebApi.IntTest.ServiceTests;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests
{
    public class ServiceControllerTypePostTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task SPTypes_Success()
        {
            await SetupServiceTypeAdminAsync();
            var newName = Guid.NewGuid().ToString();

            var preType = new ServiceType {Name = Guid.NewGuid().ToString()};
            _Context.ServiceTypes.Add(preType);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/serviceTypes/{preType.Id}", new ServiceType {Name = newName, Id = -1}.ToStringContent());
            r.EnsureSuccessStatusCode();

            var dbType = await CreateDataContext().ServiceTypes.FirstOrDefaultAsync(x => x.Id == preType.Id);

            Assert.NotNull(dbType);
            Assert.Equal(newName, dbType.Name);

            var postType = JsonConvert.DeserializeObject<ServiceType>(await r.Content.ReadAsStringAsync());

            Assert.Equal(newName, postType.Name);
        }

        [Fact]
        public async Task SPTypes_Conflict_1_TypeExsits()
        {
            await SetupServiceTypeAdminAsync();

            var name = Guid.NewGuid().ToString();
            var type = new ServiceType {Name = name};
            var otherType = new ServiceType {Name = Guid.NewGuid().ToString()};
            _Context.ServiceTypes.Add(type);
            _Context.ServiceTypes.Add(otherType);

            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/serviceTypes/{otherType.Id}", new ServiceType {Name = name, Id = -1}.ToStringContent());

            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_ALREADY_EXISTS), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_MustBeAuthenticated()
        {
            var type = new ServiceType {Name = Guid.NewGuid().ToString()};
            _Context.ServiceTypes.Add(type);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}", new ServiceType {Name = Guid.NewGuid().ToString(), Id = -1}.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task SPTypes_Forbidden_1()
        {
            await SetupAuthenticationAsync();
            var type = await CreateTypeAsync();

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}", new ServiceType {Id = -1, Name = Guid.NewGuid().ToString()}.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var dbType = await CreateDataContext().ServiceTypes.FirstOrDefaultAsync(x => x.Id == type.Id);
            Assert.NotNull(dbType);
            Assert.Equal(type.Name, dbType.Name);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_NotFound_1_Type()
        {
            await SetupAuthenticationAsync();
            var preType = new ServiceType {Name = Guid.NewGuid().ToString()};
            _Context.ServiceTypes.Add(preType);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("api/serviceTypes/333", new ServiceType {Name = Guid.NewGuid().ToString(), Id = preType.Id}.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_422_1_NameTooLong()
        {
            await SetupServiceTypeAdminAsync();
            var name = Guid.NewGuid() + Guid.NewGuid().ToString() + Guid.NewGuid();
            var type = await CreateTypeAsync();

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}", new ServiceType {Name = name, Id = -1}.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(error);
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.TYPE_NAME_TOO_LONG), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_422_2_NameMustBeSet()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}", new ServiceType {Name = "      ", Id = -1}.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.TYPE_NAME_MUST_BE_SET), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            await CreateTypeAsync();
            var r = await _Client.PostAsync("api/serviceTypes/999", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}