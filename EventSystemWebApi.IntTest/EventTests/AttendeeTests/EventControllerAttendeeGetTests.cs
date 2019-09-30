using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.ErrorCodes;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests.AttendeeTests
{
    public class EventControllerAttendeeGetTests : EventControllerTestBase
    {
        [Fact]
        public async Task GetAll_NotFound_1_Event()
        {
            await SetupEventRelationshipAsync();

            var r = await _Client.GetAsync("api/event/999/attendees");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task GetAll_Success()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/attendees");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<AttendeeRelationship>>(await r.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Equal(setup.secondHelperAtFirst.EventId, result[0].EventId);
            Assert.Equal(setup.secondHelperAtFirst.PersonId, result[0].PersonId);
            Assert.Equal(setup.secondHelperAtFirst.Type, result[0].Type);
        }

        [Fact]
        public async Task GetAll_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/event/1/attendees");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetAll_Forbidden_1_UserDoesntHavePermission()
        {
            var setup = await SetupEventAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/attendees");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_GET_RELATIONSHIP), error.ErrorCode);
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_1_Event()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.GetAsync($"api/event/999/attendees/{setup.secondHelperAtFirst.PersonId}");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_2_Person()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/attendees/999");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task GetSingle_NotFound_3_Relationship()
        {
            var setup = await SetupEventAsync();
            var r = await _Client.GetAsync($"api/event/{setup.firstEvent.Id}/attendees/{setup.secondUser.Person.Id}");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND), error.ErrorCode);
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        [Fact]
        public async Task GetSingle_Forbidden_1_CannotRead()
        {
            var setup = await SetupEventRelationshipAsync();
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            _Context.AttendeeRelationships.Add(new AttendeeRelationship
            {
                EventId = setup.secondEvent.Id,
                PersonId = user.Person.Id,
                Type = AttendeeTypes.Guest
            });
            _Context.AttendeeRelationships.Remove(setup.firstGuestAtSecond);
            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/attendees/{user.Person.Id}");

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_GET_RELATIONSHIP), error.ErrorCode);
            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);
        }

        [Fact]
        public async Task GetSingle_Success()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}/attendees/{setup.firstUser.Person.Id}");
            r.EnsureSuccessStatusCode();

            var relationship = JsonConvert.DeserializeObject<AttendeeRelationship>(await r.Content.ReadAsStringAsync());

            Assert.Equal(setup.firstGuestAtSecond.EventId, relationship.EventId);
            Assert.Equal(setup.firstGuestAtSecond.PersonId, relationship.PersonId);
            Assert.Equal(setup.firstGuestAtSecond.Type, relationship.Type);
        }
    }
}