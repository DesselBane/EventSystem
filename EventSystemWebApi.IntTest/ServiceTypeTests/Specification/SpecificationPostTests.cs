using System;
using System.Net;
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
    public class SpecificationPostTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task UpdateSpec_NotFound_ServiceType()
        {
            await SetupServiceTypeAdminAsync();
            var r = await _Client.PostAsync("api/serviceTypes/999/spec/999", CreateValidSpecification().ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateSpec_Forbidden()
        {
            await SetupAuthenticationAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);
            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}/spec/{spec.Id}", CreateValidSpecification().ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateSpec_422_NameMustBeSet()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);
            spec.Name = null;

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}/spec/{spec.Id}", spec.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.NAME_MUST_BE_SET), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateSpec_422_TypeMustBeSet()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);
            spec.AttributeType = null;

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}/spec/{spec.Id}", spec.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.SPEC_TYPE_MUST_BE_SET), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateSpec_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            
            var r = await _Client.PostAsync("api/serviceTypes/999/spec/999", "".ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateSpec_Success_Result()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var oldSpec = await CreateValidSpecificationAsync(type.Id);
            var spec = CreateValidSpecification();
            spec.Id = -1;
            spec.ServiceTypeId = -1;

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}/spec/{oldSpec.Id}", spec.ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAttributeSpecification>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(type.Id,result.ServiceTypeId);
            Assert.Equal(spec.AttributeType,result.AttributeType);
            Assert.Equal(spec.Description,result.Description);
            Assert.Equal(spec.Name,result.Name);
        }
        
        [Fact]
        public async Task UpdateSpec_Success_Database()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var oldSpec = await CreateValidSpecificationAsync(type.Id);
            var spec = CreateValidSpecification();
            spec.Id = -1;
            spec.ServiceTypeId = -1;

            var r = await _Client.PostAsync($"api/serviceTypes/{type.Id}/spec/{oldSpec.Id}", spec.ToStringContent());
            r.EnsureSuccessStatusCode();
            
            var result = await CreateDataContext().ServiceAttributeSpecifications.FirstOrDefaultAsync(x => x.Name == spec.Name);
            var json = JsonConvert.DeserializeObject<ServiceAttributeSpecification>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(json);
            
            Assert.NotNull(result);
            Assert.Equal(result.Id,json.Id);

            Assert.Equal(type.Id,result.ServiceTypeId);
            Assert.Equal(spec.AttributeType,result.AttributeType);
            Assert.Equal(spec.Description,result.Description);
            Assert.Equal(spec.Name,result.Name);
        }
    }
}