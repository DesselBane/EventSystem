using System.Runtime.Serialization;

namespace Infrastructure.DataModel.Misc
{
    [DataContract]
    public class Location
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string City { get; set; }
        
        [DataMember]
        public string ZipCode { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Country { get; set; }

        #endregion
    }
}