using GGMTG.Server.Models;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using GGMTG.Server.Models.RequestBodies;

namespace GGMTG.Server.Services
{
    internal class SecurityService(IConfiguration config, IAccountRepository accountRepository) : ISecurityService
    {
        public byte[] HashPassword(string password)
        {
            byte[] data = Encoding.ASCII.GetBytes(password + config["Salt"]!);
            return SHA512.HashData(data);
        }

        public string? Login(LoginRequest loginRequest)
        {
            if (loginRequest.Email == null || loginRequest.Password == null)
            {
                return null;
            }

            var account = accountRepository.FindAccountByEmail(loginRequest.Email);
            if (account == null)
            {
                return null;
            }

            byte[] hash = HashPassword(loginRequest.Password);

            if (account.Password.SequenceEqual(hash))
            {
                return "Login successful"; // Returns success message instead of JWT
            }
            return null;
        }

        public AuthResult Register(RegisterRequest registerRequest)
        {
            if (registerRequest.Password == null || registerRequest.Email == null)
            {
                return new AuthResult
                {
                    Errors = new List<string> { "Invalid request" },
                };
            }

            Account? existingUser = accountRepository.FindAccountByEmail(registerRequest.Email);

            if (existingUser != null)
            {
                return new AuthResult
                {
                    Errors = new List<string> { "Email already exists" },
                };
            }

            byte[] hashedPassword = HashPassword(registerRequest.Password);

            var account = new Account
            {
                Email = registerRequest.Email,
                Password = hashedPassword,
            };

            bool isCreated = accountRepository.AddAccount(account);
            if (isCreated)
            {
                return new AuthResult()
                {
                    AccessToken = "Account created successfully", // Placeholder response
                };
            }

            return new AuthResult
            {
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

        public string PrincipalToEmail(ClaimsPrincipal principal)
        {
            return principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        }
    }
}
