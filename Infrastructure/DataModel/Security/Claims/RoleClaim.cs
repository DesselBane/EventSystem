using System.Security.Claims;

namespace Infrastructure.DataModel.Security.Claims
{
    public class RoleClaim : Claim
    {
        #region Const

        public const string ROLE_CLAIM_TYPE = "http://EventSystem/Claims/Security/Role";

        #endregion

        #region Constructors

        public RoleClaim(RoleClaimTypes value, string issuer = "")
            : this(value, issuer, "")
        {
        }

        public RoleClaim(RoleClaimTypes value, string issuer, string originalIssuer, ClaimsIdentity subject = null)
            : base(ROLE_CLAIM_TYPE, ((int) value).ToString(), ClaimValueTypes.Integer, issuer, originalIssuer, subject)
        {
        }

        #endregion
    }
}