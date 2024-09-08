using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgerRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly ILogger<MinimumAgerRequirementHandler> _logger;

        public MinimumAgerRequirementHandler(ILogger<MinimumAgerRequirementHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth= DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);

            var userName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;

            _logger.LogInformation($"Authorizing user: {userName} with date of birth: {dateOfBirth}...");

            if (dateOfBirth.AddYears(requirement.MinimumAge) < DateTime.Today)
            {
                _logger.LogInformation("Authorization succeeded!");
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
