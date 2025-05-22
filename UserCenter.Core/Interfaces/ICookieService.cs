using Microsoft.AspNetCore.Http;

namespace UserCenter.Core.Interfaces
{
    public interface ICookieService
    {
        void AppendAuthTokenCookie(HttpResponse response, string token);
        void ExpireAuthTokenCookie(HttpResponse response);
    }
}
