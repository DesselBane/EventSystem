using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Common.People;
using Infrastructure.DataModel.Events;
using Infrastructure.Interception;
using Infrastructure.Services.Events;

namespace Common.Events
{
    public class EventServiceInterceptor : InterceptingMappingBase, IEventService
    {
        #region Vars

        private readonly EventValidator _eventValidator;
        private readonly PersonValidator _personValidator;

        #endregion

        #region Constructors

        public EventServiceInterceptor(EventValidator eventValidator, PersonValidator personValidator)
        {
            _eventValidator = eventValidator;
            _personValidator = personValidator;
            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(CreateEventAsync),
                    x => CreateEventAsync((Event) x.Arguments[0])
                },
                {
                    nameof(UpdateEventAsync),
                    x => UpdateEventAsync((Event) x.Arguments[0])
                },
                {
                    nameof(DeleteEventAsync),
                    x => DeleteEventAsync((int) x.Arguments[0])
                },
                {
                    nameof(GetEventAsync),
                    x => GetEventAsync((int) x.Arguments[0])
                },
                {
                    nameof(GetAllEventsForCurrentUserAsync),
                    x => GetAllEventsForCurrentUserAsync()
                },
                {
                    nameof(UpdateHostAsync),
                    x => UpdateHostAsync((int) x.Arguments[0], (int) x.Arguments[1])
                }
            });
        }

        #endregion

        public Task<Event> CreateEventAsync(Event newEvent)
        {
            _eventValidator.ValidateEventProperties(newEvent);
            return null;
        }

        public Task<Event> UpdateEventAsync(Event updatedEvent)
        {
            _eventValidator.ValidateEventExists(updatedEvent.Id);
            _eventValidator.CanUpdateEvent(updatedEvent.Id);
            _eventValidator.ValidateEventProperties(updatedEvent);
            return null;
        }

        public Task DeleteEventAsync(int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _eventValidator.CanDeleteEvent(eventId);
            return null;
        }

        public Task<Event> GetEventAsync(int id)
        {
            _eventValidator.ValidateEventExists(id);
            _eventValidator.CanReadEvent(id);
            return null;
        }

        public Task<IEnumerable<Event>> GetAllEventsForCurrentUserAsync()
        {
            return null;
        }

        public Task UpdateHostAsync(int eventId, int newHostId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _personValidator.ValidatePersonExists(newHostId);
            _eventValidator.CanUpdateHost(eventId);
            return null;
        }

        #region Rules

        #endregion
    }
}