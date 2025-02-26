using Microsoft.EntityFrameworkCore;
using GGMTG.Server.Models;

namespace GGMTG.Server.Models
{
    public class Context : DbContext
    {
        public DbSet<Account> Accounts { get; set; }  // Existing DbSet for Account
        public DbSet<Card> Cards { get; set; }        // New DbSet for Card

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=C:\\Users\\Chance Barimbao\\Documents\\testMTG.db"); // Make sure to use your actual path or connection string
            }
        }

        // Optionally, you can override OnModelCreating to customize entity configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
                .ToTable("cards")  // Explicitly map the table to "cards"
                .Property(c => c.Id)
                .HasColumnType("TEXT") // Ensure UUID is stored as TEXT in SQLite
                .HasConversion(
                    v => v.ToString(),
                    v => Guid.Parse(v)); // Map Guid to/from string for SQLite

            modelBuilder.Entity<Card>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Set default value to current timestamp in the database
        }
    }
    // Additional configurations can go here, if needed.
}
