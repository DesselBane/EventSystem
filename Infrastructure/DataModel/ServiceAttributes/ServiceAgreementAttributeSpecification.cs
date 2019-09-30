namespace Infrastructure.DataModel.ServiceAttributes
{
    public class ServiceAgreementAttributeSpecification : ServiceAttributeSpecificationBase
    {
        public override bool Equals(object obj)
        {
            if (!(obj is ServiceAgreementAttributeSpecification))
                return false;
            
            return base.Equals(obj);
        }
    }
}