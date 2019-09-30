using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.Services;
using Infrastructure.Services.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Events
{
    public class EventService : IEventService
    {
        #region Vars

        private readonly DataContext _eventContext;
        private readonly IPersonService _personService;

        #endregion

        #region Constructors

        public EventService(IPersonService personService, DataContext eventContext)
        {
            _personService = personService;
            _eventContext = eventContext;
        }

        #endregion

        #region Impl

        public static void UpdateEvent(Event target, Event source)
        {
            target.Start = source.Start;
            target.End = source.End;
            target.Name = source.Name;
            target.Budget = source.Budget;
            target.Description = source.Description;
        }

        #endregion

        #region Implementation of IEventService

        public async Task<Event> CreateEventAsync(Event newEvent)
        {
            var Event = new Event
            {
                HostId = (await _personService.GetPersonForUserAsync()).Id
            };

            UpdateEvent(Event, newEvent);

            _eventContext.Events.Add(Event);
            await _eventContext.SaveChangesAsync();
            return Event;
        }

        public async Task<Event> UpdateEventAsync(Event updatedEvent)
        {
            var dbEvent = await _eventContext.Events.FirstOrDefaultAsync(x => x.Id == updatedEvent.Id);

            UpdateEvent(dbEvent, updatedEvent);
            await _eventContext.SaveChangesAsync();

            return dbEvent;
        }

        public async Task DeleteEventAsync(int eventId)
        {
            var dbEvent = await _eventContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            _eventContext.Events.Remove(dbEvent);
            await _eventContext.SaveChangesAsync();
        }

        public async Task<Event> GetEventAsync(int id)
        {
            var dbEvent = await _eventContext.Events
                                             .FirstOrDefaultAsync(x => x.Id == id);

            return dbEvent;
        }

        public async Task<IEnumerable<Event>> GetAllEventsForCurrentUserAsync()
        {
            var person = await _personService.GetPersonForUserAsync();

            return await _eventContext.Events
                                      .Include(x => x.AttendeeRelationships)
                                      .Where(x => x.HostId == person.Id
                                                  || x.AttendeeRelationships.Any(y => y.PersonId == person.Id))
                                      .ToListAsync();
        }

        public async Task UpdateHostAsync(int eventId, int newHostId)
        {
            var dbEvent = await _eventContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);

            dbEvent.HostId = newHostId;
            await _eventContext.SaveChangesAsync();
        }

        #endregion Implementation of IEventService
    }
}