using Microsoft.AspNetCore.Mvc.Testing;

namespace sprint4.IntegrationTests.Controllers;

public class AuthControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTest(ApiWebApplicationFactory factory) : base(factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Auth_Login_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/login");
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
}