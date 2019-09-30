using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.ServiceAttributes;

namespace Infrastructure.DataModel.Service
{
    [DataContract]
    public class EventServiceModel
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public decimal Salary { get; set; }

        [DataMember]
        public string Profile { get; set; }

        [DataMember]
        public int TypeId { get; set; }

        public ServiceType Type { get; set; }

        [DataMember]
        public int LocationId { get; set; }

        public Location Location { get; set; } = new Location();

        [DataMember]
        public int PersonId { get; set; }

        public Person Person { get; set; }

        #endregion
    }
}