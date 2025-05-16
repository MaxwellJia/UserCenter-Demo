using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Entities;

namespace UserCenter.Core.Interfaces
{
    public interface IUserService
    {
        // Task<IEnumerator<ApplicationUser>> GetAllUsersAsync();
        // Task<ApplicationUser?> GetUserByIdAsync(int id);
        // Task<ApplicationUser> CreateUserAsync(ApplicationUser applicationUser);
        Task<(SaveChangesRespondDto, string)> UpdateUserAsync(SaveChangesRequestDto saveChangesRequestDto);
        // Task<bool> DeleteUserAsync(int id);
    }
}
