//using Code_Blueprints.Server.Models.DiagramObjects;
using System.Text.Json.Serialization;

namespace GGMTG.Server.Models.RequestBodies
{
    /// <summary>
    /// The request body for Save Diagram Requests
    /// </summary>
    public record SaveDiagramRequest
    {
        [JsonIgnore]
        public int? CustId { get; set; }
        public int ProjId { get; set; }
        //public Diagram? Diagram { get; set; }
    }
}
