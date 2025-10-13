using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using project.DTO.Yard;
using Xunit;

namespace project.Test.Integration;

public class YardControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public YardControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private void AddAuthHeader(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token");
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        AddAuthHeader(_client);

        var dto = new YardDTO
        {
            Name = "Osasco I",
            SubsidiaryId = 1
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/yard", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ReadAll_ShouldReturnOk()
    {
        AddAuthHeader(_client);

        var response = await _client.GetAsync("/api/yard?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadById_ShouldReturnOkOrNotFound()
    {
        AddAuthHeader(_client);

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
        AddAuthHeader(_client);

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
        AddAuthHeader(_client);

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