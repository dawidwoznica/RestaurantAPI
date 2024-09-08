using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;
        public int? GetUserId => User is null ? null : (int?)int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
