using System.Text.Json.Serialization;

namespace GGMTG.Server.Models.RequestBodies
{
    /// <summary>
    /// Request body for Get Project Details request
    /// </summary>
    public record GetProjectRequest
    {
        [JsonIgnore]
        public int? CustId { get; set; }
        public int ProjId { get; set; }
    }
}