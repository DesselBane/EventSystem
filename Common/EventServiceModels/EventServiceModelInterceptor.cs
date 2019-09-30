using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Common.ServiceTypes;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Service;
using Infrastructure.Interception;
using Infrastructure.Services.EventServiceModels;

namespace Common.EventServiceModels
{
    public class EventServiceModelInterceptor : InterceptingMappingBase, IEventServiceModelService
    {
        #region Vars

        private readonly ServiceTypeValidator _serviceTypeValidator;
        private readonly EventServiceModelValidator _serviceValidator;

        #endregion

        #region Constructors

        public EventServiceModelInterceptor(EventServiceModelValidator serviceValidator, ServiceTypeValidator serviceTypeValidator)
        {
            _serviceValidator = serviceValidator;
            _serviceTypeValidator = serviceTypeValidator;

            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(GetServiceByIdAsync),
                    x => GetServiceByIdAsync((int) x.Arguments[0])
                },
                {
                    nameof(CreateEventServiceModelAsync),
                    x => CreateEventServiceModelAsync((EventServiceModel) x.Arguments[0])
                },
                {
                    nameof(UpdateEventServiceModelAsync),
                    x => UpdateEventServiceModelAsync((EventServiceModel) x.Arguments[0])
                },
                {
                    nameof(DeleteEventServiceModelAsync),
                    x => DeleteEventServiceModelAsync((int) x.Arguments[0])
                },
                {
                    nameof(GetServiceByTypeIdAsync),
                    x => GetServiceByTypeIdAsync((int) x.Arguments[0])
                }
            });
        }

        #endregion

        public Task<EventServiceModel> GetServiceByIdAsync(int serviceId)
        {
            _serviceValidator.ValidateEventServiceModelExists(serviceId);
            return null;
        }

        public Task<EventServiceModel> CreateEventServiceModelAsync(EventServiceModel serviceModel)
        {
            _serviceTypeValidator.ValidateTypeExists(serviceModel.TypeId);


            _serviceValidator.ValidateEventServiceModelSalary(serviceModel);
            return null;
        }

        public Task<EventServiceModel> UpdateEventServiceModelAsync(EventServiceModel serviceModel)
        {
            _serviceTypeValidator.ValidateTypeExists(serviceModel.TypeId);
            _serviceValidator.ValidateEventServiceModelExists(serviceModel.Id);

            _serviceValidator.CanUpdateEventServiceModel(serviceModel.Id);

            _serviceValidator.ValidateEventServiceModelSalary(serviceModel);
            return null;
        }

        public Task DeleteEventServiceModelAsync(int serviceModelId)
        {
            _serviceValidator.ValidateEventServiceModelExists(serviceModelId);
            _serviceValidator.CanDeleteEventServiceModel(serviceModelId);
            return null;
        }

        public Task<IEnumerable<EventServiceModel>> GetServiceForCurrentUser()
        {
            // Not mapped since its not needed
            return null;
        }

        public Task<IEnumerable<EventServiceModel>> GetServiceByTypeIdAsync(int typeId)
        {
            _serviceTypeValidator.ValidateTypeExists(typeId);

            return null;
        }

        public Task<IEnumerable<ServiceAgreement>> GetAgreementsForCurrentUser()
        {
            // Not mapped since its not needed
            return null;
        }
    }
}