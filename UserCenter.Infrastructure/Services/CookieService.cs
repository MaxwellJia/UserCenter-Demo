using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserCenter.Core.Configuration;
using UserCenter.Core.Interfaces;

namespace UserCenter.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly JwtSettings _jwtSettings;

        public CookieService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public void AppendAuthTokenCookie(HttpResponse response, string token)
        {
            response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // 本地开发设为 false
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

        public ClaimsPrincipal? ValidateToken(HttpRequest request)
        {
            var token = request.Cookies["token"];
            if (string.IsNullOrEmpty(token)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // 不允许时间偏差
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}

