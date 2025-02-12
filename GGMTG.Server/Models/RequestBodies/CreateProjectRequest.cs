using System.Text.Json.Serialization;

namespace GGMTG.Server.Models.RequestBodies
{
    /// <summary>
    /// The request body for Create Project Requests
    /// </summary>
    public record CreateProjectRequest
    {
        [JsonIgnore]
        public int? CustId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
