using System.IdentityModel.Tokens.Jwt;
using YouG.Application.Common.Interfaces;

namespace YouG.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var subClaim = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (subClaim is null || !Guid.TryParse(subClaim, out var userId))
            {
                throw new InvalidOperationException("No authenticated user on the current request.");
            }

            return userId;
        }
    }
}
