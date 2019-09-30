using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    public class EventControllerGetTests : EventControllerTestBase
    {
        [Fact]
        public async Task GetEvent_AsGuest()
        {
            var setup = await SetupEventRelationshipAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}");
            r.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetEvent_AsHelper()
        {
            var setup = await SetupEventRelationshipAsync();

            var relationship = await _Context.AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == setup.secondEvent.Id && x.PersonId == setup.firstUser.Person.Id);
            relationship.Type = AttendeeTypes.Helper;
            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync($"api/event/{setup.secondEvent.Id}");
            r.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetEvent_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/event/0");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetEvent_Forbidden_1()
        {
            await SetupAuthenticationAsync();

            var newEvent = new Event
            {
                Name = "sadlkgjn",
                Budget = 200,
                End = DateTime.Now,
                Start = DateTime.Now,
                Host = new RealPerson
                {
                    Firstname = "slrgkfj",
                    Lastname = "lksdng",
                    User = new User
                    {
                        EMail = Guid.NewGuid() + "@a.de"
                    }
                }
            };

            var ctx = CreateDataContext();
            ctx.Events.Add(newEvent);
            ctx.Users.Add(newEvent.Host.User);
            ctx.RealPeople.Add(newEvent.Host);
            await ctx.SaveChangesAsync();

            var r = await _Client.GetAsync($"/api/event/{newEvent.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task GetEvent_NotFound_1()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/event/888");
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetEvent_Success()
        {
            var setup = await SetupAsync();

            var r = await _Client.GetAsync($"/api/event/{setup.eventItem.Id}");
            r.EnsureSuccessStatusCode();

            var rEvent = JsonConvert.DeserializeObject<Event>(await r.Content.ReadAsStringAsync());

            Assert.Equal(setup.eventItem.Id, rEvent.Id);
            Assert.Equal(setup.eventItem.HostId, rEvent.HostId);
            Assert.Equal(setup.eventItem.Budget, rEvent.Budget);
            Assert.Equal(setup.eventItem.Name, rEvent.Name);
            Assert.Equal(setup.eventItem.End, rEvent.End);
            Assert.Equal(setup.eventItem.Start, rEvent.Start);
        }

        [Fact]
        public async Task GetEventOverview_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/event");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetEventOverview_Success()
        {
            var setup = await SetupAsync();
            var firstname = Guid.NewGuid().ToString();
            var lastname = Guid.NewGuid().ToString();

            var otherUser = new User
            {
                EMail = Guid.NewGuid() + "@a.de",
                Person = new RealPerson
                {
                    Firstname = firstname,
                    Lastname = lastname
                }
            };
            var guestEvent = new Event
            {
                Name = "guest",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow,
                Budget = 0,
                Host = otherUser.Person
            };
            var helperEvent = new Event
            {
                Name = "helper",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow,
                Budget = 0,
                Host = otherUser.Person
            };
            var noEvent = new Event
            {
                Name = "ERROR",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow,
                Budget = 0,
                Host = otherUser.Person
            };

            var helperRelation = new AttendeeRelationship
            {
                Event = helperEvent,
                PersonId = setup.host.Person.Id,
                Type = AttendeeTypes.Helper
            };

            var guestRelation = new AttendeeRelationship
            {
                Event = guestEvent,
                PersonId = setup.host.Person.Id,
                Type = AttendeeTypes.Guest
            };

            var ctx = CreateDataContext();
            ctx.Users.Add(otherUser);
            ctx.RealPeople.Add(otherUser.Person);
            ctx.Events.Add(guestEvent);
            ctx.Events.Add(helperEvent);
            ctx.Events.Add(noEvent);
            ctx.AttendeeRelationships.AddRange(helperRelation, guestRelation);

            await ctx.SaveChangesAsync();

            var r = await _Client.GetAsync("api/event");
            r.EnsureSuccessStatusCode();

            var events = JsonConvert.DeserializeObject<Event[]>(await r.Content.ReadAsStringAsync()).ToList();

            Assert.Equal(3, events.Count);
            Assert.NotNull(events.FirstOrDefault(x => x.Id == setup.eventItem.Id));
            Assert.NotNull(events.FirstOrDefault(x => x.Id == helperEvent.Id));
            Assert.NotNull(events.FirstOrDefault(x => x.Id == guestEvent.Id));
        }
    }
}