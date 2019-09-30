using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Common.ServiceTypes;
using Common.ServiceTypes.ServiceAttributes;
using Infrastructure.DataModel;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.Interception;
using Infrastructure.Services.EventServiceModels.ServiceAttributes;

namespace Common.EventServiceModels.ServiceAttributes
{
    public class ServiceAttributeServiceInterceptor : InterceptingMappingBase, IServiceAttributeService
    {
        private readonly ServiceAttributeValidator _attributeValidator;
        private readonly EventServiceModelValidator _eventServiceModelValidator;

        public ServiceAttributeServiceInterceptor(ServiceAttributeValidator attributeValidator, EventServiceModelValidator eventServiceModelValidator)
        {
            _attributeValidator = attributeValidator;
            _eventServiceModelValidator = eventServiceModelValidator;

            BuildUp(new Dictionary<string, Action<IInvocation>>
                    {
                        {
                            nameof(UpdateAttributeAsync),
                            x => UpdateAttributeAsync((ServiceAttribute) x.Arguments[0])
                        },
                        {
                            nameof(GetAllAttributesForServiceAsync),
                            x => GetAllAttributesForServiceAsync((int) x.Arguments[0])
                        },
                        {
                            nameof(GetSingleAttributeAsync),
                            x => GetSingleAttributeAsync((int) x.Arguments[0], (int) x.Arguments[1])
                        }
                    });
        }
        
        public Task<ServiceAttribute> UpdateAttributeAsync(ServiceAttribute attr)
        {
            _attributeValidator.ValidateAttributeExists(attr.EventServiceModelId,attr.ServiceAttributeSpecificationId);
            _eventServiceModelValidator.CanUpdateEventServiceModel(attr.EventServiceModelId);
            
            return null;
        }

        public Task<IEnumerable<ServiceAttribute>> GetAllAttributesForServiceAsync(int serviceId)
        {
            _eventServiceModelValidator.ValidateEventServiceModelExists(serviceId);

            return null;
        }

        public Task<ServiceAttribute> GetSingleAttributeAsync(int serviceId, int specId)
        {
            _eventServiceModelValidator.ValidateEventServiceModelExists(serviceId);
            _attributeValidator.ValidateAttributeExists(serviceId,specId);

            return null;
        }
    }
}