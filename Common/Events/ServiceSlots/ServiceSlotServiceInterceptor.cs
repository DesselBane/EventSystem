using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.Events;
using Infrastructure.Interception;
using Infrastructure.Services.Events;

namespace Common.Events.ServiceSlots
{
    public class ServiceSlotServiceInterceptor : InterceptingMappingBase, IServiceSlotService
    {
        #region Vars

        private readonly EventValidator _eventValidator;
        private readonly ServiceSlotValidator _serviceSlotValidator;

        #endregion

        #region Constructors

        public ServiceSlotServiceInterceptor(EventValidator eventValidator, ServiceSlotValidator serviceSlotValidator)
        {
            _eventValidator = eventValidator;
            _serviceSlotValidator = serviceSlotValidator;
            var mappings = new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(AddServiceSlotAsync),
                    x => AddServiceSlotAsync((ServiceSlot) x.Arguments[0])
                },
                {
                    nameof(DeleteServiceSlotAsync),
                    x => DeleteServiceSlotAsync((int) x.Arguments[0], (int) x.Arguments[1])
                },
                {
                    nameof(GetAllServiceSlotsForEventAsync),
                    x => GetAllServiceSlotsForEventAsync((int) x.Arguments[0])
                },
                {
                    nameof(GetServiceSlotAsync),
                    x => GetServiceSlotAsync((int) x.Arguments[0], (int) x.Arguments[1])
                },
                {
                    nameof(UpdateServiceSlotAsync),
                    x => UpdateServiceSlotAsync((ServiceSlot) x.Arguments[0])
                }
            };

            BuildUp(mappings);
        }

        #endregion

        public Task<ServiceSlot> AddServiceSlotAsync(ServiceSlot slot)
        {
            _eventValidator.ValidateEventExists(slot.EventId);
            _serviceSlotValidator.ValidateServiceProviderTypeExists(slot.TypeId);

            _eventValidator.CanUpdateEvent(slot.EventId);

            _serviceSlotValidator.ValidateServiceProviderSlotProperties(slot);
            return null;
        }

        public Task DeleteServiceSlotAsync(int id, int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, id);
            _eventValidator.CanUpdateEvent(eventId);
            return null;
        }

        public Task<IEnumerable<ServiceSlot>> GetAllServiceSlotsForEventAsync(int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _eventValidator.CanReadEvent(eventId);
            return null;
        }

        public Task<ServiceSlot> GetServiceSlotAsync(int id, int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, id);
            _eventValidator.CanReadEvent(eventId);
            return null;
        }

        public Task UpdateServiceSlotAsync(ServiceSlot slot)
        {
            _eventValidator.ValidateEventExists(slot.EventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(slot.EventId, slot.Id);
            _serviceSlotValidator.ValidateServiceProviderTypeExists(slot.TypeId);

            _eventValidator.CanUpdateEvent(slot.EventId);

            _serviceSlotValidator.ValidateServiceProviderSlotProperties(slot);
            return null;
        }
    }
}