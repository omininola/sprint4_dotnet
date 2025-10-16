using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using sprint4.DTO.Subsidiary;
using sprint4.IntegrationTests;
using Xunit;

namespace sprint4.IntegrationTests.Controllers;

public class SubsidiaryControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SubsidiaryControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AddAuthHeader()
    {
        var response = await _client.PostAsync("/api/auth/login", null);
        var json = await response.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(json);
        var token = doc.RootElement.GetProperty("token").GetString();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    [Fact]
    public async Task Subsidiary_Create_ShouldReturnCreated()
    {
        // Arrange
        await AddAuthHeader();
        
        var dto = new SubsidiaryDTO
        {
            Name = "Osasco",
            Address = "Rua dos Bobos, 123"
        };
     
        // Act
        var response = await _client.PostAsJsonAsync("/api/subsidiary", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Subsidiary_ReadAll_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();
        
        // Act
        var response = await _client.GetAsync("/api/subsidiary?page=1&pageSize=10");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Subsidiary_ReadById_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();
        
        var dto = new SubsidiaryDTO
        {
            Name = "Test",
            Address = "Address"
        };

        var id = await CreateSubsidiaryAndGetItsId(dto);

        // Act
        var response = await _client.GetAsync($"/api/subsidiary/{id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Subsidiary_Update_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();
        
        var dto = new SubsidiaryDTO
        {
            Name = "OldName",
            Address = "OldAddress"
        };

        var id = await CreateSubsidiaryAndGetItsId(dto);

        var updateDto = new SubsidiaryDTO
        {
            Name = "NewName",
            Address = "NewAddress"
        };

        // Act
        var updateResponse = await _client.PutAsJsonAsync($"/api/subsidiary/{id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Subsidiary_Delete_ShouldReturnNoContent()
    {
        // Arrange
        await AddAuthHeader();
        
        var dto = new SubsidiaryDTO
        {
            Name = "DelName",
            Address = "DelAddress"
        };

        var id = await CreateSubsidiaryAndGetItsId(dto);
        
        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/subsidiary/{id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    private async Task<int> CreateSubsidiaryAndGetItsId(SubsidiaryDTO dto)
    {
        var response = await _client.PostAsJsonAsync("/api/subsidiary", dto);
        var json = await response.Content.ReadAsStringAsync();
        
        var created = JsonSerializer.Deserialize<SubsidiaryResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;
        
        return id;
    }
}