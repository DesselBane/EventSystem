using System;
using System.Linq;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.ErrorCodes;

namespace Common.ServiceTypes.ServiceAttributes
{
    public class AttributeSpecificationValidator
    {
        private readonly DataContext _dataContext;

        public AttributeSpecificationValidator(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public static void ValidateNameIsSet(ServiceAttributeSpecification spec)
        {
            if(string.IsNullOrWhiteSpace(spec.Name))
                throw new UnprocessableEntityException($"'{nameof(ServiceAttributeSpecification.Name)}' must be set",Guid.Parse(AttributeSpecificationErrorCodes.NAME_MUST_BE_SET));
        }

        public static void ValidateSpecTypeIsSet(ServiceAttributeSpecification spec)
        {
            if(string.IsNullOrWhiteSpace(spec.AttributeType))
                throw new UnprocessableEntityException($"'{nameof(ServiceAttributeSpecification.AttributeType)}' must be set",Guid.Parse(AttributeSpecificationErrorCodes.SPEC_TYPE_MUST_BE_SET));
        }

        public void ValidateSpecificationExists(int specId, int typeId)
        {
            if(!_dataContext.ServiceAttributeSpecifications.Any(x => x.Id == specId && x.ServiceTypeId == typeId))
                throw new NotFoundException(specId,nameof(ServiceAttributeSpecification),Guid.Parse(AttributeSpecificationErrorCodes.NOT_FOUND));
        }
    }
}