using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.AttendeeTests
{
    public class EventControllerAttendeePostTests : EventControllerTestBase
    {
        [Fact]
        public async Task Attendee_Forbidden_4_CannotAddGuest()
        {
            var setup = await SetupEventRelationshipAsync();
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            var rel = new AttendeeRelationship
            {
                EventId = setup.secondEvent.Id,
                PersonId = user.Person.Id,
                Type = AttendeeTypes.Helper
            };
            _Context.AttendeeRelationships.Add(rel);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondEvent.Id}/attendees/{user.Person.Id}", new AttendeeRelationship
            {
                EventId = -1,
                PersonId = -1,
                Type = AttendeeTypes.Guest
            }.ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_GUEST), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_Forbidden_5_CannotAddHelper()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstGuestAtSecond.EventId}/attendees/{setup.firstGuestAtSecond.PersonId}", new AttendeeRelationship
            {
                EventId = -1,
                PersonId = -1,
                Type = AttendeeTypes.Helper
            }.ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_HELPER), error.ErrorCode);
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_NotFound_1_Event()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PostAsync($"api/event/999/attendees/{setup.secondHelperAtFirst.PersonId}", new AttendeeRelationship
            {
                Type = AttendeeTypes.Guest
            }.ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_NotFound_2_Person()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PostAsync($"api/event/{setup.secondHelperAtFirst.EventId}/attendees/999", new AttendeeRelationship
            {
                Type = AttendeeTypes.Guest
            }.ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_NotFound_3_Relationship()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}", new AttendeeRelationship
            {
                Type = AttendeeTypes.Helper
            }.ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_Success()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PostAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}", new AttendeeRelationship
            {
                Type = AttendeeTypes.Guest
            }.ToStringContent());

            r.EnsureSuccessStatusCode();
            var ctx = CreateDataContext();

            var dbRelationship = await ctx.AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == setup.firstEvent.Id && x.PersonId == setup.secondUser.Person.Id);

            Assert.NotNull(dbRelationship);
            Assert.Equal(AttendeeTypes.Guest, dbRelationship.Type);
        }

        [Fact]
        public async Task Attendee_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/event/1/attendees/1", "".ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_422_0_NoData()
        {
            await SetupAsync();
            var r = await _Client.PostAsync("api/event/1/attendees/1", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
    }
}