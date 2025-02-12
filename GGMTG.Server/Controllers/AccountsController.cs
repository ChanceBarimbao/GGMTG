using GGMTG.Server.Models;
using GGMTG.Server.Models.RequestBodies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GGMTG.Server.Controllers
{
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

        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            if (string.IsNullOrWhiteSpace(changePasswordRequest.OldPassword) || string.IsNullOrWhiteSpace(changePasswordRequest.NewPassword))
            {
                return BadRequest("Missing password");
            }

            if (changePasswordRequest.OldPassword == changePasswordRequest.NewPassword)
            {
                return BadRequest("New password must be different from old password");
            }

            int custId = _securityService.PrincipalToId(HttpContext.User);
            var account = _accountRepository.FindAccountById(custId);

            if (account == null || !_securityService.VerifyPassword(account.Password, changePasswordRequest.OldPassword))
            {
                return BadRequest("Incorrect password");
            }

            account.Password = _securityService.HashPassword(changePasswordRequest.NewPassword);

            return _accountRepository.UpdateAccount(account)
                ? Ok(JsonSerializer.Serialize("Password updated successfully!"))
                : BadRequest("Failed to update password");
        }

        [HttpGet("getProfile")]
        public IActionResult GetProfile()
        {
            var account = _accountRepository.FindAccountById(_securityService.PrincipalToId(HttpContext.User));
            return account != null
                ? Ok(JsonSerializer.Serialize(new GetProfileRequest { Email = account.Email }))
                : BadRequest("Unable to retrieve account data");
        }

        [HttpPut("ChangeEmail")]
        public IActionResult ChangeEmail([FromBody] ChangeEmailRequest changeEmailRequest)
        {
            if (string.IsNullOrWhiteSpace(changeEmailRequest.NewEmail) || changeEmailRequest.NewEmail.Length > 125)
            {
                return BadRequest("Invalid email");
            }

            if (_accountRepository.FindAccountByEmail(changeEmailRequest.NewEmail) != null)
            {
                return BadRequest("Email already exists");
            }

            int custId = _securityService.PrincipalToId(HttpContext.User);
            var account = _accountRepository.FindAccountById(custId);

            if (account == null)
            {
                return BadRequest("Account not found");
            }

            account.Email = changeEmailRequest.NewEmail;

            return _accountRepository.UpdateAccount(account)
                ? Ok(JsonSerializer.Serialize("Email updated successfully!"))
                : BadRequest("Failed to update email");
        }

        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteAccount()
        {
            int custId = _securityService.PrincipalToId(HttpContext.User);
            var account = _accountRepository.FindAccountById(custId);

            return account != null && _accountRepository.DeleteAccount(account)
                ? Ok(JsonSerializer.Serialize("Account deleted"))
                : BadRequest("Failed to delete account");
        }
    }
}
