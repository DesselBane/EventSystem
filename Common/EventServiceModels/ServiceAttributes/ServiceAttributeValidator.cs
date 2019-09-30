using System.Linq;
using Common.ServiceTypes.ServiceAttributes;
using Infrastructure.DataModel;

namespace Common.EventServiceModels.ServiceAttributes
{
    public class ServiceAttributeValidator
    {
        private readonly DataContext _dataContext;
        private readonly EventServiceModelValidator _eventServiceModelValidator;
        private readonly AttributeSpecificationValidator _attributeSpecificationValidator;

        public ServiceAttributeValidator(DataContext dataContext, EventServiceModelValidator eventServiceModelValidator, AttributeSpecificationValidator attributeSpecificationValidator)
        {
            _dataContext = dataContext;
            _eventServiceModelValidator = eventServiceModelValidator;
            _attributeSpecificationValidator = attributeSpecificationValidator;
        }

        public void ValidateAttributeExists(int serviceId, int specId)
        {
            _eventServiceModelValidator.ValidateEventServiceModelExists(serviceId);

            var service = _dataContext.EventService.First(x => x.Id == serviceId);
            
            _attributeSpecificationValidator.ValidateSpecificationExists(specId,service.TypeId);
        }
    }
}