using System.Text.Json.Serialization;

namespace GGMTG.Server.Models.RequestBodies
{
    /// <summary>
    /// The request body for Rename Project Requests
    /// </summary>
    public record RenameProjectRequest
    {
        [JsonIgnore]
        public int? CustId { get; set; }
        public int ProjId { get; set; }
        public string NewName { get; set; } = string.Empty;
    }
}
