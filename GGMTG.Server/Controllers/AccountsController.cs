using Azure;
using GGMTG.Server.Models;
using GGMTG.Server.Models.RequestBodies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Text.Json;

namespace GGMTG.Server.Controllers
{
    /// <summary>
    /// An account controller for mutating account data.
    /// This class does not create new accounts. Refer to AuthController for account creation.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityService _securityService;

        public AccountsController(ISecurityService securityService, IAccountRepository accountRepository)
        {
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        /// <summary>
        /// Authenticates against the current password. If authenticated successfully then change the password.
        /// </summary>
        /// <param name="changePasswordRequest"></param>
        /// <returns>
        /// Returns an Ok or BadRequest response.
        /// </returns>
        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (changePasswordRequest.OldPassword == null || changePasswordRequest.NewPassword == null)
            {
                return BadRequest("missing password");
            }
            if (changePasswordRequest.OldPassword == changePasswordRequest.NewPassword)
            {
                return BadRequest("new password must be different from old password");
            }
            int custId = _securityService.PrincipalToId(HttpContext.User);
            Account account = _accountRepository.FindAccountById(custId)!;

            // verify password to make sure you are not a hacker.
            bool isVerified = _securityService.VerifyPassword(account.Password, changePasswordRequest.OldPassword);
            if (isVerified)
            {
                account.Password = _securityService.HashPassword(changePasswordRequest.NewPassword);
                if (_accountRepository.UpdateAccount(account))
                {
                    return Ok(JsonSerializer.Serialize("password updated successfully!"));
                }
                else
                {
                    return BadRequest("failed to update password");
                }
            }
            else
            {
                return BadRequest("incorrect password");
            }
        }
        /// <summary>
        /// Renames the username in the database.
        /// </summary>
        /// <param name="changeUsernameRequest"></param>
        /// <returns>
        /// Returns an Ok or BadRequest response.
        /// </returns>
        [HttpPut("ChangeUsername")]
        public IActionResult ChangeUsername([FromBody] ChangeUsernameRequest changeUsernameRequest)
        {
            _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (changeUsernameRequest.NewUsername == null)
            {
                return BadRequest("missing username");
            }

            if(changeUsernameRequest.NewUsername.Length > 50) {
                return BadRequest("New username is too long");
            }
            

            int custId = _securityService.PrincipalToId(HttpContext.User);
            Account account = _accountRepository.FindAccountById(custId)!;

            if (account.Username == changeUsernameRequest.NewUsername)
            {
                return BadRequest("new username must be different from old username");
            }

            account.Username = changeUsernameRequest.NewUsername;
            if (_accountRepository.UpdateAccount(account))
            {
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
                HttpContext.Response.Cookies.Append(
                    "code-blueprints-access-token",
                    _securityService.ConvertToAccessToken(account),
                    cookieOptions
                );

                return Ok(JsonSerializer.Serialize("username updated successfully!"));
            }
            else
            {
                return BadRequest("failed to update username");
            }
        }

        [Obsolete("Use GetProfile instead")]
        [HttpGet("getUsername")]
        public IActionResult GetUsername()
        {
            Account? account = _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (account is null)
            {
                return BadRequest("Unable to get username");
            }
            //string username = _securityService.PrincipalToUsername(HttpContext.User);
            return Ok(JsonSerializer.Serialize(account.Username));
        }

        [Obsolete("Use GetProfile instead")]
        [HttpGet("getEmail")]
        public IActionResult GetEmail()
        {
            Account? account = _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (account is null)
            {
                return BadRequest("Unable to get email");
            }
            // string username = _securityService.PrincipalToEmail(HttpContext.User);
            return Ok(JsonSerializer.Serialize(account.Email));
        }

        [Obsolete("Use GetProfile instead")]
        [HttpGet("getAccountCreated")]
        public IActionResult GetAccountCreated()
        {
            Account? account = _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (account is null)
            {
                return BadRequest("Unable to get account join date");
            }
            // DateTime accountCreated = _securityService.PrincipalToAccountCreated(HttpContext.User);
            return Ok(JsonSerializer.Serialize(account.Joined));
        }

        [HttpGet("getProfile")]
        public IActionResult GetProfile()
        {
            Account? account = _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (account is null)
            {
                return BadRequest("Unable to get account join date");
            }
            GetProfileRequest request = new()
            {
                Username = account.Username,
                Email = account.Email,
                Joined = account.Joined
            };
            return Ok(JsonSerializer.Serialize(request));
        }


        /// <summary>
        /// Changes the email of the account in the database.
        /// The email must not already exist.
        /// </summary>
        /// <param name="changeEmailRequest"></param>
        /// <returns>
        /// /// Returns an Ok or BadRequest response.
        /// </returns>
        [HttpPut("ChangeEmail")]
        public IActionResult ChangeEmail([FromBody] ChangeEmailRequest changeEmailRequest)
        {
            _securityService.Consistence(HttpContext.User, HttpContext.Response);
            if (changeEmailRequest.NewEmail == null)
            {
                return BadRequest("missing email");
            }
            if (changeEmailRequest.NewEmail.Length > 125)
            {
                return BadRequest("new email is too long");
            }

            Account? existingAccount = _accountRepository.FindAccountByEmail(changeEmailRequest.NewEmail);
            if(existingAccount != null) {
                return BadRequest("Email already exists");
            }

            int custId = _securityService.PrincipalToId(HttpContext.User);
            Account account = _accountRepository.FindAccountById(custId)!;

            account.Email = changeEmailRequest.NewEmail;
            if (_accountRepository.UpdateAccount(account))
            {
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
                HttpContext.Response.Cookies.Append(
                    "code-blueprints-access-token",
                    _securityService.ConvertToAccessToken(account),
                    cookieOptions
                );
                return Ok(JsonSerializer.Serialize("email updated successfully!"));
            }
            else
            {
                return BadRequest("failed to update email");
            }
        }

        /// <summary>
        /// Deletes the account from the database and then deletes access token.
        /// </summary>
        /// <returns>
        /// Returns an Ok or BadRequest response.
        /// </returns>
        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteAccount()
        {
            _securityService.Consistence(HttpContext.User, HttpContext.Response);
            int custId = _securityService.PrincipalToId(HttpContext.User);
            Account account = _accountRepository.FindAccountById(custId)!;
            if (_accountRepository.DeleteAccount(account))
            {
                HttpContext.Response.Cookies.Delete("code-blueprints-access-token");
                return Ok(JsonSerializer.Serialize("account deleted"));
            }
            else
            {
                return BadRequest("failed to delete account");
            }
        }
    }
}
