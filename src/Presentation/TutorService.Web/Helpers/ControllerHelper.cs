using System.Security.Claims;

namespace TutorService.Web.Helpers;

public static class ControllerHelper
{
    public static Guid GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal)
    {
        var idClaim = claimsPrincipal.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(idClaim)) throw new UnauthorizedAccessException("User id claim missing");
        return Guid.Parse(idClaim);
    }
}