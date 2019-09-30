using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.ServiceTypeTests.Specification
{
    public class SpecificationDeleteTests : ServiceTypeControllerTestBase
    {
        [Fact]
        public async Task DeleteSpec_NotFound_Type()
        {
            await SetupServiceTypeAdminAsync();

            var r = await _Client.DeleteAsync("api/serviceTypes/999/spec/999");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteSpec_NotFound_Spec()
        {
            await SetupServiceTypeAdminAsync();
            var tpye = await CreateTypeAsync();

            var r = await _Client.DeleteAsync($"api/serviceTypes/{tpye.Id}/spec/999");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttributeSpecificationErrorCodes.NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteSpec_Forbidden()
        {
            await SetupAuthenticationAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);

            var r = await _Client.DeleteAsync($"api/serviceTypes/{type.Id}/spec/{spec.Id}");
            
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteSpec_Success()
        {
            await SetupServiceTypeAdminAsync();
            var type = await CreateTypeAsync();
            var spec = await CreateValidSpecificationAsync(type.Id);

            var r = await _Client.DeleteAsync($"api/serviceTypes/{type.Id}/spec/{spec.Id}");

            r.EnsureSuccessStatusCode();
            
            Assert.Null(await CreateDataContext().ServiceAttributeSpecifications.FirstOrDefaultAsync(x => x.Id == spec.Id && x.ServiceTypeId == type.Id));
        }
    }
}