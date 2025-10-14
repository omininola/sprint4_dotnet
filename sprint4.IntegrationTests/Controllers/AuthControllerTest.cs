using Microsoft.AspNetCore.Mvc.Testing;

namespace sprint4.IntegrationTests.Controllers;

public class AuthControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    // private readonly HttpClient _client;
    //
    // public AuthControllerTest(ApiWebApplicationFactory factory)
    // {
    //     _client = factory.CreateClient();
    // }
    
    [Fact]
    public async Task Auth_Login_ShouldReturnOk()
    {
        // Arrange
        var application = new ApiWebApplicationFactory();
        var _client = application.CreateClient();
        
        // Act
        var response = await _client.GetAsync("/api/auth/login");
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
}