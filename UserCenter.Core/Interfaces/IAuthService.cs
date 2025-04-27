using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.Core.Entities;

namespace UserCenter.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ApplicationUser?> RegisterAsync(string username, string password);
    }
}
