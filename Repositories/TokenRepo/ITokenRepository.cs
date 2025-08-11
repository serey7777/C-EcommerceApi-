using Microsoft.AspNetCore.Identity;

namespace WebApplicationProductAPI.Repositories.TokenRepo
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
