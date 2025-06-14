
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserCenter.Core.Configuration;
using UserCenter.Core.Entities;
using UserCenter.Core.Interfaces;
using UserCenter.Infrastructure.Data;
using UserCenter.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace UserCenter.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            

            var builder = WebApplication.CreateBuilder(args);

            // debuger
            Console.WriteLine("Environment: " + builder.Environment.EnvironmentName);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine("ConnectionString: " + (connectionString ?? "NULL"));

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            Console.WriteLine("JWT Issuer: " + jwtSettings?.Issuer);
            Console.WriteLine("JWT Secret: " + jwtSettings?.SecretKey);

            
            //var secretKey = jwtSettings?.SecretKey;
           // var issuer = jwtSettings?.Issuer;
            //var audience = jwtSettings?.Audience;



            // Add services to the container.

            // 读取连接字符串
            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // 注册 DbContext
            builder.Services.AddDbContext<UserCenterDbContext>(options =>
    options.UseSqlServer(connectionString)); // ✅ SQL Server 驱动


            // 注册 JWT 认证
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            // 注册 CORS 策略
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("https://agreeable-moss-01b810900.6.azurestaticapps.net", "http://localhost:3000") // 你的前端地址
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // 允许 Cookie（HTTP ONLY 必须带上）
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
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["token"];
            context.Response.Headers.Append("X-Debug-Token", token ?? "no-token");

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            context.Response.Headers.Append("X-Debug-AuthError", context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});


            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICookieService, CookieService>();


            // 其他 Service 注册...
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            app.UseDefaultFiles(); // 可选：自动寻找 index.html
            app.UseStaticFiles();  // 必需：启用静态文件服务


            // 开发环境启用 Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ✅ 添加 Seeder 的异常保护
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    try
                    {
                        var services = scope.ServiceProvider;
                        var dbContext = services.GetRequiredService<UserCenterDbContext>();
                        var seeder = new DataSeeder(dbContext);
                        Console.WriteLine("[Seeder] 开始生成用户数据 如果数据库存在数据则不会自动生成");
                        await seeder.SeedUsersAsync(500); // 👈 可根据需要更改为较小数量
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Seeder] 数据种子失败: " + ex.Message);
                    }
                }
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
