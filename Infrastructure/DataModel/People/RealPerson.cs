using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Security;

namespace Infrastructure.DataModel.People
{
    [DataContract]
    public class RealPerson : Person
    {
        #region Properties

        public virtual User User { get; set; }

        public virtual int UserId { get; set; }

        public IEnumerable<Event> HostedEvents { get; protected set; } = new List<Event>();

        #endregion
    }
}