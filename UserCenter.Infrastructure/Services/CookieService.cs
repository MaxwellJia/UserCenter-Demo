using Microsoft.AspNetCore.Http;
using UserCenter.Core.Interfaces;

namespace UserCenter.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        public void AppendAuthTokenCookie(HttpResponse response, string token)
        {
            response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // 本地开发可以设置为 false
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
        }

        public void ExpireAuthTokenCookie(HttpResponse response)
        {
            response.Cookies.Append("token", "", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            });
        }
    }
}
