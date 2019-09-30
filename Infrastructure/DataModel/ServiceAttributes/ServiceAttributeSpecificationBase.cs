using System.Runtime.Serialization;
using System.Security.Cryptography;
using Infrastructure.DataModel.Service;

namespace Infrastructure.DataModel.ServiceAttributes
{
    [DataContract]
    public abstract class ServiceAttributeSpecificationBase
    {
        #region Properties

        [DataMember]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual int ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }

        [DataMember]
        public virtual string AttributeType { get; set; }
        [DataMember]
        public virtual string Name { get; set; }
        [DataMember]
        public virtual string Description { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is ServiceAttributeSpecificationBase other))
                return false;

            return other.Id == Id && other.ServiceTypeId == ServiceTypeId && other.AttributeType == AttributeType && other.Name == Name && other.Description == Description;
        }

        public override int GetHashCode()
        {
            return (Name + Description + AttributeType + Id + ServiceTypeId + GetType().Name).GetHashCode();
        }
    }
}