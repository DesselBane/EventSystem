using System;
using System.Runtime.Serialization;
using Infrastructure.DataModel.Service;

namespace Infrastructure.DataModel.Events
{
    [DataContract]
    public class ServiceSlot
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public decimal? BudgetTarget { get; set; }

        [DataMember]
        public DateTime? Start { get; set; }

        [DataMember]
        public DateTime? End { get; set; }

        [DataMember]
        public int EventId { get; set; }

        public Event Event { get; set; }

        [DataMember]
        public int TypeId { get; set; }

        public ServiceType Type { get; set; }

        #endregion
    }
}