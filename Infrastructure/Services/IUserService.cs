using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Security;

namespace Infrastructure.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetManageableUsersAsync();
        Task<User> GetSingleUserAsync(int userId);
    }
}