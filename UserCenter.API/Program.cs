
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            // ��ȡ�����ַ���
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // ע�� DbContext
            builder.Services.AddDbContext<UserCenterDbContext>(options =>
                options.UseNpgsql(connectionString));

            // ע�� Identity
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

            // ���������һ�У�ע�� Controller ����
            builder.Services.AddControllers();

            // ���������У����� Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ע����֤ + ��Ȩ����
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            // ���� Service ע��...
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // ������������ Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure the HTTP request pipeline...
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
