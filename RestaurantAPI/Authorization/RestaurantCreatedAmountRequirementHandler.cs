using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Authorization
{
    public class RestaurantCreatedAmountRequirementHandler(RestaurantDbContext dbContext)
        : AuthorizationHandler<RestaurantsCreatedAmountRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestaurantsCreatedAmountRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            
            var userRestaurantsCount = dbContext.Restaurants.Count(r => r.CreatedById == userId);

            if(userRestaurantsCount >= requirement.MinimumAmount)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
