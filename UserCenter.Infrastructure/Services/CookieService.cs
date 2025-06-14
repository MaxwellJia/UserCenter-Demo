using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserCenter.Core.Configuration;
using UserCenter.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace UserCenter.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IWebHostEnvironment _env;

        public CookieService(IOptions<JwtSettings> jwtSettings, IWebHostEnvironment env)
        {
            _jwtSettings = jwtSettings.Value;
            _env = env;
        }

        public void AppendAuthTokenCookie(HttpResponse response, string token)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            };

            if (_env.IsDevelopment())
            {
                options.SameSite = SameSiteMode.Lax;
                options.Secure = false;
            }
            else
            {
                options.SameSite = SameSiteMode.None;
                options.Secure = true;
            }

            response.Cookies.Append("token", token, options);
        }

        public void ExpireAuthTokenCookie(HttpResponse response)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };

            if (_env.IsDevelopment())
            {
                options.SameSite = SameSiteMode.Lax;
                options.Secure = false;
            }
            else
            {
                options.SameSite = SameSiteMode.None;
                options.Secure = true;
            }

            response.Cookies.Append("token", "", options);
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
                    ClockSkew = TimeSpan.Zero
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


