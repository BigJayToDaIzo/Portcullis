using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Domain.Entities;

namespace Portcullis.Api.Data
{
    public class PortcullisDbContext(DbContextOptions<PortcullisDbContext> options)
        : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Secret> Secrets { get; set; }
        public DbSet<AuditLogEntry> AuditLogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SecretConfiguration).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (
                var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
            )
            {
                if (entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                    entry.Property("UpdatedAt").CurrentValue = DateTimeOffset.UtcNow;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
