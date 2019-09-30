using System;
using System.Runtime.Serialization;

namespace Infrastructure.DataContracts
{
    [DataContract]
    public class TokenDTO
    {
        #region Properties

        [DataMember]
        public string AccessToken { get; set; }

        [DataMember]
        public string RefreshToken { get; set; }

        [DataMember]
        public DateTime AccessToken_ValidUntil { get; set; }

        [DataMember]
        public DateTime RefreshToken_ValidUntil { get; set; }

        #endregion
    }
}