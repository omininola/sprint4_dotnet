using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using sprint4.Controllers;
using sprint4.DTO.Bike;

namespace sprint4.IntegrationTests.Controllers;

public class BikeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BikeControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task Bike_Created_ShouldReturnCreated()
    {
        await AddAuthHeader();

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
    }

    [Fact]
    public async Task Bike_ReadAll_ShouldReturnOk()
    {
        await AddAuthHeader();

        var response = await _client.GetAsync("/api/bike?page=0&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Bike_ReadById_ShouldReturnOkOrNotFound()
    {
        await AddAuthHeader();

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

        var response = await _client.GetAsync($"/api/bike/{id}");
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Bike_Update_ShouldReturnOk()
    {
        await AddAuthHeader();

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
    public async Task Bike_Delete_ShouldReturnNoContent()
    {
        await AddAuthHeader();

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

        var deleteResponse = await _client.DeleteAsync($"/api/bike/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
