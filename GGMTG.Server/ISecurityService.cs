using GGMTG.Server.Models;
using GGMTG.Server.Models.RequestBodies;
using System.Security.Claims;

namespace GGMTG.Server
{
    /// <inheritdoc/>
    public interface ISecurityService
    {
        /// <inheritdoc/>
        string? Login(LoginRequest loginRequest);

        /// <inheritdoc/>
        AuthResult Register(RegisterRequest registerRequest);

        /// <inheritdoc/>
        bool VerifyPassword(byte[] PasswordCurrent, string PasswordInput);

        /// <inheritdoc/>
        byte[] HashPassword(string password);

        int PrincipalToId(ClaimsPrincipal principal);

    }
}
