using EMark.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMark.DataAccess.Connection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>().HasOne<User>().WithMany().IsRequired();
        }
    }
}