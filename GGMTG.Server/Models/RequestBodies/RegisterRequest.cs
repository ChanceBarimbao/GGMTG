using Microsoft.AspNetCore.Authorization;

// request strings must be nullable or the request will not parse properly.

namespace GGMTG.Server.Models.RequestBodies
{
    public record RegisterRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
