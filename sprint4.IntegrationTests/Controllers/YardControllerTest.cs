using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using sprint4.DTO.Subsidiary;
using sprint4.DTO.Yard;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace sprint4.IntegrationTests.Controllers;

public class YardControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public YardControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task Yard_Create_ShouldReturnCreated()
    {
        // Arrange
        await AddAuthHeader();

        var subsidiaryId = await CreateSubsidiaryAndGetItsId();
        
        var dto = new YardDTO
        {
            Name = "Osasco I",
            SubsidiaryId = subsidiaryId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/yard", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ReadAll_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();
        
        // Act
        var response = await _client.GetAsync("/api/yard?page=1&pageSize=10");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadById_ShouldReturnOkOrNotFound()
    {
        // Arrange
        await AddAuthHeader();

        var subsidiaryId = await CreateSubsidiaryAndGetItsId();        

        var dto = new YardDTO
        {
            Name = "Test Yard",
            SubsidiaryId = subsidiaryId
        };

        var id = await CreateYardAndGetItsId(dto);
        
        // Act
        var response = await _client.GetAsync($"/api/yard/{id}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();

        var subsidiaryId = await CreateSubsidiaryAndGetItsId();
        
        var dto = new YardDTO
        {
            Name = "Old Yard",
            SubsidiaryId = subsidiaryId
        };

        var id = await CreateYardAndGetItsId(dto);
        
        var updateDto = new YardDTO
        {
            Name = "Updated Yard",
            SubsidiaryId = subsidiaryId
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/yard/{id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // Arrange
        await AddAuthHeader();

        var subsidiaryId = await CreateSubsidiaryAndGetItsId();
        
        var dto = new YardDTO
        {
            Name = "Del Yard",
            SubsidiaryId = subsidiaryId
        };
        
        var id = await CreateYardAndGetItsId(dto);

        // Act
        var response = await _client.DeleteAsync($"/api/yard/{id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<int> CreateYardAndGetItsId(YardDTO dto)
    {
        var response = await _client.PostAsJsonAsync("/api/yard", dto);
        var json = await response.Content.ReadAsStringAsync();
        YardResponse? yard = JsonConvert.DeserializeObject<YardResponse>(json);
        return yard?.Id ?? 1;
    }
    
    private async Task<int> CreateSubsidiaryAndGetItsId()
    {
        var dto = new SubsidiaryDTO
        {
            Name = "Osasco",
            Address = "Rua dos Bobos, 123"
        };

        var response = await _client.PostAsJsonAsync("/api/subsidiaries", dto);
        var json = await response.Content.ReadAsStringAsync();
        SubsidiaryResponse? subsidiary = JsonConvert.DeserializeObject<SubsidiaryResponse>(json);

        return subsidiary?.Id ?? 1;
    }
}