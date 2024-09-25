namespace RestaurantAPI.IntegrationTests;

using System.Net;
using System.Text;
using Entities;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public RestaurantControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
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

        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
    {
        var restaurant = new Restaurant()
        {
            CreatedById = 2,
            Name = "Test",
            Category = "Test",
            Description = "Test",
        };

        await SeedRestaurant(restaurant);

        var response = await _client.DeleteAsync("api/restaurant/" + restaurant.Id);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_ForRestaurantOwner_ReturnsNotContent()
    {
        var restaurant = new Restaurant()
        {
            CreatedById = 1,
            Name = "Test",
            Category = "Test",
            Description = "Test",
        };

        await SeedRestaurant(restaurant);

        var response = await _client.DeleteAsync("api/restaurant/" + restaurant.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ForNonExistingRestaurant_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/restaurant/987");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

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

        var response = await _client.PostAsync("/api/restaurant", model.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
    {
        var model = new CreateRestaurantDto
        {
            HasDelivery = true,
            Description = "Test description",
        };

        var response = await _client.PostAsync("/api/restaurant", model.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

    private async Task SeedRestaurant(Restaurant restaurant)
    {
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();
        dbContext.Restaurants.Add(restaurant);
        await dbContext.SaveChangesAsync();
    }
}