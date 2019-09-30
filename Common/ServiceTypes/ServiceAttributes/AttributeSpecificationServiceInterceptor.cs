using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.Interception;
using Infrastructure.Services.ServiceTypes;

namespace Common.ServiceTypes.ServiceAttributes
{
    public class AttributeSpecificationServiceInterceptor : InterceptingMappingBase, IAttributeSpecificationService
    {
        private readonly ServiceTypeValidator _serviceTypeValidator;
        private readonly AttributeSpecificationValidator _attributeSpecificationValidator;

        public AttributeSpecificationServiceInterceptor(ServiceTypeValidator serviceTypeValidator, AttributeSpecificationValidator attributeSpecificationValidator)
        {
            _serviceTypeValidator = serviceTypeValidator;
            _attributeSpecificationValidator = attributeSpecificationValidator;
            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(CreateAttributeSpecificationAsync),
                    x => CreateAttributeSpecificationAsync((ServiceAttributeSpecification) x.Arguments[0])
                },
                {
                    nameof(GetAllSpecificationsForTypeAsync),
                    x => GetAllSpecificationsForTypeAsync((int) x.Arguments[0])
                },
                {
                    nameof(UpdateAttributeSpecificationAsync),
                    x => UpdateAttributeSpecificationAsync((ServiceAttributeSpecification) x.Arguments[0])
                },
                {
                    nameof(DeleteSpecificationAsync),
                    x => DeleteSpecificationAsync((int) x.Arguments[0],(int) x.Arguments[1])
                },
                {
                    nameof(GetSingleSpecificationAsync),
                    x => GetSingleSpecificationAsync((int) x.Arguments[0],(int) x.Arguments[1])
                }
            });
        }

        public Task<ServiceAttributeSpecification> CreateAttributeSpecificationAsync(ServiceAttributeSpecification spec)
        {
            _serviceTypeValidator.ValidateTypeExists(spec.ServiceTypeId);
            _serviceTypeValidator.CanUpdateServiceType();
            
            AttributeSpecificationValidator.ValidateNameIsSet(spec);
            AttributeSpecificationValidator.ValidateSpecTypeIsSet(spec);

            return null;
        }

        public Task<IEnumerable<ServiceAttributeSpecification>> GetAllSpecificationsForTypeAsync(int typeId)
        {
            _serviceTypeValidator.ValidateTypeExists(typeId);

            return null;
        }

        public Task<ServiceAttributeSpecification> UpdateAttributeSpecificationAsync(ServiceAttributeSpecification spec)
        {
            _serviceTypeValidator.ValidateTypeExists(spec.ServiceTypeId);
            _attributeSpecificationValidator.ValidateSpecificationExists(spec.Id,spec.ServiceTypeId);
            _serviceTypeValidator.CanUpdateServiceType();
            
            AttributeSpecificationValidator.ValidateNameIsSet(spec);
            AttributeSpecificationValidator.ValidateSpecTypeIsSet(spec);
            
            return null;
        }

        public Task DeleteSpecificationAsync(int specId, int typeId)
        {
            _serviceTypeValidator.ValidateTypeExists(typeId);
            _attributeSpecificationValidator.ValidateSpecificationExists(specId,typeId);
            _serviceTypeValidator.CanUpdateServiceType();

            return null;
        }

        public Task<ServiceAttributeSpecification> GetSingleSpecificationAsync(int specId, int typeId)
        {
            _serviceTypeValidator.ValidateTypeExists(typeId);
            _attributeSpecificationValidator.ValidateSpecificationExists(specId,typeId);

            return null;
        }
    }
}