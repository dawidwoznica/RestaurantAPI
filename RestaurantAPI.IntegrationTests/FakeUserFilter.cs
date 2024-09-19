namespace RestaurantAPI.IntegrationTests;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

public class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin")
        ]));

        context.HttpContext.User = claimsPrincipal;

        await next();
    }
}