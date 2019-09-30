using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests.Specification
{
    public class SpecificationPutTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task CreateSpec_NotFound_ServiceType()
        {
            await SetupServiceTypeAdminAsync();
            var r = await _Client.PutAsync("api/serviceTypes/999/spec", CreateValidSpecification().ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task CreateSpec_Forbidden()
        {
            await SetupAuthenticationAsync();
            var type = await CreateTypeAsync();
            var r = await _Client.PutAsync($"api/serviceTypes/{type.Id}/spec", CreateValidSpecification().ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task CreateSpec_422_NameMustBeSet()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = CreateValidSpecification();
            spec.Name = null;

            var r = await _Client.PutAsync($"api/serviceTypes/{type.Id}/spec", spec.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.NAME_MUST_BE_SET), error.ErrorCode);
        }

        [Fact]
        public async Task CreateSpec_422_TypeMustBeSet()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = CreateValidSpecification();
            spec.AttributeType = null;

            var r = await _Client.PutAsync($"api/serviceTypes/{type.Id}/spec", spec.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.SPEC_TYPE_MUST_BE_SET), error.ErrorCode);
        }

        [Fact]
        public async Task CreateSpec_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            
            var r = await _Client.PutAsync("api/serviceTypes/999/spec", "".ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task CreateSpec_Success_Result()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = CreateValidSpecification();

            var r = await _Client.PutAsync($"api/serviceTypes/{type.Id}/spec", spec.ToStringContent());

            var result = JsonConvert.DeserializeObject<ServiceAttributeSpecification>(await r.Content.ReadAsStringAsync());
            
            Assert.Equal(HttpStatusCode.Created,r.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(type.Id,result.ServiceTypeId);
            Assert.Equal(spec.AttributeType,result.AttributeType);
            Assert.Equal(spec.Description,result.Description);
            Assert.Equal(spec.Name,result.Name);
        }
        
        [Fact]
        public async Task CreateSpec_Success_Database()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = CreateValidSpecification();

            var r = await _Client.PutAsync($"api/serviceTypes/{type.Id}/spec", spec.ToStringContent());

            var result = await CreateDataContext().ServiceAttributeSpecifications.FirstOrDefaultAsync(x => x.Name == spec.Name);
            var json = JsonConvert.DeserializeObject<ServiceAttributeSpecification>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(json);
            Assert.Equal(result.Id,json.Id);
            
            Assert.Equal(HttpStatusCode.Created,r.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(type.Id,result.ServiceTypeId);
            Assert.Equal(spec.AttributeType,result.AttributeType);
            Assert.Equal(spec.Description,result.Description);
            Assert.Equal(spec.Name,result.Name);
        }
    }
}