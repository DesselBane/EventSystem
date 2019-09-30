using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests.Specification
{
    public class SpecificationGetTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task GetAll_NotFound_ServiceType()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/serviceTypes/999/spec");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetAll_Success()
        {
            await SetupAuthenticationAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);

            var r = await _Client.GetAsync($"api/serviceTypes/{type.Id}/spec");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ServiceAttributeSpecification>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(1,result.Count);
            Assert.Equal(spec.AttributeType,result[0].AttributeType);
            Assert.Equal(spec.Description,result[0].Description);
            Assert.Equal(spec.Name,result[0].Name);
            Assert.Equal(spec.ServiceTypeId,result[0].ServiceTypeId);
            Assert.Equal(spec.Id,result[0].Id);
        }

        [Fact]
        public async Task GetSingle_NotFound_Type()
        {
            await SetupServiceTypeAdminAsync();

            var r = await _Client.GetAsync("api/serviceTypes/999/spec/999");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_Spec()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();

            var r = await _Client.GetAsync($"api/serviceTypes/{type.Id}/spec/999");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_Success()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);

            var r = await _Client.GetAsync($"api/serviceTypes/{type.Id}/spec/{spec.Id}");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAttributeSpecification>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            
            Assert.Equal(spec,result);
        }
    }
}