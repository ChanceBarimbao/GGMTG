namespace GGMTG.Server.Models.RequestBodies
{
    public record ProjectDetails
    {
        public int ProjId { get; set; }
        public string ProjName { get; set; }
        public DateTime ProjCreated {  get; set; }
        public DateTime ProjUpdate { get; set; }
    }
}
