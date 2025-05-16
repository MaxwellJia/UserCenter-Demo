
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserCenter.Core.Configuration;
using UserCenter.Core.Entities;
using UserCenter.Core.Interfaces;
using UserCenter.Infrastructure.Data;
using UserCenter.Infrastructure.Services;

namespace UserCenter.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // 读取连接字符串
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 注册 DbContext
            builder.Services.AddDbContext<UserCenterDbContext>(options =>
                options.UseNpgsql(connectionString));

            // 注册 JWT 认证
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            // 注册 CORS 策略
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000") // 你的前端地址
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // 注册 Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<UserCenterDbContext>()
            .AddDefaultTokenProviders();

            // 必须添加这一行！注册 Controller 服务
            builder.Services.AddControllers();

            // 加上这两行，启用 Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 注册认证 + 授权服务 + 用户相关服务
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUserService, UserService>();

            // 其他 Service 注册...
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // 开发环境启用 Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Allow all frontend access
            app.UseCors("AllowFrontend");

            // Configure the HTTP request pipeline...
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
