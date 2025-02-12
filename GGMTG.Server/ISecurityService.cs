using GGMTG.Server.Models;
using GGMTG.Server.Models.RequestBodies;
using Microsoft.AspNetCore.Mvc;
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
        string GenerateAccessToken(IEnumerable<Claim> claims);
        bool VerifyPassword(byte[] PasswordCurrent, string PasswordInput);
        byte[] HashPassword(string password);
        int PrincipalToId(ClaimsPrincipal principal);
        string PrincipalToUsername(ClaimsPrincipal principal);
        string PrincipalToEmail(ClaimsPrincipal principal);
        DateTime PrincipalToAccountCreated(ClaimsPrincipal principal);

        string? ConvertToAccessToken(Account account);
        Account? Consistence(ClaimsPrincipal principal, HttpResponse response);
    }
}
