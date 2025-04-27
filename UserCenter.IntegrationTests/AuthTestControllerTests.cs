using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UserCenter.API;
using Xunit;

namespace UserCenter.IntegrationTests
{
    public class AuthTestControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthTestControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(); // 创建真实HttpClient，连到你的API
        }

        [Fact]
        public async Task Register_ShouldCreateUserInDatabase()
        {
            // Arrange
            var username = $"testuser_{Guid.NewGuid()}"; // 保证唯一
            var password = "Password123!";

            var requestUri = $"/api/AuthTest/register?username={username}&password={password}";

            // Act
            var response = await _client.PostAsync(requestUri, null);

            // Assert
            response.EnsureSuccessStatusCode(); // 确保返回200 OK

            var result = await response.Content.ReadFromJsonAsync<UserResponse>();

            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        // 简单封装一个UserResponse对应你的返回数据
        private class UserResponse
        {
            public Guid Id { get; set; }
            public string Username { get; set; } = string.Empty;
        }
    }
}
