using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UserCenter.Core.Interfaces
{
    public interface ICookieService
    {
        void AppendAuthTokenCookie(HttpResponse response, string token);
        void ExpireAuthTokenCookie(HttpResponse response);
        ClaimsPrincipal? ValidateToken(HttpRequest request);
    }
}
