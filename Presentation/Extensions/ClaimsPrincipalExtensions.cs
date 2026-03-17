using DAL.Models.Enum;
using System.Security.Claims;

namespace Presentation.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetRequiredUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            return Guid.Parse(userIdClaim);
        }

        public static void RequireRole(this ClaimsPrincipal user, params Role[] allowedRoles)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(roleClaim) || !int.TryParse(roleClaim, out var rawRole))
            {
                throw new UnauthorizedAccessException("User role is invalid");
            }

            if (!allowedRoles.Any(role => (int)role == rawRole))
            {
                throw new UnauthorizedAccessException("User does not have permission to perform this action");
            }
        }
    }
}
