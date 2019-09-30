using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.AttendeeTests
{
    public class EventControllerAttendeeDeleteTests : EventControllerTestBase
    {
        [Fact]
        public async Task Attendee_NotFound_1_Event()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.DeleteAsync($"api/event/999/attendees/{setup.secondHelperAtFirst.PersonId}");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_NotFound_2_Person()
        {
            var setup = await SetupEventRelationshipAsync();
            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/attendees/999");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_NotFound_3_Relationship()
        {
            var setup = await SetupEventAsync();
            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_Forbidden_2_CannotDeleteRelationship()
        {
            var setup = await SetupEventRelationshipAsync();
            var r = await _Client.DeleteAsync($"api/event/{setup.secondEvent.Id}/attendees/{setup.firstUser.Person.Id}");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_DELETE_RELATIONSHIP), error.ErrorCode);
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_Success()
        {
            var setup = await SetupEventRelationshipAsync();
            var r = await _Client.DeleteAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}");

            r.EnsureSuccessStatusCode();

            var dbRel = await CreateDataContext().AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == setup.firstEvent.Id && x.PersonId == setup.secondUser.Person.Id);

            Assert.Null(dbRel);
        }
    }
}