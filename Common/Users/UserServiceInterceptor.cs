using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.Security;
using Infrastructure.Interception;
using Infrastructure.Services;

namespace Common.Users
{
    public class UserServiceInterceptor : InterceptingMappingBase,IUserService
    {
        private readonly UserValidator _userValidator;

        public UserServiceInterceptor(UserValidator userValidator)
        {
            _userValidator = userValidator;
            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(GetManageableUsersAsync),
                    x => GetManageableUsersAsync()
                },
                {
                    nameof(GetSingleUserAsync),
                    x => GetSingleUserAsync((int) x.Arguments[0])
                }
            });
        }
        
        #region Implementation of IUserService

        public Task<IEnumerable<User>> GetManageableUsersAsync()
        {
            return null;
        }

        public Task<User> GetSingleUserAsync(int userId)
        {
            _userValidator.ValidateUserExists(userId);
            _userValidator.ValidateGetPermissions();
            
            return null;
        }

        #endregion
    }
}