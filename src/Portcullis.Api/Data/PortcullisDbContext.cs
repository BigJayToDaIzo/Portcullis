using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Domain.Entities;

namespace Portcullis.Api.Data
{
  public class PortcullisDbContext(DbContextOptions<PortcullisDbContext> options)
    : DbContext(options)
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Domain.Entities.Secret> Secrets { get; set; }
    public DbSet<AuditLogEntry> AuditLogEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Domain.Entities.Secret>()
        .HasIndex(s => new { s.UserId, s.Name })
        .IsUnique();
      modelBuilder.Entity<Domain.Entities.Secret>()
        .HasOne<User>()
        .WithMany()
        .HasForeignKey(s => s.UserId);
    }
  }
}
