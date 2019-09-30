using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Events;

namespace Infrastructure.Services.Events
{
    public interface IServiceSlotService
    {
        Task<ServiceSlot> AddServiceSlotAsync(ServiceSlot slot);
        Task DeleteServiceSlotAsync(int id, int eventId);
        Task<IEnumerable<ServiceSlot>> GetAllServiceSlotsForEventAsync(int eventId);
        Task<ServiceSlot> GetServiceSlotAsync(int id, int eventId);
        Task UpdateServiceSlotAsync(ServiceSlot slot);
    }
}