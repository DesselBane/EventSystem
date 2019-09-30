using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Common.Events.ServiceSlots;
using Common.EventServiceModels;
using Infrastructure.DataModel.Events;
using Infrastructure.Interception;
using Infrastructure.Services.Events;

namespace Common.Events.ServiceAgreements
{
    public class AgreementServiceInterceptor : InterceptingMappingBase, IAgreementService
    {
        private readonly AgreementValidator _agreementValidator;
        private readonly EventValidator _eventValidator;
        private readonly ServiceSlotValidator _serviceSlotValidator;
        private readonly EventServiceModelValidator _eventServiceModelValidator;

        public AgreementServiceInterceptor(AgreementValidator agreementValidator, EventValidator eventValidator, ServiceSlotValidator serviceSlotValidator,EventServiceModelValidator eventServiceModelValidator)
        {
            _agreementValidator = agreementValidator;
            _eventValidator = eventValidator;
            _serviceSlotValidator = serviceSlotValidator;
            _eventServiceModelValidator = eventServiceModelValidator;
            BuildUp(new Dictionary<string, Action<IInvocation>>
                    {
                        {
                            nameof(GetSingleServiceAgreementByEventSlot),
                            x => GetSingleServiceAgreementByEventSlot((int) x.Arguments[0], (int) x.Arguments[1])
                        },
                        {
                            nameof(GetAllServiceAgreementsByEvent),
                            x => GetAllServiceAgreementsByEvent((int) x.Arguments[0])
                        },
                        {
                            nameof(CreateServiceAgreement),
                            x => CreateServiceAgreement((int) x.Arguments[0],(int) x.Arguments[1], (int) x.Arguments[2])
                        },
                        {
                            nameof(UpdateServiceAgreement),
                            x => UpdateServiceAgreement((ServiceAgreement) x.Arguments[0])
                        },
                        {
                            nameof(DeleteAgreement),
                            x => DeleteAgreement((int) x.Arguments[0], (int) x.Arguments[1])
                        },
                        {
                            nameof(ProposeAgreement),
                            x => ProposeAgreement((int) x.Arguments[0],(int) x.Arguments[1])
                        },
                        {
                            nameof(AcceptAgreement),
                            x => AcceptAgreement((int) x.Arguments[0],(int) x.Arguments[1])
                        },
                        {
                            nameof(DeclineAgreement),
                            x => DeclineAgreement((int) x.Arguments[0],(int) x.Arguments[1])
                        }
                    });
        }
        
        
        public Task<ServiceAgreement> GetSingleServiceAgreementByEventSlot(int eventId, int spsId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, spsId);
            _agreementValidator.ValidateAgreementExists(eventId,spsId);
            
            _eventValidator.CanReadEvent(eventId);
            
            return null;
        }

        public Task<IEnumerable<ServiceAgreement>> GetAllServiceAgreementsByEvent(int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _eventValidator.CanReadEvent(eventId);

            return null;
        }

        public Task<ServiceAgreement> CreateServiceAgreement(int eventId, int spsId, int serviceId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, spsId);
            _agreementValidator.ValidateAgreementDoesntExist(eventId,spsId);
            _eventServiceModelValidator.ValidateEventServiceModelExists(serviceId);
            
            _eventValidator.CanUpdateEvent(eventId);

            return null;
        }

        public Task<ServiceAgreement> UpdateServiceAgreement(ServiceAgreement agreement)
        {
            _eventValidator.ValidateEventExists(agreement.EventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(agreement.EventId, agreement.ServiceSlotId);
            _agreementValidator.ValidateAgreementExists(agreement.EventId,agreement.ServiceSlotId);
            
            _agreementValidator.ValidateCanUpdate(agreement);
            _agreementValidator.ValidateCanUpdateValues(agreement.EventId,agreement.ServiceSlotId);
            _agreementValidator.ValidateStartBeforeEnd(agreement);
            
            return null;
        }

        public Task DeleteAgreement(int eventId, int spsId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, spsId);
            _agreementValidator.ValidateAgreementExists(eventId,spsId);
            
            _eventValidator.CanUpdateEvent(eventId);
            
            return null;
        }

        public Task<ServiceAgreement> ProposeAgreement(int eventId, int spsId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, spsId);
            _agreementValidator.ValidateAgreementExists(eventId,spsId);

            _agreementValidator.CanProposeAgreement(eventId, spsId);
            _agreementValidator.CanTraverseToProposal(eventId, spsId);

            return null;
        }

        public Task<ServiceAgreement> AcceptAgreement(int eventId, int spsId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, spsId);
            _agreementValidator.ValidateAgreementExists(eventId,spsId);

            _agreementValidator.CanAcceptAgreement(eventId, spsId);
            _agreementValidator.CanTraverseToAccepted(eventId, spsId);

            return null;
        }

        public Task<ServiceAgreement> DeclineAgreement(int eventId, int spsId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _serviceSlotValidator.ValidateServiceProviderSlotExists(eventId, spsId);
            _agreementValidator.ValidateAgreementExists(eventId,spsId);

            _agreementValidator.CanDeclineAgreement(eventId, spsId);

            return null;
        }
    }
}