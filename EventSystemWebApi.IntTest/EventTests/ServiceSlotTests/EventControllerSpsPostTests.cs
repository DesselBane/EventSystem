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
    public class EventControllerSpsPostTests : EventControllerTestBase
    {
        [Fact]
        public async Task UpdateServiceProviderSlot_NotFound_1_Event()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = setup.firstEvent.Id,
                TypeId = types.djType.Id
            };

            _Context.ServiceSlots.Add(sps);
            await _Context.SaveChangesAsync();

            var newSps = new ServiceSlot
            {
                Id = sps.Id,
                EventId = sps.EventId,
                TypeId = types.catererType.Id
            };


            var r = await _Client.PostAsync($"api/event/999/sps/{sps.Id}", newSps.ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var answer = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), answer.ErrorCode);
        }

        [Fact]
        public async Task UpdateServiceProviderSlot_Forbidden_2()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = setup.secondEvent.Id,
                TypeId = types.djType.Id
            };

            _Context.ServiceSlots.Add(sps);
            await _Context.SaveChangesAsync();

            var newSps = new ServiceSlot
            {
                Id = -1,
                EventId = -1,
                TypeId = types.catererType.Id
            };


            var r = await _Client.PostAsync($"api/event/{sps.EventId}/sps/{sps.Id}", newSps.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateServiceProviderSlot_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/event/1/sps/1", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task UpdateServiceProviderSlot_NotFound_4_Slot()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var sps = new ServiceSlot
            {
                Id = setup.firstSlot.Id,
                EventId = setup.firstEvent.Id,
                TypeId = 1
            };

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/999", sps.ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateServiceProviderSlot_422_1_StartBeforeEnd()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = setup.firstEvent.Id,
                TypeId = types.djType.Id
            };

            _Context.ServiceSlots.Add(sps);
            await _Context.SaveChangesAsync();

            var newSps = new ServiceSlot
            {
                Id = -1,
                EventId = -1,
                TypeId = types.catererType.Id,
                End = DateTime.UtcNow,
                Start = DateTime.UtcNow.AddDays(1)
            };


            var r = await _Client.PostAsync($"api/event/{sps.EventId}/sps/{sps.Id}", newSps.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.START_MUSE_BE_BEFORE_END), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_422_StartNoSqlDate()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var slot = new ServiceSlot
            {
                Start = new DateTime(1, 1, 1),
                TypeId = 1
            };

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}", slot.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.START_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task AddServiceSlot_422_EndNoSqlDate()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var slot = new ServiceSlot
            {
                End = new DateTime(1, 1, 1),
                TypeId = 1
            };

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/sps/{setup.firstSlot.Id}", slot.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceSlotErrorCodes.END_DATE_INVALID), error.ErrorCode);
        }


        [Fact]
        public async Task UpdateServiceProviderSlot_Success()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = setup.firstEvent.Id,
                TypeId = types.djType.Id
            };

            _Context.ServiceSlots.Add(sps);
            await _Context.SaveChangesAsync();

            var newSps = new ServiceSlot
            {
                Id = -1,
                EventId = -1,
                TypeId = types.catererType.Id
            };

            var r = await _Client.PostAsync($"api/event/{sps.EventId}/sps/{sps.Id}", newSps.ToStringContent());
            r.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateServiceProviderSlot_NotFound_3_Type()
        {
            var types = await CreateDefaultTypesAsync();
            var setup = await SetupEventAsync();
            var sps = new ServiceSlot
            {
                EventId = setup.firstEvent.Id,
                TypeId = types.djType.Id
            };

            _Context.ServiceSlots.Add(sps);
            await _Context.SaveChangesAsync();

            var newSps = new ServiceSlot
            {
                Id = -1,
                EventId = -1,
                TypeId = 999
            };


            var r = await _Client.PostAsync($"api/event/{sps.EventId}/sps/{sps.Id}", newSps.ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateServiceProviderSlot_422_0_NoData()
        {
            var setup = await SetupEventServiceSlotsAsync();
            var r = await _Client.PostAsync($"api/event/{setup.firstSlot.EventId}/sps/{setup.firstSlot.Id}", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}