using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using LoginRequest = GGMTG.Server.Models.RequestBodies.LoginRequest;
using RegisterRequest = GGMTG.Server.Models.RequestBodies.RegisterRequest;

namespace GGMTG.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ISecurityService _securityService;

        public AuthController(ISecurityService securityService)
        {
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        }

        /// <summary>
        /// Executes a login request. If successful, returns a success message.
        /// </summary>
        [HttpPost("Login"), AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest.Email is null)
            {
                return BadRequest("Email is null");
            }
            if (loginRequest.Email.Length > 50)
            {
                return BadRequest("Email is too long");
            }

            bool isLoginSuccessful = _securityService.Login(loginRequest) != null;
            if (!isLoginSuccessful)
            {
                return BadRequest("Login failed");
            }

            return Ok(JsonSerializer.Serialize(new { message = "Login successful" }));
        }

        /// <summary>
        /// A register request that will add the user to the database if parameters are valid.
        /// </summary>
        [HttpPost("Register"), AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest.Email is null)
            {
                return BadRequest("Email is null");
            }
            if (registerRequest.Email.Length > 125 || registerRequest.Email.Length > 50)
            {
                return BadRequest("Email is too long");
            }

            var result = _securityService.Register(registerRequest);

            if (result.Errors != null && result.Errors.Count > 0)
            {
                return BadRequest(JsonSerializer.Serialize(result.Errors));
            }

            return Ok(JsonSerializer.Serialize(new { message = "Register successful" }));
        }

        /// <summary>
        /// Attempts to refresh the session or any user-related data.
        /// </summary>
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            // Since JWT is no longer in use, this method could be repurposed for refreshing user session data, etc.
            return Ok(JsonSerializer.Serialize(new { message = "Session refresh successful" }));
        }

        /// <summary>
        /// Logs the user out, ending their session.
        /// </summary>
        [HttpDelete("Logout")]
        public IActionResult Logout()
        {
            return Ok(JsonSerializer.Serialize("Logout successful"));
        }
    }
}
