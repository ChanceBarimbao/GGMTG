using Microsoft.AspNetCore.Authorization;

// request strings must be nullable or the request will not parse properly.

namespace GGMTG.Server.Models.RequestBodies
{
    public record GetProfileRequest
    {
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime Joined { get; set; }
    }
}