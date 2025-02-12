using Microsoft.EntityFrameworkCore;
using GGMTG.Server.Models;

namespace GGMTG.Server.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        //public DbSet<Project> Projects { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Project>()
        //        .HasOne(p => p.Account)
        //        .WithMany(b => b.Projects)
        //        .HasForeignKey(p => p.CustId)
        //        .HasPrincipalKey(b => b.CustId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}
    }
}
