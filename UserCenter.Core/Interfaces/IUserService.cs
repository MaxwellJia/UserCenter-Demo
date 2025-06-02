using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.DTOs.User;
using UserCenter.Core.Entities;

namespace UserCenter.Core.Interfaces
{
    public interface IUserService
    {
        Task<(SaveChangesRespondDto, string)> UpdateUserAsync(SaveChangesRequestDto saveChangesRequestDto);

        Task<(List<UserQueryDto> Users, int Total)> FilterUsersAsync(FilterUserDto filter);

        Task<bool> IsUserAdminAsync(string userId);

        Task<bool> SoftDeleteUserAsync(Guid userId);

        Task<bool> UpdateUserAsync(UpdateUserDto dto);
        

    }
}
