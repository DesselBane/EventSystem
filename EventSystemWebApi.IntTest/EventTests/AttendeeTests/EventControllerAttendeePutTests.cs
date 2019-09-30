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
    public class EventControllerAttendeePutTests : EventControllerTestBase
    {
        [Fact]
        public async Task Attendee_Helper_Success()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}", new AttendeeRelationship {EventId = -1, PersonId = -1, Type = AttendeeTypes.Helper}.ToStringContent());

            r.EnsureSuccessStatusCode();

            var dbRel = await CreateDataContext().AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == setup.firstEvent.Id && x.PersonId == setup.secondUser.Person.Id);

            Assert.NotNull(dbRel);
            Assert.Equal(AttendeeTypes.Helper, dbRel.Type);
        }

        [Fact]
        public async Task Attendee_Guest_Success()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}", new AttendeeRelationship {EventId = -1, PersonId = -1, Type = AttendeeTypes.Guest}.ToStringContent());

            r.EnsureSuccessStatusCode();

            var dbRel = await CreateDataContext().AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == setup.firstEvent.Id && x.PersonId == setup.secondUser.Person.Id);

            Assert.NotNull(dbRel);
            Assert.Equal(AttendeeTypes.Guest, dbRel.Type);
            Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_Helper_Forbidden_5_RandomUser()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PutAsync($"api/event/{setup.secondEvent.Id}/attendees/{setup.first.Person.Id}", new AttendeeRelationship {EventId = -1, PersonId = -1, Type = AttendeeTypes.Helper}.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_HELPER), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_Helper_Forbidden_5_Helper()
        {
            var setup = await SetupEventAsync();
            var attRel = new AttendeeRelationship {PersonId = setup.first.Person.Id, EventId = setup.secondEvent.Id, Type = AttendeeTypes.Helper};
            _Context.AttendeeRelationships.Add(attRel);
            await _Context.SaveChangesAsync();
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.PutAsync($"api/event/{setup.secondEvent.Id}/attendees/{user.Person.Id}", new AttendeeRelationship {EventId = -1, PersonId = -1, Type = AttendeeTypes.Helper}.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_HELPER), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_Guest_Forbidden_4_RandomUser()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PutAsync($"api/event/{setup.secondEvent.Id}/attendees/{setup.first.Person.Id}", new AttendeeRelationship {EventId = -1, PersonId = -1, Type = AttendeeTypes.Guest}.ToStringContent());

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_GUEST), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_NotFound_1_Event()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PutAsync($"api/event/999/attendees/{setup.secondUser.Person.Id}", new AttendeeRelationship {EventId = setup.secondEvent.Id, PersonId = -1, Type = AttendeeTypes.Helper}.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_NotFound_2_Person()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/attendees/999", new AttendeeRelationship {EventId = -1, PersonId = setup.first.Person.Id, Type = AttendeeTypes.Helper}.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_MustBeAuthenticated()
        {
            var r = await _Client.PutAsync("api/event/1/attendees/1", new AttendeeRelationship {EventId = 1, PersonId = 1, Type = AttendeeTypes.Guest}.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task Attendee_422_0_NoData()
        {
            var setup = await SetupEventAsync();
            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/attendees/999", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_Conflict_1_PersonAlreadyMapped()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PutAsync($"api/event/{setup.secondHelperAtFirst.EventId}/attendees/{setup.secondHelperAtFirst.PersonId}", new AttendeeRelationship {EventId = setup.secondHelperAtFirst.EventId, PersonId = setup.secondHelperAtFirst.PersonId, Type = AttendeeTypes.Helper}.ToStringContent());

            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.PERSON_IS_ALREADY_MAPPED_TO_EVENT), error.ErrorCode);
        }

        [Fact]
        public async Task Attendee_Conflict_2_CannotAddHostAsGuest()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.PutAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.firstUser.Person.Id}", new AttendeeRelationship
            {
                EventId = -1,
                PersonId = -1,
                Type = AttendeeTypes.Helper
            }.ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.CANNOT_ADD_HOST_AS_GUEST), error.ErrorCode);
            Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
        }
    }
}