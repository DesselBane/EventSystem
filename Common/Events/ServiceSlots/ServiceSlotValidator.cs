using System;
using System.Linq;
using Common.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using JetBrains.Annotations;

namespace Common.Events.ServiceSlots
{
    public class ServiceSlotValidator
    {
        #region Vars

        private readonly DataContext _dataContext;

        #endregion

        #region Constructors

        public ServiceSlotValidator(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        [AssertionMethod]
        public void ValidateServiceProviderSlotProperties(ServiceSlot slot)
        {
            if (slot.End.HasValue && slot.Start.HasValue && slot.End.Value < slot.Start.Value)
                throw new UnprocessableEntityException("Start must be earlier or equal to End", Guid.Parse(ServiceSlotErrorCodes.START_MUSE_BE_BEFORE_END));

            if(slot.End.HasValue && !slot.End.Value.IsValidSqlDate())
                throw new UnprocessableEntityException($"The '{nameof(ServiceSlot.End)}' Property is no valid SqlDate", Guid.Parse(ServiceSlotErrorCodes.END_DATE_INVALID));

            if(slot.Start.HasValue && !slot.Start.Value.IsValidSqlDate())
                throw new UnprocessableEntityException($"The '{nameof(ServiceSlot.Start)}' Property is no valid SqlDate", Guid.Parse(ServiceSlotErrorCodes.START_DATE_INVALID));
        }

        public void ValidateServiceProviderTypeExists(int typeId)
        {
            if (!_dataContext.ServiceTypes.Any(x => x.Id == typeId))
                throw new NotFoundException(typeId, "Service Provider Type", Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND));
        }

        public void ValidateServiceProviderSlotExists(int eventId, int slotId)
        {
            if (!_dataContext.ServiceSlots.Any(x => x.Id == slotId && x.EventId == eventId))
                throw new NotFoundException(slotId, "Service Provider Slot", Guid.Parse(ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND));
        }
    }
}