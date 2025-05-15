using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserCenter.Infrastructure.Services;
using UserCenter.Core.Entities;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Configuration;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null
        );

        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(j => j.Value).Returns(new JwtSettings
        {
            SecretKey = "12345678901234567890123456789012", // 32-char for HMAC
            Issuer = "testissuer",
            Audience = "testaudience",
            ExpirationMinutes = 30
        });

        _loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _jwtSettingsMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsValid()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Username = "TestUser",
            Password = "Valid@123",
            Email = "test@example.com"
        };

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenPasswordInvalid()
    {
        var dto = new RegisterUserDto
        {
            Username = "TestUser",
            Password = "weak", // invalid format
            Email = "test@example.com"
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Contains("Password", result.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreCorrect()
    {
        var user = new ApplicationUser { UserName = "TestUser" };
        var dto = new LoginRequestDto
        {
            Username = "TestUser",
            Password = "Valid@123"
        };

        _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
        _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false))
            .ReturnsAsync(SignInResult.Success);

        var result = await _authService.LoginAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenPasswordIsWrong()
    {
        var user = new ApplicationUser { UserName = "TestUser" };
        var dto = new LoginRequestDto
        {
            Username = "TestUser",
            Password = "Wrong@123"
        };

        _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
        _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        var result = await _authService.LoginAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid password", result.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenUserNotFound()
    {
        var dto = new LoginRequestDto
        {
            Username = "NotExist",
            Password = "Anything123!"
        };

        _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.LoginAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.Message);
    }
}
