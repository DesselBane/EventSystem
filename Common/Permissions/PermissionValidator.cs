using System;
using System.Linq;
using System.Security.Claims;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.ErrorCodes;

namespace Common.Permissions
{
    public class PermissionValidator
    {
        #region Vars

        private readonly DataContext _dataContext;
        private readonly ClaimsIdentity _identity;

        #endregion

        #region Constructors

        public PermissionValidator(DataContext dataContext, ClaimsIdentity identity)
        {
            _dataContext = dataContext;
            _identity = identity;
        }

        #endregion

        

        public void ValidateGetPermissions()
        {
            if (!_identity.HasClaim(x => x.Type == RoleClaim.ROLE_CLAIM_TYPE && x.Value == ((int) RoleClaimTypes.System_Administrator).ToString()))
                throw new ForbiddenException("Current User doesnt have permission to get Permission of others", Guid.Parse(PermissionErrorCodes.NO_GET_PERMISSION));
        }

        public void ValidateClaimExists(int claimId)
        {
            if (!_dataContext.Claims.Any(x => x.Id == claimId))
                throw new NotFoundException(claimId, nameof(UserClaim), Guid.Parse(PermissionErrorCodes.CLAIM_NOT_FOUND));
        }

        public static void ValidateClaim(UserClaim claim)
        {
            switch (claim.Type)
            {
                case RoleClaim.ROLE_CLAIM_TYPE:
                    try
                    {
                        var valueInt = int.Parse(claim.Value);
                        if(valueInt > 1 || valueInt < 0)
                            throw new Exception();
                    }
                    catch (Exception)
                    {
                        throw new UnprocessableEntityException("Provided Claim Value is of the wrong type", Guid.Parse(PermissionErrorCodes.INVALID_CLAIM_VALUE));
                    }
                    break;

                default:
                    throw new UnprocessableEntityException("Provided Claim Type isnt known", Guid.Parse(PermissionErrorCodes.INVALID_CLAIM_TYPE));
            }
        }

        public void ValidateClaimDoesntAlreadyExist(int userId,UserClaim claim)
        {
            if(_dataContext.Claims.Any(x => x.Type == claim.Type && x.Value == claim.Value && x.User_Id == userId))
                throw new ConflictException("Claim already exists",Guid.Parse(PermissionErrorCodes.CLAIM_ALREADY_CREATED));
        }

        public void ValidateUpdatePermissions()
        {
            if (!_identity.HasClaim(x => x.Type == RoleClaim.ROLE_CLAIM_TYPE && x.Value == ((int) RoleClaimTypes.System_Administrator).ToString()))
                throw new ForbiddenException("Current User doesnt have permission to update Permission of others", Guid.Parse(PermissionErrorCodes.NO_UPDATE_PERMISSION));
        }
        
        public void ValidateDeletePermissions()
        {
            if (!_identity.HasClaim(x => x.Type == RoleClaim.ROLE_CLAIM_TYPE && x.Value == ((int) RoleClaimTypes.System_Administrator).ToString()))
                throw new ForbiddenException("Current User doesnt have permission to delete Permission of others", Guid.Parse(PermissionErrorCodes.NO_DELETE_PERMISSION));
        }
    }
}