using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.People;

namespace Infrastructure.DataModel.Events
{
    [DataContract]
    public class Event
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime Start { get; set; }

        [DataMember]
        public DateTime End { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal? Budget { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int HostId { get; set; }

        public RealPerson Host { get; set; }

        [DataMember]
        public int LocationId { get; set; }

        public Location Location { get; set; } = new Location();

        public List<ServiceSlot> EventServiceSlots { get; protected set; } = new List<ServiceSlot>();

        public virtual List<AttendeeRelationship> AttendeeRelationships { get; protected set; } = new List<AttendeeRelationship>();

        #endregion
    }
}