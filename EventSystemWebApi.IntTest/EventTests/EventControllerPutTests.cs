using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests
{
    public class EventControllerPutTests : EventControllerTestBase
    {
        [Fact]
        public async Task CreateEvent_422_3_InvalidBudget()
        {
            await SetupAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Budget = -200;

            var r = await _Client.PutAsync("api/event/", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.BUDGET_GREATER_OR_EQUAL_ZERO), error.ErrorCode);
        }

        [Fact]
        public async Task CreateEvent_422_2_InvalidEndDate()
        {
            var setup = await SetupAsync();
            var updatedEvent = new Event
            {
                Id = setup.eventItem.Id,
                End = new DateTime(1, 1, 1),
                Name = "asdlkgbnj",
                Budget = 12,
                Start = DateTime.Now
            };

            var r = await _Client.PutAsync("api/event", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.END_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task CreateEvent_422_4_InvalidName()
        {
            await SetupAsync();
            var updatedEvent = new Event
            {
                End = DateTime.Now,
                Budget = 12,
                Start = DateTime.Now
            };

            var r = await _Client.PutAsync("api/event", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NAME_REQUIRED), error.ErrorCode);
        }

        [Fact]
        public async Task CreateEvent_422_1_InvalidStartDate()
        {
            var setup = await SetupAsync();
            var updatedEvent = new Event
            {
                Id = setup.eventItem.Id,
                Start = new DateTime(1, 1, 1),
                End = DateTime.Now,
                Name = "asdlkgbnj",
                Budget = 12
            };

            var r = await _Client.PutAsync("api/event", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(EventErrorCodes.START_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task CreateEvent_422_StartMustBeBeforeEnd()
        {
            await SetupAsync();
            var newEvent = CreateValidEvent();
            newEvent.End = DateTime.UtcNow;
            newEvent.Start = DateTime.UtcNow.AddDays(1);

            var r = await _Client.PutAsync("api/event", newEvent.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.START_MUST_BE_BEFORE_END), error.ErrorCode);
        }

        [Fact]
        public async Task CreateEvent_MustBeAuthenticated()
        {
            var r = await _Client.PutAsync("/api/event", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_Success()
        {
            await SetupAuthenticationAsync();
            var blueprint = new Event
            {
                Budget = null,
                End = new DateTime(2011, 01, 01, 22, 22, 22, DateTimeKind.Utc),
                Start = new DateTime(2011, 01, 01, 22, 22, 22, DateTimeKind.Utc),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString()
            };

            var response = await _Client.PutAsync("/api/event", blueprint.ToStringContent());

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var ctx = CreateDataContext();

            var entity = await ctx.Events.FirstOrDefaultAsync(x => x.Name == blueprint.Name);

            Assert.Equal(blueprint.Budget, entity.Budget);
            Assert.Equal(blueprint.End, entity.End);
            Assert.Equal(blueprint.Start, entity.Start);
            Assert.Equal(blueprint.Description, entity.Description);
        }

        [Fact]
        public async Task CreateEvent_422_0_NoData()
        {
            await SetupAsync();
            var r = await _Client.PutAsync("api/event", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}