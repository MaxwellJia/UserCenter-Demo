using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using UserCenter.Core.Entities;
using UserCenter.Infrastructure.Data;
using UserCenter.Infrastructure.Services;
using Xunit;

namespace UserCenter.Tests
{
    public class AuthServiceTests
    {
        private AuthService CreateAuthService(out UserCenterDbContext context)
        {
            // 用 InMemory 数据库
            var options = new DbContextOptionsBuilder<UserCenterDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 每次测试独立数据库
                .Options;

            context = new UserCenterDbContext(options);

            // 配置 Identity 需要的 UserStore
            var userStore = new UserStore<ApplicationUser, IdentityRole<Guid>, UserCenterDbContext, Guid>(context);
            var userManager = new UserManager<ApplicationUser>(
                userStore,
                null, // IOptions<IdentityOptions>
                new PasswordHasher<ApplicationUser>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null, // IServiceProvider
                null  // ILogger<UserManager<ApplicationUser>>
            );

            var signInManager = new SignInManager<ApplicationUser>(
                userManager,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null
            );

            return new AuthService(userManager, signInManager);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_AndPersistToDatabase()
        {
            // Arrange
            var authService = CreateAuthService(out var context);
            var username = "testuser";
            var password = "Password123!";

            // Act
            var result = await authService.RegisterAsync(username, password);

            // Assert
            Assert.NotNull(result);  // 确认 RegisterAsync 返回了一个用户

            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            Assert.NotNull(userInDb);  // 确认数据库真的存了用户
            Assert.Equal(username, userInDb.UserName);  // 确认用户名正确
            Assert.NotNull(userInDb.PasswordHash);      // 确认密码已经加密保存
        }
    }
}