using System;
using System.Runtime.Serialization;
using Infrastructure.DataModel.Service;

namespace Infrastructure.DataModel.Events
{
    [DataContract]
    public class ServiceAgreement
    {
        #region Properties

        [DataMember]
        public int EventId { get; set; }
        [DataMember]
        public int ServiceSlotId { get; set; }
        [DataMember]
        public int EventServiceModelId { get; set; }

        public ServiceSlot ServiceSlot { get; set; }
        public EventServiceModel EventServiceModel { get; set; }
        
        [DataMember]
        public int Cost { get; set; }
        [DataMember]
        public DateTime Start { get; set; }
        [DataMember]
        public DateTime End { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public ServiceAgreementStates State { get; set; }

        #endregion
    }
}