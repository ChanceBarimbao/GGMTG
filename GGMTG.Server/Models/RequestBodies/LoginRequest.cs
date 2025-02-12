using Microsoft.AspNetCore.Authorization;

// request strings must be nullable or the request will not parse properly.

namespace GGMTG.Server.Models.RequestBodies
{
    public record LoginRequest
    {
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}
