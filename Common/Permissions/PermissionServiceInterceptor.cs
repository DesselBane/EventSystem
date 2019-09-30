using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Common.Users;
using Infrastructure.DataModel.Security;
using Infrastructure.Interception;
using Infrastructure.Services.Permissions;

namespace Common.Permissions
{
    public class PermissionServiceInterceptor : InterceptingMappingBase, IPermissionService
    {
        private readonly PermissionValidator _permissionValidator;
        private readonly UserValidator _userValidator;

        public PermissionServiceInterceptor(PermissionValidator permissionValidator, UserValidator userValidator)
        {
            _permissionValidator = permissionValidator;
            _userValidator = userValidator;
            
            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(GetPermissionsForUserAsync),
                    x => GetPermissionsForUserAsync((int) x.Arguments[0])
                },
                {
                    nameof(GetSingleClaimAsync),
                    x => GetSingleClaimAsync((int) x.Arguments[0],(int) x.Arguments[1])
                },
                {
                    nameof(CreateClaimForUserAsync),
                   x => CreateClaimForUserAsync((int) x.Arguments[0],(UserClaim) x.Arguments[1]) 
                },
                {
                    nameof(DeleteClaimForUserAsync),
                    x => DeleteClaimForUserAsync((int) x.Arguments[0], (int) x.Arguments[1])
                }
            });
        }
        
        #region Implementation of IPermissionService

        public Task<IEnumerable<UserClaim>> GetPermissionsForUserAsync(int userId)
        {
            _userValidator.ValidateUserExists(userId);
            _permissionValidator.ValidateGetPermissions();

            return null;
        }

        public Task<UserClaim> GetSingleClaimAsync(int userId, int claimId)
        {
            _userValidator.ValidateUserExists(userId);
            _permissionValidator.ValidateClaimExists(claimId);
            _permissionValidator.ValidateGetPermissions();
            return null;
        }

        public Task<UserClaim> CreateClaimForUserAsync(int userId, UserClaim claim)
        {
            _userValidator.ValidateUserExists(userId);
            PermissionValidator.ValidateClaim(claim);
            _permissionValidator.ValidateClaimDoesntAlreadyExist(userId,claim);
            _permissionValidator.ValidateUpdatePermissions();
            
            return null;
        }

        public Task DeleteClaimForUserAsync(int userId, int claimId)
        {
            _userValidator.ValidateUserExists(userId);
            _permissionValidator.ValidateClaimExists(claimId);
            _permissionValidator.ValidateDeletePermissions();
            
            return null;
        }

        #endregion
    }
}