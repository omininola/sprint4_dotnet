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

    private async Task<int> CreateSubsidiaryAndReturnItsId()
    {
        var dto = new SubsidiaryDTO
        {
            Name = "Osasco",
            Address = "Rua dos Bobos, 123"
        };

        var response = await _client.PostAsJsonAsync("/api/subsidiaries", dto);
        var json = await response.Content.ReadAsStringAsync();
        SubsidiaryResponse? subsidiary = JsonConvert.DeserializeObject<SubsidiaryResponse>(json);

        if (subsidiary != null) return subsidiary.Id;
        return 1;
    }

    [Fact]
    public async Task Yard_Create_ShouldReturnCreated()
    {
        // Arrange
        await AddAuthHeader();

        var subsidiaryId = await CreateSubsidiaryAndReturnItsId();
        
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
        await AddAuthHeader();

        var response = await _client.GetAsync("/api/yard?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadById_ShouldReturnOkOrNotFound()
    {
        await AddAuthHeader();

        // Create for test
        var dto = new YardDTO
        {
            Name = "Test Yard",
            SubsidiaryId = 1
        };
        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/yard", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<YardResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;

        var response = await _client.GetAsync($"/api/yard/{id}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        await AddAuthHeader();

        // Create for test
        var dto = new YardDTO
        {
            Name = "Old Yard",
            SubsidiaryId = 1
        };
        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/yard", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<YardResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;

        // Prepare update
        var updateDto = new YardDTO
        {
            Name = "Updated Yard",
            SubsidiaryId = 1
        };
        var updateContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
        var updateResponse = await _client.PutAsync($"/api/yard/{id}", updateContent);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        await AddAuthHeader();

        // Create for test
        var dto = new YardDTO
        {
            Name = "Del Yard",
            SubsidiaryId = 1
        };
        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/yard", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<YardResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;

        var deleteResponse = await _client.DeleteAsync($"/api/yard/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}