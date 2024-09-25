namespace RestaurantAPI.IntegrationTests;

using System.Net;
using Entities;
using FluentAssertions;
using Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using Services;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly HttpClient _clientJwtMocked;
    private readonly Mock<IAccountService> _accountServiceMock = new();

    public AccountControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions =
                        services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                    services.Remove(dbContextOptions);

                    services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                });

            }).CreateClient();

        _clientJwtMocked= factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextOptions =
                    services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                services.Remove(dbContextOptions);

                services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));

                services.AddSingleton(_accountServiceMock.Object);
            });

        }).CreateClient();

        _accountServiceMock
            .Setup(e => e.GenerateJwt(It.IsAny<LoginDto>()))
            .Returns("token");
    }

    [Fact]
    public async Task Login_ForRegisteredUser_ReturnsOk()
    {
        var loginDto = new LoginDto()
        {
            Email = "test@test.com",
            Password = "password1"
        };

        var httpContent = loginDto.ToJsonHttpContent();

        var response = await _clientJwtMocked.PostAsync("api/account/login", httpContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_ForValidModel_ReturnsOk()
    {
        var model = new RegisterUserDto()
        {
            Email = "test@test.com",
            Password = "password1",
            ConfirmPassword = "password1",
        };

        var response = await _client.PostAsync("api/account/register", model.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_ForInvalidModel_ReturnsBadRequest()
    {
        var model = new RegisterUserDto()
        {
            Email = "testtest.com",
            Password = "password1",
            ConfirmPassword = "password1",
        };

        var response = await _client.PostAsync("api/account/register", model.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_ForValidUser_ReturnsOk()
    {
        var model = new LoginDto()
        {
            Email = "test@test.com",
            Password = "password1",
        };

        var response = await _client.PostAsync("api/account/login", model.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_ForInvalidUser_ReturnsBadRequest()
    {
        var model = new LoginDto()
        {
            Email = "test@test.com",
            Password = "password123",
        };

        var response = await _client.PostAsync("api/account/login", model.ToJsonHttpContent());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}