using System;
using System.Threading.Tasks;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Service;
using Microsoft.EntityFrameworkCore;

namespace EventSystemWebApi.IntTest.EventTests
{
    public abstract class EventControllerTestBase : TestBase
    {
        protected async Task<(User first, Event firstEvent, User secondUser, Event secondEvent)> SetupEventAsync()
        {
            var normal = await SetupAsync();
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var blueprint = CreateValidEvent();

            blueprint.HostId = user.Person.Id;
            _Context.Events.Add(blueprint);
            await _Context.SaveChangesAsync();
            return (normal.host, normal.eventItem, user, blueprint);
        }

        protected Event CreateValidEvent()
        {
            return new Event
                   {
                       Start = new DateTime(2015, 1, 1, 20, 0, 0, DateTimeKind.Utc),
                       End = new DateTime(2015, 1, 1, 23, 0, 0, DateTimeKind.Utc),
                       Name = Guid.NewGuid()
                                  .ToString(),
                       Budget = null,
                       Description = Guid.NewGuid().ToString()
                   };
        }

        protected async Task<(User firstUser, Event firstEvent, ServiceSlot firstSlot, User secondUser, Event secondEvent, ServiceSlot secondSlot)> SetupEventServiceSlotsAsync()
        {
            var setup = await SetupEventAsync();

            var types = await CreateDefaultTypesAsync();

            var sps1 = new ServiceSlot
                       {
                           EventId = setup.firstEvent.Id,
                           TypeId = types.djType.Id
                       };

            var sps2 = new ServiceSlot
                       {
                           EventId = setup.secondEvent.Id,
                           TypeId = types.catererType.Id
                       };

            _Context.ServiceSlots.AddRange(sps1, sps2);
            await _Context.SaveChangesAsync();
            return (setup.first, setup.firstEvent, sps1, setup.secondUser, setup.secondEvent, sps2);
        }

        protected async Task<(User host, Event eventItem)> SetupAsync()
        {
            var user = await SetupAuthenticationAsync();
            var blueprint = new Event
                            {
                                Budget = 100,
                                End = new DateTime(2015, 1, 1, 23, 0, 0, DateTimeKind.Utc),
                                Start = new DateTime(2015, 1, 1, 20, 0, 0, DateTimeKind.Utc),
                                Name = Guid.NewGuid()
                                           .ToString(),
                                HostId = user.Person.Id
                            };

            _Context.Events.Add(blueprint);
            await _Context.SaveChangesAsync();
            return (user, blueprint);
        }

        protected async Task<(User firstUser, Event firstEvent, AttendeeRelationship secondHelperAtFirst, User secondUser, Event secondEvent, AttendeeRelationship firstGuestAtSecond)> SetupEventRelationshipAsync()
        {
            var setup = await SetupEventAsync();
            var secondHelperAtFirst = new AttendeeRelationship
                                      {
                                          EventId = setup.firstEvent.Id,
                                          PersonId = setup.secondUser.Person.Id,
                                          Type = AttendeeTypes.Helper
                                      };

            var firstGuestAtSecond = new AttendeeRelationship
                                     {
                                         EventId = setup.secondEvent.Id,
                                         PersonId = setup.first.Person.Id,
                                         Type = AttendeeTypes.Guest
                                     };

            _Context.AttendeeRelationships.AddRange(secondHelperAtFirst, firstGuestAtSecond);
            await _Context.SaveChangesAsync();
            return (setup.first, setup.firstEvent, secondHelperAtFirst, setup.secondUser, setup.secondEvent, firstGuestAtSecond);
        }

        protected async Task<(ServiceAgreement agreement, EventServiceModel serivce)> SetupServiceAgreementAsync(int eventId, int serviceSlotId, int servicePersonId)
        {
            var serviceSlot = await _Context.ServiceSlots.FirstAsync(x => x.EventId == eventId && x.Id == serviceSlotId);

            var service = new EventServiceModel
                          {
                              Location = new Location(),
                              PersonId = servicePersonId,
                              TypeId = serviceSlot.TypeId
                          };

            _Context.EventService.Add(service);
            await _Context.SaveChangesAsync();

            var item = new ServiceAgreement
                       {
                           EventId = eventId,
                           EventServiceModelId = service.Id,
                           ServiceSlotId = serviceSlotId
                       };

            _Context.ServiceAgreements.Add(item);
            await _Context.SaveChangesAsync();

            return (item, service);
        }
    }
}