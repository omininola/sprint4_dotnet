using System.Net;
using System.Net.Http.Headers;
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
    public async Task Create_ShouldReturnCreated()
    {
        await AddAuthHeader();

        var dto = new SubsidiaryDTO
        {
            Name = "Osasco",
            Address = "Rua dos Bobos, 123"
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/subsidiary", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task ReadAll_ShouldReturnOk()
    {
        await AddAuthHeader();

        var response = await _client.GetAsync("/api/subsidiary?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReadById_ShouldReturnOkOrNotFound()
    {
        await AddAuthHeader();

        var dto = new SubsidiaryDTO
        {
            Name = "Test",
            Address = "Address"
        };

        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/subsidiary", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<SubsidiaryResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;

        var response = await _client.GetAsync($"/api/subsidiary/{id}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        await AddAuthHeader();

        var dto = new SubsidiaryDTO
        {
            Name = "OldName",
            Address = "OldAddress"
        };

        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/subsidiary", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<SubsidiaryResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;

        var updateDto = new SubsidiaryDTO
        {
            Name = "NewName",
            Address = "NewAddress"
        };

        var updateContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
        var updateResponse = await _client.PutAsync($"/api/subsidiary/{id}", updateContent);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        await AddAuthHeader();

        var dto = new SubsidiaryDTO
        {
            Name = "DelName",
            Address = "DelAddress"
        };

        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/subsidiary", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<SubsidiaryResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = created?.Id ?? 1;

        var deleteResponse = await _client.DeleteAsync($"/api/subsidiary/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}