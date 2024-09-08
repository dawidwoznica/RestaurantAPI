using FluentValidation;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private readonly int[] allowedPageSizes = [5, 10, 15];

        private readonly string[] sortByColumnNames = [nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category)];
        public RestaurantQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                    context.AddFailure("PageSize", $"PageSize must be in [{string.Join(",", allowedPageSizes)}]");
            });
            RuleFor(x => x.SortBy).Must(value => string.IsNullOrEmpty(value)
                                                 || sortByColumnNames.Contains(value)).WithMessage($"SortBy is optional, or must be in [{string.Join(",", sortByColumnNames)}]");
        }
    }
}
