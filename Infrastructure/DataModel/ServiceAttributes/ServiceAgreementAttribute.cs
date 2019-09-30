using Infrastructure.DataModel.Events;

namespace Infrastructure.DataModel.ServiceAttributes
{
    public class ServiceAgreementAttribute
    {
        #region Properties

        public virtual int ServiceSlotId { get; set; }
        public virtual int EventId { get; set; }
        public virtual ServiceAgreement ServiceAgreement { get; set; }

        public virtual int ServiceTypeId { get; set; }
        public virtual int ServiceAgreementAttributeSpecificationId { get; set; }
        public virtual ServiceAgreementAttributeSpecification ServiceAttributeSpecification { get; set; }

        public virtual string Value { get; set; }

        #endregion
    }
}