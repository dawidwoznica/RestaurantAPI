using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class RestaurantsCreatedAmountRequirement(int minimumAmount) : IAuthorizationRequirement
    {
        public int MinimumAmount { get; } = minimumAmount;
    }
}
