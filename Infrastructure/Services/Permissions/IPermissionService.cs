using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Security;

namespace Infrastructure.Services.Permissions
{
    public interface IPermissionService
    {
        
        Task<IEnumerable<UserClaim>> GetPermissionsForUserAsync(int userId);
        Task<UserClaim> GetSingleClaimAsync(int userId, int claimId);
        Task<UserClaim> CreateClaimForUserAsync(int userId, UserClaim claim);
        Task DeleteClaimForUserAsync(int userId, int claimId);
    }
}