using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Events;

namespace Infrastructure.Services.Events
{
    public interface IEventService
    {
        #region Event

        Task<Event> CreateEventAsync(Event newEvent);
        Task<Event> UpdateEventAsync(Event updatedEvent);
        Task DeleteEventAsync(int eventId);
        Task<Event> GetEventAsync(int id);
        Task<IEnumerable<Event>> GetAllEventsForCurrentUserAsync();
        Task UpdateHostAsync(int eventId, int newHostId);

        #endregion
    }
}