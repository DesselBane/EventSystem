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
    public class ServiceControllerTypePutTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task SPTypes_Conflict_1_TypeExists()
        {
            await CreateDefaultTypesAsync();
            
            await SetupAuthenticationAsync();

            var r = await _Client.PutAsync("api/servicetypes", new ServiceType {Name = "DJ"}.ToStringContent());
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_ALREADY_EXISTS), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_422_1_NameTooLong()
        {
            await CreateDefaultTypesAsync();
            await SetupAuthenticationAsync();
            var spType = new ServiceType {Name = new string('A', 51)};

            var r = await _Client.PutAsync("api/servicetypes", spType.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.TYPE_NAME_TOO_LONG), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_Forbidden_1()
        {
            await CreateDefaultTypesAsync();
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            await SetupBasicAuthenticationAsync(_Client, user.EMail);

            var r = await _Client.PutAsync("api/servicetypes", new ServiceType {Name = Guid.NewGuid().ToString()}.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_CREATE_SERVICE_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_Success()
        {
            await CreateDefaultTypesAsync();
            var type = new ServiceType {Name = Guid.NewGuid().ToString()};
            await SetupServiceTypeAdminAsync();

            var r = await _Client.PutAsync("api/servicetypes", type.ToStringContent());
            r.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, r.StatusCode);
            Assert.NotNull(await CreateDataContext().ServiceTypes.FirstOrDefaultAsync(x => x.Name == type.Name));

            var retType = JsonConvert.DeserializeObject<ServiceType>(await r.Content.ReadAsStringAsync());
            Assert.Equal(type.Name, retType.Name);
        }

        [Fact]
        public async Task SPTypes_NotAuthorized()
        {
            
            var r = await _Client.PutAsync("api/servicetypes", new ServiceType {Name = Guid.NewGuid().ToString()}.ToStringContent());
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task SPType_422_2_NoEmptyName()
        {
            await CreateDefaultTypesAsync();
            await SetupAuthenticationAsync();

            var r = await _Client.PutAsync("api/servicetypes", new ServiceType().ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.TYPE_NAME_MUST_BE_SET), error.ErrorCode);
        }

        [Fact]
        public async Task SPTypes_422_0_NoData()
        {
            await CreateDefaultTypesAsync();
            await SetupAuthenticationAsync();
            var r = await _Client.PutAsync("api/servicetypes", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}