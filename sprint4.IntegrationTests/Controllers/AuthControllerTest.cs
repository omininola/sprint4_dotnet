using Microsoft.AspNetCore.Mvc.Testing;

namespace sprint4.IntegrationTests.Controllers;

public class AuthControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Auth_Login_ShouldReturnOk()
    {
        var response = await _client.PostAsync("/api/auth/login",  null);
        
        response.EnsureSuccessStatusCode();
    }
}