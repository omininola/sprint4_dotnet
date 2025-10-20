using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using sprint4.Controllers;
using sprint4.DTO.Bike;
using sprint4.DTO.Subsidiary;
using sprint4.DTO.Yard;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        var response = await _client.PostAsync("/api/v1/auth/login", null);
        var json = await response.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(json);
        var token = doc.RootElement.GetProperty("token").GetString();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task Bike_Created_ShouldReturnCreated()
    {
        // Arrange
        await AddAuthHeader();

        var yardId = await CreateYardAndGetItsId();
        
        var dto = new BikeDTO
        {
            Plate = "123ABC",
            Model = "SPORT",
            Status = "READY",
            YardId = yardId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/bike", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Bike_ReadAll_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();

        // Act
        var response = await _client.GetAsync("/api/v1/bike?page=0&pageSize=10");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Bike_ReadById_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();

        var yardId = await CreateYardAndGetItsId();
        
        var dto = new BikeDTO
        {
            Plate = "999XYZ",
            Model = "CRUISER",
            Status = "READY",
            YardId = yardId
        };

        var id = await CreateBikeAndGetItsId(dto);
        
        // Act
        var response = await _client.GetAsync($"/api/v1/bike/{id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Bike_Update_ShouldReturnOk()
    {
        // Arrange
        await AddAuthHeader();

        var yardId = await CreateYardAndGetItsId();
        
        var dto = new BikeDTO
        {
            Plate = "ZZZ111",
            Model = "TOURING",
            Status = "READY",
            YardId = yardId
        };
        
        var id = await CreateBikeAndGetItsId(dto);
        
        var updateDto = new BikeDTO
        {
            Plate = "ZZZ111",
            Model = "TOURING",
            Status = "BROKEN",
            YardId = yardId
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/bike/{id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Bike_Delete_ShouldReturnNoContent()
    {
        // Arrange
        await AddAuthHeader();

        var yardId = await CreateYardAndGetItsId();
        
        var dto = new BikeDTO
        {
            Plate = "DEL222",
            Model = "SPORT",
            Status = "READY",
            YardId = yardId
        };
        
        var id = await CreateBikeAndGetItsId(dto);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/bike/{id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private async Task<int> CreateBikeAndGetItsId(BikeDTO dto)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/bike", dto);
        var json = await response.Content.ReadAsStringAsync();
        
        BikeResponse? bike = JsonConvert.DeserializeObject<BikeResponse>(json);
        return bike?.Id ?? 1;
    }
    
    private async Task<int> CreateYardAndGetItsId()
    {
        var subsidiaryDto = new SubsidiaryDTO
        {
            Name = "Osasco",
            Address = "Rua dos Bobos, 123"
        };
        
        var subsidiaryResponse = await _client.PostAsJsonAsync("/api/v1/subsidiary",  subsidiaryDto);
        var subsidiaryJson = await subsidiaryResponse.Content.ReadAsStringAsync();
        SubsidiaryResponse? subsidiary = JsonConvert.DeserializeObject<SubsidiaryResponse>(subsidiaryJson);

        var yardDto = new YardDTO
        {
            Name = "Osasco I",
            SubsidiaryId = subsidiary?.Id ?? 1
        };
            
        var yardResponse = await _client.PostAsJsonAsync("/api/v1/yard", yardDto);
        var yardJson = await yardResponse.Content.ReadAsStringAsync();
        YardResponse? yard = JsonConvert.DeserializeObject<YardResponse>(yardJson);
        
        return yard?.Id ?? 1;
    }
}
