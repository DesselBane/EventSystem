using System.Runtime.Serialization;

namespace Infrastructure.DataModel.Service
{
    [DataContract]
    public class ServiceType
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        #endregion
    }
}