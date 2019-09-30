using System;
using System.Linq;
using System.Security.Claims;
using Infrastructure.DataModel.Security.Claims;

namespace Common.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetUsername(this ClaimsIdentity ident)
        {
            if (!ident.IsAuthenticated)
                throw new InvalidOperationException("Identity must be authenticated to get a Username");

            return ident.Claims.FirstOrDefault(x => x.Type == UsernameClaim.USERNAME_CLAIM_TYPE)?.Value;
        }
    }
}