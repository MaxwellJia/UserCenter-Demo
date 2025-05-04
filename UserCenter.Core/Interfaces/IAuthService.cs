using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.API.DTOs.Auth;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Entities;

namespace UserCenter.Core.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterRespondDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<LoginRespondDto> LoginAsync(LoginUserDto userDto);
    }
}
