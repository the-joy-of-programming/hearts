using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<IdentityProviderMapping> IdentityProviderMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityProviderMapping>().HasKey(c=> new { c.IdentityProviderName, c.IdentityProviderSub });
            modelBuilder.Entity<User>().HasIndex(u => u.DisplayName);
            modelBuilder.Entity<User>().HasIndex(u => u.Email);
        }
    }
}