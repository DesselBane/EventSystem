using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.ServiceSlotTests
{
    public class EventControllerSpsPutTests : EventControllerTestBase
    {
        [Fact]
        public async Task AddServiceSlot_NotFound_1_Event()
        {
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = setup.firstEvent.Id,
                TypeId = 1
            };

            var r = await _Client.PutAsync("api/event/999/sps", sps.ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var answer = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), answer.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_Forbidden_2()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = -1,
                TypeId = types.djType.Id
            };

            var r = await _Client.PutAsync($"api/event/{setup.secondEvent.Id}/sps", sps.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_MustBeAuthenticated()
        {
            var r = await _Client.PutAsync("api/event/1/sps", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task AddServiceSlot_422_1_StartBeforeEnd()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = -1,
                TypeId = types.djType.Id,
                End = DateTime.UtcNow,
                Start = DateTime.UtcNow.AddDays(1)
            };

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps", sps.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.START_MUSE_BE_BEFORE_END), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_422_StartNoSqlDate()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var slot = new ServiceSlot
            {
                Start = new DateTime(1, 1, 1),
                TypeId = types.djType.Id
            };

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps", slot.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.START_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_422_EndNoSqlDate()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var slot = new ServiceSlot
            {
                End = new DateTime(1, 1, 1),
                TypeId = types.djType.Id
            };

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps", slot.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.END_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_Success()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = -1,
                TypeId = types.djType.Id
            };

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps", sps.ToStringContent());
            r.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        }

        [Fact]
        public async Task AddServiceSlot_NotFound_3_Type()
        {
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = -1,
                TypeId = 999
            };

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps", sps.ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_422_0_NoData()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/sps", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}