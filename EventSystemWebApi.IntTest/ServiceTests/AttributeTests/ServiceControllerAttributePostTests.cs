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

namespace EventSystemWebApi.IntTest.ServiceTests.AttributeTests
{
    public class ServiceControllerAttributePostTests : ServiceControllerTestBase
    {
        [Fact]
        public async Task PostAttribute_NotFound_Service()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.PostAsync("api/service/999/attributes/999",CreateSeriveAttribute().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PostAttribute_NotFound_Spec()
        {
            var setup = await SetupServiceAsync();

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}/attributes/999",CreateSeriveAttribute().ToStringContent());
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task PostAttribute_Forbidden()
        {
            var setup = await SetupServiceAsync();
            var spec = await CreateAttributeSpecificationAsync(setup.secodnService.TypeId);

            var r = await _Client.PostAsync($"api/service/{setup.secodnService.Id}/attributes/{spec.Id}", CreateSeriveAttribute().ToStringContent());
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE), error.ErrorCode);
        }

        [Fact]
        public async Task PostAttribute_Success_Result()
        {
            var setup = await SetupServiceAsync();
            var spec = await CreateAttributeSpecificationAsync(setup.firstService.TypeId);
            var newAttr = CreateSeriveAttribute();

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}/attributes/{spec.Id}", newAttr.ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ServiceAttribute>(await r.Content.ReadAsStringAsync());
            
            Assert.NotNull(result);
            Assert.Equal(newAttr.Value,result.Value);
        }
        
        [Fact]
        public async Task PostAttribute_Success_Database()
        {
            var setup = await SetupServiceAsync();
            var spec = await CreateAttributeSpecificationAsync(setup.firstService.TypeId);
            var attr = await CreateSeriveAttributeAsync(setup.firstService.Id, setup.firstService.TypeId, spec.Id);
            var newAttr = CreateSeriveAttribute();

            var r = await _Client.PostAsync($"api/service/{setup.firstService.Id}/attributes/{spec.Id}", newAttr.ToStringContent());
            r.EnsureSuccessStatusCode();

            var result = await CreateDataContext().ServiceAttributes.FirstOrDefaultAsync(x => x.EventServiceModelId == setup.firstService.Id && x.ServiceAttributeSpecificationId == spec.Id && x.ServiceTypeId == setup.firstService.TypeId);
            
            Assert.NotNull(result);
            Assert.Equal(newAttr.Value,result.Value);
        }

        [Fact]
        public async Task PostAttibute_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/service/999/attributes/999", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}