namespace RestaurantAPI.IntegrationTests;

using System.Net;
using System.Text;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;

public class RestaurantControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient _client = factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureServices(services =>
        {
            var dbContextOptions =
                services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

            services.Remove(dbContextOptions);

            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

            services.AddMvc(options => options.Filters.Add(new FakeUserFilter()));

            services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
        });
    }).CreateClient();


    [Fact]
    public async Task CreateRestaurant_WithValidModel_ReturnsCreated()
    {
        var model = new CreateRestaurantDto
        {
            Name = "Test",
            Street = "Długa 5",
            City = "Kraków",
            Category = "TestCategory",
            PostalCode = "60-606",
            Description = "Test description",
        };

        var jsonModel = JsonConvert.SerializeObject(model);
        var httpContent = new StringContent(jsonModel, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/restaurant", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }


[Theory]
    [InlineData("PageNumber=1&PageSize=5")]
    [InlineData("PageNumber=2&PageSize=10")]
    [InlineData("PageNumber=3&PageSize=15")]
    public async Task GetAll_WithQueryParameters_ReturnsOk(string queryParams)
    {
        var response = await _client.GetAsync($"/api/restaurant?{queryParams}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("PageNumber=1&PageSize=55")]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetAll_WithInvalidQueryParameters_ReturnsBadRequest(string queryParams)
    {
        var response = await _client.GetAsync($"/api/restaurant?{queryParams}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}