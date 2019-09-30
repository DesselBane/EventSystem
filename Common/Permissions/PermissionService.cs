using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Extensions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.Services.Permissions;
using Microsoft.EntityFrameworkCore;

namespace Common.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly DataContext _dataContext;
        private readonly ClaimsIdentity _identity;

        public PermissionService(DataContext dataContext, ClaimsIdentity identity)
        {
            _dataContext = dataContext;
            _identity = identity;
        }
        
        #region Implementation of IPermissionService

        

        public async Task<IEnumerable<UserClaim>> GetPermissionsForUserAsync(int userId)
        {
            return await _dataContext.Claims.Where(x => x.User_Id == userId).ToListAsync();
        }

        public Task<UserClaim> GetSingleClaimAsync(int userId, int claimId)
        {
            return _dataContext.Claims.FirstAsync(x => x.Id == claimId);
        }

        public async Task<UserClaim> CreateClaimForUserAsync(int userId, UserClaim claim)
        {
            var dbClaim = UserClaim.FromClaim(new Claim(claim.Type, claim.Value));
            dbClaim.User_Id = userId;
            _dataContext.Claims.Add(dbClaim);
            await _dataContext.SaveChangesAsync();
            return dbClaim;
        }

        public async Task DeleteClaimForUserAsync(int userId, int claimId)
        {
            var claim = await _dataContext.Claims.FirstOrDefaultAsync(x => x.Id == claimId && x.User_Id == userId);
            _dataContext.Claims.Remove(claim);
            await _dataContext.SaveChangesAsync();
        }

        #endregion
    }
}