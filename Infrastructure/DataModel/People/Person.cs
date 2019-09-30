using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Infrastructure.DataModel.MapperEntities;

namespace Infrastructure.DataModel.People
{
    [DataContract]
    public abstract class Person
    {
        #region Properties

        [DataMember]
        public virtual string Firstname { get; set; }

        [DataMember]
        public virtual string Lastname { get; set; }

        public virtual byte[] ProfilePicture { get; set; }

        [NotMapped]
        [DataMember]
        public string ProfilePictureUrl => $"/api/person/{Id}/picture";

        public virtual List<AttendeeRelationship> EventPersonRelationships { get; protected set; } = new List<AttendeeRelationship>();

        [DataMember]
        public virtual int Id { get; set; }

        #endregion
    }
}