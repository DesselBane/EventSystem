using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.Misc;
using Infrastructure.Interception;
using Infrastructure.Services.Events;

namespace Common.Events.Locations
{
    public class EventLocationServiceInterceptor : InterceptingMappingBase, IEventLocationService
    {
        #region Vars

        private readonly EventValidator _eventValidator;

        #endregion

        #region Constructors

        public EventLocationServiceInterceptor(EventValidator eventValidator)
        {
            _eventValidator = eventValidator;
            var mappings = new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(GetLocationForEventAsync),
                    x => GetLocationForEventAsync((int) x.Arguments[0])
                },
                {
                    nameof(UpdateLocationForEventAsync),
                    x => UpdateLocationForEventAsync((int) x.Arguments[0], (Location) x.Arguments[1])
                }
            };

            BuildUp(mappings);
        }

        #endregion

        public Task<Location> GetLocationForEventAsync(int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _eventValidator.CanReadEvent(eventId);
            return null;
        }

        public Task<Location> UpdateLocationForEventAsync(int eventId, Location location)
        {
            _eventValidator.ValidateEventExists(eventId);
            _eventValidator.CanUpdateEvent(eventId);
            return null;
        }
    }
}