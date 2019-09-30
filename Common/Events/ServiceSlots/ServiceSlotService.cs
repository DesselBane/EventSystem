using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.Services.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Events.ServiceSlots
{
    public class ServiceSlotService : IServiceSlotService
    {
        #region Vars

        private readonly DataContext _eventContext;

        #endregion

        #region Constructors

        public ServiceSlotService(DataContext eventContext)
        {
            _eventContext = eventContext;
        }

        #endregion

        public async Task<ServiceSlot> AddServiceSlotAsync(ServiceSlot slot)
        {
            _eventContext.ServiceSlots.Add(slot);
            await _eventContext.SaveChangesAsync();
            return slot;
        }

        public async Task UpdateServiceSlotAsync(ServiceSlot slot)
        {
            var dbSlot = await _eventContext.ServiceSlots.FirstOrDefaultAsync(x => x.Id == slot.Id);
            UpdateSlot(dbSlot, slot);
            await _eventContext.SaveChangesAsync();
        }

        public async Task DeleteServiceSlotAsync(int id, int eventId)
        {
            var slot = await _eventContext.ServiceSlots.FirstOrDefaultAsync(x => x.Id == id);
            _eventContext.ServiceSlots.Remove(slot);
            await _eventContext.SaveChangesAsync();
        }

        public Task<ServiceSlot> GetServiceSlotAsync(int id, int eventId)
        {
            return _eventContext.ServiceSlots.FirstOrDefaultAsync(x => x.Id == id && x.EventId == eventId);
        }

        public async Task<IEnumerable<ServiceSlot>> GetAllServiceSlotsForEventAsync(int eventId)
        {
            return await _eventContext.ServiceSlots
                                      .Where(x => x.EventId == eventId)
                                      .ToListAsync();
        }

        public static void UpdateSlot(ServiceSlot target, ServiceSlot source)
        {
            target.End = source.End;
            target.EventId = source.EventId;
            target.Start = source.Start;
            target.TypeId = source.TypeId;
            target.BudgetTarget = source.BudgetTarget;
        }
    }
}