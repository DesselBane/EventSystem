using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTests.AttributeTests
{
    public class ServiceControllerAttributeGetTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task GetAll_NotFound_Service()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/service/999/attributes");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetAll_Success_Undefined()
        {
            var setup = await SetupServiceAsync();

            var spec = await CreateAttributeSpecificationAsync(setup.firstService.TypeId);

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}/attributes");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ServiceAttribute>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(1,result.Count);
            Assert.Equal(spec.Id,result[0].ServiceAttributeSpecificationId);
            Assert.Equal(setup.firstService.Id,result[0].EventServiceModelId);
            Assert.Equal(setup.firstService.TypeId,result[0].ServiceTypeId);
            Assert.Equal("undefined",result[0].Value);
        }

        [Fact]
        public async Task GetAll_Success()
        {
            var setup = await SetupServiceAsync();
            var spec1 = await CreateAttributeSpecificationAsync(setup.firstService.TypeId);
            var spec2 = await CreateAttributeSpecificationAsync(setup.firstService.TypeId);

            var attr1 = await CreateSeriveAttributeAsync(setup.firstService.Id, setup.firstService.TypeId, spec1.Id);
            var attr2 = await CreateSeriveAttributeAsync(setup.firstService.Id, setup.firstService.TypeId, spec2.Id);

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}/attributes");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<ServiceAttribute>>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Contains(attr1,result);
            Assert.Contains(attr2,result);
        }

        [Fact]
        public async Task GetSingle_NotFound_Service()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/service/999/attributes/999");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_Spec()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}/attributes/999");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetSingle_Success_Undefined()
        {
            var setup = await SetupServiceAsync();
            var spec = await CreateAttributeSpecificationAsync(setup.firstService.Id);

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}/attributes/{spec.Id}");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAttribute>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(spec.Id,result.ServiceAttributeSpecificationId);
            Assert.Equal(setup.firstService.TypeId,result.ServiceTypeId);
            Assert.Equal(setup.firstService.Id,result.EventServiceModelId);
            Assert.Equal("undefined",result.Value);
        }

        [Fact]
        public async Task GetSingle_Success()
        {
            var setup = await SetupServiceAsync();
            var spec = await CreateAttributeSpecificationAsync(setup.firstService.Id);
            var attr = await CreateSeriveAttributeAsync(setup.firstService.Id, setup.firstService.TypeId, spec.Id);

            var r = await _Client.GetAsync($"api/service/{setup.firstService.Id}/attributes/{spec.Id}");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAttribute>(await r.Content.ReadAsStringAsync());
            Assert.Equal(attr,result);
        }
    }
}