namespace RestaurantAPI.IntegrationTests;

using Entities;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Validators;

public class RestaurantUserDtoValidatorTests
{
    private readonly RestaurantDbContext _dbContext;

    public RestaurantUserDtoValidatorTests()
    {
        var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
        builder.UseInMemoryDatabase("TestDb");

        _dbContext = new RestaurantDbContext(builder.Options);

        Seed();
    }

    public static IEnumerable<object[]> GetValidData()
    {
        var list = new List<RegisterUserDto>
        {
            new() { Email = "test@test.com", Password = "password123", ConfirmPassword = "password123" },
            new() { Email = "test@kek.pl", Password = "12p#@!3ad2d1!@!3dasdadda3!!!23s", ConfirmPassword = "12p#@!3ad2d1!@!3dasdadda3!!!23s" },
            new() { Email = "test22@test.com", Password = "Apassword123", ConfirmPassword = "Apassword123"}
        };
        return list.Select(x => new object[] { x });
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        var list = new List<RegisterUserDto>
        {
            new() { Email = "test@test.com", Password = "password1", ConfirmPassword = "passwor1" },
            new() { Email = "test3@test.com", Password = "password123", ConfirmPassword = "password123" },
            new() { Email = "test@test.com", Password = "pass", ConfirmPassword = "pass"}
        };
        return list.Select(x => new object[] { x });
    }

    private void Seed()
    {
        var testUsers = new List<User>
        {
            new()
            {
                Email = "test2@test.com",
                PasswordHash = "password",
            },
            new()
            {
                Email = "test3@test.com",
                PasswordHash = "password",
            }
        };

        _dbContext.Users.AddRange(testUsers);
        _dbContext.SaveChanges();
    }

    [Theory]
    [MemberData(nameof(GetValidData))]
    public void Validate_ForCorrectModel_ReturnsSuccess(RegisterUserDto registerUserDto)
    {
        var validator = new RegisterUserDtoValidator(_dbContext);
        var result = validator.TestValidate(registerUserDto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetInvalidData))]
    public void Validate_ForIncorrectModel_ReturnsFail(RegisterUserDto registerUserDto)
    {
        var validator = new RegisterUserDtoValidator(_dbContext);
        var result = validator.TestValidate(registerUserDto);

        result.ShouldHaveAnyValidationError();
    }
}