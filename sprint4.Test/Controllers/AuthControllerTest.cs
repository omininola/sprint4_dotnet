using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace sprint4.Test.Controllers;

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
        var response = await _client.GetAsync("/api/auth/login");
        response.EnsureSuccessStatusCode();
    }
}