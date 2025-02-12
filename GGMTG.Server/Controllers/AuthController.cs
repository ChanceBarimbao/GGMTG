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
        /// Excutes a login request. If successful, an access token is generated.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>
        /// An ActionResult that may unwrap to be a plain string if badrequest,
        /// or a JSON string if the request is OK
        /// </returns>
        [HttpPost("Login"), AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest.Username is null) {
                return BadRequest("username is null");
            }
            if (loginRequest.Username.Length > 50) {
                return BadRequest("username is too long");
            }
            var response = HttpContext.Response;
            string? token = _securityService.Login(loginRequest);
            if (token == null)
            {
                return BadRequest("login failed");
            }

            var cookieOptions = new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddHours(2),
                // cookie can only be accessed via https
                Secure = true,
                // prevent javascript from accessing the token.
                HttpOnly = true,
                Domain = "localhost",
                // This is set to Lax because Strict will be blocked. Note that it's better than None.
                SameSite = SameSiteMode.Lax,
            };

            response.Cookies.Append("code-blueprints-access-token", token, cookieOptions);
            return Ok(JsonSerializer.Serialize("login successful"));
        }

        /// <summary>
        /// A register request that will add the user to the database if parameters are valid.
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns>
        /// An ActionResult that unwraps to be JSON string.
        /// </returns>
        [HttpPost("Register"), AllowAnonymous]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest.Email is null || registerRequest.Username is null) {
                return BadRequest("one of the params was null is null");
            }
            if (registerRequest.Email.Length > 125 || registerRequest.Username.Length > 50) {
                return BadRequest("one of the params is too long");
            }
            var result = _securityService.Register(registerRequest);

            if (result.AccessToken == null)
            {
                return BadRequest(JsonSerializer.Serialize(result.Errors));
            }

            var cookieOptions = new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(120),
                // cookie can only be accessed via https
                Secure = true,
                Domain = "localhost",
                // prevent javascript from accessing the token.
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
            };
            HttpContext.Response.Cookies.Append("code-blueprints-access-token", result.AccessToken, cookieOptions);
            return Ok(JsonSerializer.Serialize("register successful"));
        }

        /// <summary>
        /// Attempts to refresh the access token to keep it alive for another 2 hours.
        /// If the access token is expired or if a new access token fails to generate,
        /// then the refresh request fails.
        /// </summary>
        /// <returns>
        /// An ActionResult that may unwrap to be a plain string if badrequest,
        /// or a JSON string if the request is OK
        /// </returns>
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            var response = HttpContext.Response;

            var principal = HttpContext.User;
            if (principal == null)
            {
                return BadRequest("Invalid access token");
            }
            var newAccessToken = _securityService.GenerateAccessToken(principal.Claims);
            if (newAccessToken == null)
            {
                return BadRequest("failed to get new access token");
            }

            var cookieOptions = new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(120),
                // cookie can only be accessed via https
                Secure = true,
                Domain = "localhost",
                // prevent javascript from accessing the token.
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
            };
            HttpContext.Response.Cookies.Append("code-blueprints-access-token", newAccessToken, cookieOptions);

            return Ok(JsonSerializer.Serialize("refresh successful"));
        }

        /// <summary>
        /// Deletes the access token.
        /// </summary>
        /// <returns>
        /// Returns an Ok response.
        /// </returns>
        [HttpDelete("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("code-blueprints-access-token");
            HttpContext.Response.Cookies.Delete("code-blueprints-logged-in");
            return Ok(JsonSerializer.Serialize("logout successful"));
        }
    }
}
