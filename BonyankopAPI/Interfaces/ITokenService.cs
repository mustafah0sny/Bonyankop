using BonyankopAPI.Models;

namespace BonyankopAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        Guid GetUserIdFromToken(System.Security.Claims.ClaimsPrincipal user);
    }
}
