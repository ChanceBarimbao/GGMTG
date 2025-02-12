namespace GGMTG.Server.Models
{
    // result type for resolving security service errors and returning AuthHeader.
    public record AuthResult
    {
        public string? AccessToken { get; set; }
        public List<string>? Errors { get; set; }
    }
}
