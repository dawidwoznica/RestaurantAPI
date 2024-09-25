namespace RestaurantAPI.IntegrationTests;

using FluentValidation.TestHelper;
using Models;
using Models.Validators;

public class RestaurantQueryValidatorTests
{
    public static IEnumerable<object[]> GetValidData()
    {
        var list = new List<RestaurantQuery>
        {
            new() { PageNumber = 1, PageSize = 10 },
            new() { PageNumber = 2, PageSize = 5 },
            new() { PageNumber = 23, PageSize = 5, SortBy = "Name"},
            new() { PageNumber = 12, PageSize = 15, SortDirection = SortDirection.Descending },
        };
        return list.Select(x => new object[] { x });
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        var list = new List<RestaurantQuery>
        {
            new() { PageSize = 3 },
            new() { PageNumber = 2, PageSize = 1 },
            new() { PageNumber = 23, PageSize = 5, SortBy = "Test"},
            new() { PageNumber = 12, SortDirection = SortDirection.Descending },
        };
        return list.Select(x => new object[] { x });
    }

    [Theory]
    [MemberData(nameof(GetValidData))]
    public void Validate_ForCorrectModel_ReturnsSuccess(RestaurantQuery query)
    {
        var validator = new RestaurantQueryValidator();
        var result = validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetInvalidData))]
    public void Validate_ForIncorrectModel_ReturnsFail(RestaurantQuery query)
    {
        var validator = new RestaurantQueryValidator();
        var result = validator.TestValidate(query);

        result.ShouldHaveAnyValidationError();
    }
}