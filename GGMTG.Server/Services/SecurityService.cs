using GGMTG.Server.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using GGMTG.Server.Models.RequestBodies;
using Microsoft.AspNetCore.Http;

namespace GGMTG.Server.Services
{
    internal class SecurityService(IConfiguration config, IAccountRepository accountRepository) : ISecurityService
    {

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ??
                                                                           throw new InvalidOperationException("Could not find JWT configuration")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public byte[] HashPassword(string password) {
            byte[] data = Encoding.ASCII.GetBytes(password + config["Salt"]!);
            return SHA512.HashData(data);
        }

        public string? ConvertToAccessToken(Account account)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Username),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Sid, account.CustId.ToString()),
                    new Claim("Joined", account.Joined.ToString())
                };
            var accessToken = GenerateAccessToken(claims);
            return accessToken;
        }

        public string? Login(LoginRequest loginRequest)
        {
            if (loginRequest.Username == null || loginRequest.Password == null) {
                // not a valid request
                return null;
            }

            var account = accountRepository.FindAccountByUsername(loginRequest.Username);
            if (account == null)
            {
                // User is not found.
                return null;
            }

            byte[] hash = HashPassword(loginRequest.Password);


            string? accessToken = null;
            if (account.Password.SequenceEqual(hash))
            {
                accessToken = ConvertToAccessToken(account);
            }
            return accessToken;
        }


        public AuthResult Register(RegisterRequest registerRequest)
        {
            if (registerRequest.Username == null
            || registerRequest.Password == null
            || registerRequest.Email == null) {
                // not a valid request. This should be prevented at client level.
                var result = new AuthResult
                {
                    Errors = new List<string> { "Invalid request" },
                };
                return result;
            }

            Account? existingUser = accountRepository.FindAccountByEmail(registerRequest.Email);


            if (existingUser != null)
            {
                // User is found by email.
                var result = new AuthResult
                {
                    Errors = new List<string> { "Email already exists" },
                };
                return result;
            }

            existingUser = accountRepository.FindAccountByUsername(registerRequest.Username);

            if (existingUser != null)
            {
                // User is found by username
                var result = new AuthResult
                {
                    Errors = new List<string> { "Username already exists" },
                };
                return result;
            }

            byte[] hashedPassword = HashPassword(registerRequest.Password);

            var account = new Account
            {
                Username = registerRequest.Username,
                Email = registerRequest.Email,
                Password = hashedPassword,
                Joined = DateTime.Now,
            };
            bool isCreated = accountRepository.AddAccount(account);
            if (isCreated)
            {
                var accessToken = ConvertToAccessToken(account);

                return new AuthResult()
                {
                    AccessToken = accessToken,
                };
            }

            // TODO: double check that this is actually the case
            return new AuthResult {
                Errors = new List<string> { "Failed to create account" }
            };
        }

        public bool VerifyPassword(byte[] PasswordCurrent, string PasswordInput)
        {
            byte[] hash = HashPassword(PasswordInput);
            return PasswordCurrent.SequenceEqual(hash);
        }

        public int PrincipalToId(ClaimsPrincipal principal)
        {
            string sid = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
            return int.Parse(sid);
        }

        public string PrincipalToUsername(ClaimsPrincipal principal)
        {
            return principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
        public string PrincipalToEmail(ClaimsPrincipal principal)
        {
            return principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        }
        public DateTime PrincipalToAccountCreated(ClaimsPrincipal principal)
        {
            return DateTime.Parse(principal.Claims.First(c => c.Type == "Joined").Value);
        }
        // This function will check if the claims are still consistent with database.
        // It will fix any inconsistencies detected with user session.
        public Account? Consistence(ClaimsPrincipal principal, HttpResponse response)
        {
            string sid = principal.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
            int accountId = Int32.Parse(sid);
            Account? account = accountRepository.FindAccountById(accountId);
            if(account == null) {
                 response.Cookies.Delete("code-blueprints-access-token");
                return null;
            }
            var cookieOptions = new CookieOptions()
            {
                // expiry is reset
                Expires = DateTimeOffset.UtcNow.AddHours(2),
                // cookie can only be accessed via https
                Secure = true,
                // prevent javascript from accessing the token.
                HttpOnly = true,
                Domain = "localhost",
                // This is set to Lax because Strict will be blocked. Note that it's better than None.
                SameSite = SameSiteMode.Lax,
            };
            string username = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string email = principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            if(username == account.Username || email == account.Email) {
                response.Cookies.Append(
                    "code-blueprints-access-token",
                    ConvertToAccessToken(account),
                    cookieOptions
                );
            }
            return account;
        }
    }
}
