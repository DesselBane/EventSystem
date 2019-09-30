using System;
using System.Linq;
using System.Security.Claims;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.ErrorCodes;

namespace Common.Users
{
    public class UserValidator
    {
        private readonly DataContext _dataContext;
        private readonly ClaimsIdentity _identity;

        public UserValidator(DataContext dataContext, ClaimsIdentity identity)
        {
            _dataContext = dataContext;
            _identity = identity;
        }
        
        public void ValidateUserExists(int userId)
        {
            if (!_dataContext.Users.Any(x => x.Id == userId))
                throw new NotFoundException(userId, nameof(User), Guid.Parse(UserErrorCodes.USER_NOT_FOUND));
        }

        public void ValidateGetPermissions()
        {
            if (!_identity.HasClaim(x => x.Type == RoleClaim.ROLE_CLAIM_TYPE && x.Value == ((int) RoleClaimTypes.System_Administrator).ToString()))
                throw new ForbiddenException("Current User doesnt have permission to get user infos", Guid.Parse(UserErrorCodes.NO_GET_PERMISSIONS));
        }
    }
}