using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.Core.Entities;

namespace UserCenter.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerator<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(int id);
        Task<ApplicationUser> CreateUserAsync(ApplicationUser applicationUser);
        Task<ApplicationUser> UpdateUserAsync(ApplicationUser applicationUser);
        Task<bool> DeleteUserAsync(int id);
    }
}
