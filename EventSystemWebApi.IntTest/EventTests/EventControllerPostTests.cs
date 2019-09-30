using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests
{
    public class EventControllerPostTests : EventControllerTestBase
    {
        [Fact]
        public async Task UpdateEvent_NotFound_1()
        {
            var setup = await SetupEventAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Id = setup.firstEvent.Id;

            var r = await _Client.PostAsync("/api/event/999", updatedEvent.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_Forbidden_2()
        {
            await SetupAuthenticationAsync();

            var otherUser = new User
            {
                EMail = "bl@bla.de",
                Password = "faslkgn",
                Salt = "sfkdgjn",
                Person = new RealPerson
                {
                    Firstname = "adflkgn",
                    Lastname = "fslgkj"
                }
            };

            var otherEvent = new Event
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow,
                Name = "blalkfijhn",
                Budget = 200,
                Host = otherUser.Person
            };

            var ctx = CreateDataContext();
            ctx.Events.Add(otherEvent);
            ctx.Users.Add(otherUser);
            ctx.RealPeople.Add(otherUser.Person);
            await ctx.SaveChangesAsync();

            var updatedEvent = CreateValidEvent();
            updatedEvent.Id = -1;

            var r = await _Client.PostAsync($"/api/event/{otherEvent.Id}", updatedEvent.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_HelperCan()
        {
            var setup = await SetupEventAsync();

            var ctx1 = CreateDataContext();

            ctx1.AttendeeRelationships.Add(new AttendeeRelationship
            {
                EventId = setup.secondEvent.Id,
                PersonId = setup.first.Person.Id,
                Type = AttendeeTypes.Helper
            });

            await ctx1.SaveChangesAsync();

            var updatedEvent = CreateValidEvent();
            updatedEvent.Id = -1;

            var r = await _Client.PostAsync($"/api/event/{setup.secondEvent.Id}", updatedEvent.ToStringContent());
            r.EnsureSuccessStatusCode();

            var ctx = CreateDataContext();
            var entity = await ctx.Events.FirstOrDefaultAsync(x => x.Id == setup.secondEvent.Id);

            Assert.Equal(updatedEvent.Budget, entity.Budget);
            Assert.Equal(updatedEvent.End, entity.End);
            Assert.Equal(updatedEvent.Start, entity.Start);
            Assert.Equal(updatedEvent.Name, entity.Name);
        }

        [Fact]
        public async Task UpdateEvent_422_3_InvalidBudget()
        {
            var setup = await SetupAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Budget = -200;

            var r = await _Client.PostAsync($"api/event/{setup.eventItem.Id}", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.BUDGET_GREATER_OR_EQUAL_ZERO), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_422_2_InvalidEndDate()
        {
            var setup = await SetupAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Id = -1;
            updatedEvent.End = new DateTime(1, 1, 1);

            var r = await _Client.PostAsync($"api/event/{setup.eventItem.Id}", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.END_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_422_4_InvalidName()
        {
            var setup = await SetupAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Name = "";
            updatedEvent.Id = -1;

            var r = await _Client.PostAsync($"api/event/{setup.eventItem.Id}", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NAME_REQUIRED), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_422_1_InvalidStartDate()
        {
            var setup = await SetupAsync();
            var updatedEvent = CreateValidEvent();

            updatedEvent.Id = -1;
            updatedEvent.Start = new DateTime(1, 1, 1);

            var r = await _Client.PostAsync($"api/event/{setup.eventItem.Id}", updatedEvent.ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.START_DATE_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_422_StartMustBeBeforeEnd()
        {
            var setup = await SetupEventAsync();
            var newEvent = CreateValidEvent();
            newEvent.End = DateTime.UtcNow;
            newEvent.Start = DateTime.UtcNow.AddDays(1);

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}", newEvent.ToStringContent());

            Assert.Equal((HttpStatusCode)422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.START_MUST_BE_BEFORE_END), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateEvent_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/event/1", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task UpdateEvent_Success()
        {
            var setup = await SetupAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Id = -1;

            var r = await _Client.PostAsync($"/api/event/{setup.eventItem.Id}", updatedEvent.ToStringContent());
            r.EnsureSuccessStatusCode();

            var ctx = CreateDataContext();
            var entity = await ctx.Events.FirstOrDefaultAsync(x => x.Id == setup.eventItem.Id);

            Assert.Equal(updatedEvent.Budget, entity.Budget);
            Assert.Equal(updatedEvent.End, entity.End);
            Assert.Equal(updatedEvent.Start, entity.Start);
            Assert.Equal(updatedEvent.Name, entity.Name);
            Assert.Equal(updatedEvent.Description, entity.Description);
        }

        [Fact]
        public async Task UpdateEvent_Success_Result()
        {
            var setup = await SetupAsync();
            var updatedEvent = CreateValidEvent();
            updatedEvent.Id = -1;

            var r = await _Client.PostAsync($"/api/event/{setup.eventItem.Id}", updatedEvent.ToStringContent());
            r.EnsureSuccessStatusCode();

            var entity = JsonConvert.DeserializeObject<Event>(await r.Content.ReadAsStringAsync());

            Assert.Equal(updatedEvent.Budget, entity.Budget);
            Assert.Equal(updatedEvent.End, entity.End);
            Assert.Equal(updatedEvent.Start, entity.Start);
            Assert.Equal(updatedEvent.Name, entity.Name);
            Assert.Equal(updatedEvent.Description, entity.Description);
        }

        [Fact]
        public async Task UpdateHost_Forbidden_2()
        {
            var setup = await SetupAsync();
            var otherUser = new User
            {
                EMail = Guid.NewGuid() + "@a.de",
                Person = new RealPerson
                {
                    Firstname = "adkfjn",
                    Lastname = "sadlkjgnh"
                }
            };
            _Context.Users.Add(otherUser);
            _Context.RealPeople.Add(otherUser.Person);
            await _Context.SaveChangesAsync();
            setup.eventItem.HostId = otherUser.Person.Id;
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.eventItem.Id}/updatehost/{setup.host.Person.Id}", null);

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_UPDATE_HOST_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateHost_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/event/updatehost?eventId=0&newHostId=0", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task UpdateHost_NotFound_1_Event()
        {
            var setup = await SetupAsync();

            var r = await _Client.PostAsync($"api/event/888/updatehost/{setup.host.Person.Id}",
                null);

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateHost_NotFound_2_Host()
        {
            var setup = await SetupAsync();
            var r = await _Client.PostAsync($"api/event/{setup.eventItem.Id}/updatehost/888", null);

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateHost_Success()
        {
            var setup = await SetupAsync();
            var otherUser = new User
            {
                EMail = Guid.NewGuid() + "@a.de",
                Person = new RealPerson
                {
                    Firstname = "adkfjn",
                    Lastname = "sadlkjgnh"
                }
            };

            _Context.Users.Add(otherUser);
            _Context.RealPeople.Add(otherUser.Person);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync(
                $"api/event/{setup.eventItem.Id}/updateHost/{otherUser.Person.Id}", null);
            r.EnsureSuccessStatusCode();

            await _Context.Entry(setup.eventItem).ReloadAsync();
            Assert.Equal(otherUser.Person.Id, setup.eventItem.HostId);
        }

        [Fact]
        public async Task UpdateEvent_422_0_NoData()
        {
            var setup = await SetupEventAsync();
            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}