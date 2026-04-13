using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskManagement.Presentation.Tests;

public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UnitTest1(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PublicPing_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users/public-ping");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithSeedCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/users/login", new
        {
            username = "admin",
            password = "admin123"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.Token));
    }

    private sealed class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}
