using System.Security.Claims;

namespace Infrastructure.DataModel.Security.Claims
{
    public class UsernameClaim : Claim
    {
        #region Const

        public const string USERNAME_CLAIM_TYPE = "http://EventSystem/Claims/Security/Username";

        #endregion

        #region Constructors

        public UsernameClaim(string value)
            : base(USERNAME_CLAIM_TYPE, value)
        {
        }

        public UsernameClaim(string value, string valueType)
            : base(USERNAME_CLAIM_TYPE, value, valueType)
        {
        }

        public UsernameClaim(string value, string valueType, string issuer)
            : base(USERNAME_CLAIM_TYPE, value, valueType, issuer)
        {
        }

        public UsernameClaim(string value, string valueType, string issuer, string originalIssuer)
            : base(USERNAME_CLAIM_TYPE, value, valueType, issuer, originalIssuer)
        {
        }

        public UsernameClaim(string value,
                             string valueType,
                             string issuer,
                             string originalIssuer,
                             ClaimsIdentity subject)
            : base(USERNAME_CLAIM_TYPE, value, valueType, issuer, originalIssuer, subject)
        {
        }

        #endregion
    }
}