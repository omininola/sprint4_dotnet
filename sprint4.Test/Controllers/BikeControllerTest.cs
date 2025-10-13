using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using project.DTO.Bike;
using Xunit;

namespace project.Test.Integration;

public class BikeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BikeControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // Helper: Create Auth Header (replace with your real token logic)
    private void AddAuthHeader(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-jwt-token");
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        AddAuthHeader(_client);

        var dto = new BikeDTO
        {
            Plate = "123ABC",
            Model = "SPORT",
            Status = "READY",
            YardId = 1
        };

        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/bike", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Optionally, test response body
        var json = await response.Content.ReadAsStringAsync();
        // var bike = JsonSerializer.Deserialize<BikeResponse>(json);
        // Assert.NotNull(bike);
    }

    [Fact]
    public async Task ReadAll_ShouldReturnOk()
    {
        AddAuthHeader(_client);

        var response = await _client.GetAsync("/api/bike?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Optionally, test response body
        // var json = await response.Content.ReadAsStringAsync();
        // var bikes = JsonSerializer.Deserialize<List<BikeResponse>>(json);
        // Assert.NotNull(bikes);
    }

    [Fact]
    public async Task ReadById_ShouldReturnOkOrNotFound()
    {
        AddAuthHeader(_client);

        // First, create a bike to ensure there is one (optional, depends on test DB state)
        var dto = new BikeDTO
        {
            Plate = "999XYZ",
            Model = "CRUISER",
            Status = "READY",
            YardId = 1
        };
        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/bike", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var createdBike = JsonSerializer.Deserialize<BikeResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = createdBike?.Id ?? 1;

        // Now test GET by ID
        var response = await _client.GetAsync($"/api/bike/{id}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        AddAuthHeader(_client);

        // First, create a bike
        var dto = new BikeDTO
        {
            Plate = "ZZZ111",
            Model = "TOURING",
            Status = "READY",
            YardId = 1
        };
        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/bike", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var createdBike = JsonSerializer.Deserialize<BikeResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = createdBike?.Id ?? 1;

        // Prepare update
        var updateDto = new BikeDTO
        {
            Plate = "ZZZ111",
            Model = "TOURING",
            Status = "BROKEN",
            YardId = 1
        };
        var updateContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
        var updateResponse = await _client.PutAsync($"/api/bike/{id}", updateContent);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        AddAuthHeader(_client);

        // First, create a bike
        var dto = new BikeDTO
        {
            Plate = "DEL222",
            Model = "SPORT",
            Status = "READY",
            YardId = 1
        };
        var createContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/bike", createContent);
        var createdJson = await createResponse.Content.ReadAsStringAsync();
        var createdBike = JsonSerializer.Deserialize<BikeResponse>(createdJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var id = createdBike?.Id ?? 1;

        // Now delete
        var deleteResponse = await _client.DeleteAsync($"/api/bike/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}

// You will need to define or import BikeDTO and BikeResponse for this test file to compile.
// If you use a custom authentication scheme for testing, consider customizing AddAuthHeader or using a TestAuthHandler.