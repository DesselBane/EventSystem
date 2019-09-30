using System;
using System.Linq;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Design;

namespace Common.Events.ServiceAgreements
{
    public class AgreementValidator
    {
        private readonly DataContext _dataContext;
        private readonly IPersonService _personService;
        private readonly EventValidator _eventValidator;

        public AgreementValidator(DataContext dataContext, IPersonService personService, EventValidator eventValidator)
        {
            _dataContext = dataContext;
            _personService = personService;
            _eventValidator = eventValidator;
        }

        public void ValidateAgreementExists(int eventId, int spsId)
        {
            if(!_dataContext.ServiceAgreements.Any(x => x.EventId == eventId && x.ServiceSlotId == spsId))
                throw new NotFoundException($"EventId: {eventId} && SlotId: {spsId}",nameof(ServiceAgreement),Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND));
        }

        public void ValidateAgreementDoesntExist(int eventId, int spsId)
        {
            if(_dataContext.ServiceAgreements.Any(x => x.EventId == eventId && x.ServiceSlotId == spsId))
                throw new ConflictException("Agreement already exists",Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_ALREADY_EXISTS));
        }

        public void ValidateStartBeforeEnd(ServiceAgreement agreement)
        {
            if(agreement.End < agreement.Start)
                throw new UnprocessableEntityException("Start must be before end",Guid.Parse(ServiceAgreementErrorCodes.AGREEMENT_START_BEFORE_END));
        }

        public void ValidateCanUpdate(ServiceAgreement agreement)
        {
            if(!IsProviderForAgreement(agreement.EventId,agreement.ServiceSlotId))
                throw new ForbiddenException("User doesnt have permission to update Agreement",Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS));
        }

        public void CanProposeAgreement(int eventId, int spsId)
        {
            if(!IsProviderForAgreement(eventId,spsId))
                throw new ForbiddenException("User doesnt have permission to update ServiceAgreement",Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS)); 
        }

        public void CanTraverseToProposal(int eventId, int spsId)
        {
            if(!_dataContext.ServiceAgreements.Any(x => x.EventId == eventId
                                                        && x.ServiceSlotId == spsId
                                                        && x.State == ServiceAgreementStates.Request))
                throw new ConflictException("Cannot set State to Proposal",Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL));
        }

        public void CanAcceptAgreement(int eventId, int spsId)
        {
            try
            {
                _eventValidator.CanUpdateEvent(eventId);
            }
            catch (Exception e)
            {
                throw new ForbiddenException("User doesnt have permission to update ServiceAgreement", Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS),e);
            }
        }

        public void CanTraverseToAccepted(int eventId, int spsId)
        {
            if(!_dataContext.ServiceAgreements.Any(x => x.EventId == eventId
                                                        && x.ServiceSlotId == spsId
                                                        && x.State == ServiceAgreementStates.Proposal))
                throw new ConflictException("Cannot set State to Accepted",Guid.Parse(ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL));
        }

        public void CanDeclineAgreement(int eventId, int spsId)
        {
            try
            {
                if(!IsProviderForAgreement(eventId,spsId))
                    _eventValidator.CanUpdateEvent(eventId);
            }
            catch 
            {
                throw new ForbiddenException("User doesnt have permission to decline agreement",Guid.Parse(ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS));
            }
        }

        public bool IsProviderForAgreement(int eventId, int spsId)
        {
            var user = _personService.GetPersonForUserAsync().Result;

            return (from agreement in _dataContext.ServiceAgreements
                         join serviceModel in _dataContext.EventService on agreement.EventServiceModelId equals serviceModel.Id
                         where agreement.EventId == eventId
                               && agreement.ServiceSlotId == spsId
                               && serviceModel.PersonId == user.Id
                         select agreement).Count() != 0;
        }

        public void ValidateCanUpdateValues(int agreementEventId, int agreementServiceSlotId)
        {
            if(!_dataContext.ServiceAgreements.Any(x => x.EventId == agreementEventId 
                                                        && x.ServiceSlotId == agreementServiceSlotId
                                                        && x.State == ServiceAgreementStates.Request))
                throw new ConflictException("Can only update Agreements in Request State",Guid.Parse(ServiceAgreementErrorCodes.CANNOT_UPDATE_VALUES));
        }
    }
}