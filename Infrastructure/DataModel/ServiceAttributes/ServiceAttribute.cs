using Infrastructure.DataModel.Service;

namespace Infrastructure.DataModel.ServiceAttributes
{
    public class ServiceAttribute
    {
        #region Properties

        public virtual int EventServiceModelId { get; set; }
        public virtual EventServiceModel EventServiceModel { get; set; }
        public virtual int ServiceTypeId { get; set; }
        public virtual int ServiceAttributeSpecificationId { get; set; }
        public virtual ServiceAttributeSpecification ServiceAttributeSpecification { get; set; }
        public virtual string Value { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is ServiceAttribute other))
                return false;

            return other.EventServiceModelId == EventServiceModelId
                   && other.ServiceTypeId == ServiceTypeId
                   && other.ServiceAttributeSpecificationId == ServiceAttributeSpecificationId
                   && other.Value == Value;

        }

        public override int GetHashCode()
        {
            return EventServiceModelId.GetHashCode()
                   + ServiceTypeId.GetHashCode()
                   + ServiceAttributeSpecificationId.GetHashCode()
                   + Value.GetHashCode();
        }
    }
}