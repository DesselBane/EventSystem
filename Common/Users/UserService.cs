using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Extensions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Common.Users
{
    public class UserService : IUserService
    {
        #region Vars

        private readonly DataContext _dataContext;
        private readonly ClaimsIdentity _identity;

        #endregion

        #region Constructors

        public UserService(DataContext dataContext, ClaimsIdentity identity)
        {
            _dataContext = dataContext;
            _identity = identity;
        }

        #endregion

        public async Task<IEnumerable<User>> GetManageableUsersAsync()
        {
            var user = await _dataContext.Users
                                         .Include(x => x.Claims)
                                         .FirstOrDefaultAsync(x => string.Equals(x.EMail, _identity.GetUsername(), StringComparison.CurrentCultureIgnoreCase));

            List<User> users;

            if (user.HasClaim(RoleClaim.ROLE_CLAIM_TYPE, (int) RoleClaimTypes.System_Administrator))
                users = await _dataContext.Users.Include(x => x.Person).ToListAsync();
            else
                users = new List<User>();

            return users;
        }

        public Task<User> GetSingleUserAsync(int userId)
        {
            return _dataContext.Users.Include(x => x.Person)
                               .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}