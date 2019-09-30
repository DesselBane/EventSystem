using System.Runtime.Serialization;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.People;

namespace Infrastructure.DataModel.MapperEntities
{
    [DataContract]
    public class AttendeeRelationship
    {
        #region Properties

        public Event Event { get; set; }

        [DataMember]
        public int EventId { get; set; }

        public Person Person { get; set; }

        [DataMember]
        public int PersonId { get; set; }

        [DataMember]
        public AttendeeTypes Type { get; set; }

        #endregion
    }
}