namespace Infrastructure.DataModel.ServiceAttributes
{
    public class ServiceAttributeSpecification : ServiceAttributeSpecificationBase
    {
        public override bool Equals(object obj)
        {
            if (!(obj is ServiceAttributeSpecification))
                return false;
            
            return base.Equals(obj);
        }
    }
}