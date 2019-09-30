using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace Infrastructure.DataModel.Security
{
    [DataContract]
    public class UserClaim
    {
        #region Properties

        [DataMember]
        public int Id { get; set; }

        [ForeignKey(nameof(User_Id))]
        public virtual User User { get; set; }

        [DataMember]
        public virtual int User_Id { get; set; }

        [DataMember]
        public virtual string Issuer { get; set; }

        [DataMember]
        public virtual string OriginalIssuer { get; set; }

        [DataMember]
        [Required]
        public virtual string Type { get; set; }

        [DataMember]
        [Required]
        public virtual string Value { get; set; }

        [DataMember]
        public virtual string ValueType { get; set; }

        #endregion

        public Claim ToClaim()
        {
            return new Claim(Type, Value, ValueType, Issuer, OriginalIssuer);
        }

        public static explicit operator Claim(UserClaim userClaim)
        {
            return userClaim.ToClaim();
        }

        public static implicit operator UserClaim(Claim claim)
        {
            return FromClaim(claim);
        }

        public static UserClaim FromClaim(Claim claim)
        {
            return new UserClaim
            {
                Issuer = claim.Issuer,
                OriginalIssuer = claim.OriginalIssuer,
                Type = claim.Type,
                Value = claim.Value,
                ValueType = claim.ValueType
            };
        }
    }
}