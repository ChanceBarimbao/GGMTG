using Microsoft.EntityFrameworkCore;
using GGMTG.Server.Models;

namespace GGMTG.Server.Models
{
    public class Context : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=yourdatabase.db");
            }
        }
    }
}
