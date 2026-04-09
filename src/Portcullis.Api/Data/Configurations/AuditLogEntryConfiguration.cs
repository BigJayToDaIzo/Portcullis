using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
  void IEntityTypeConfiguration<AuditLogEntry>.Configure(
    EntityTypeBuilder<AuditLogEntry> builder
  )
  {
    builder.HasOne<User>();
    builder.Property(a => a.Action)
      .HasConversion<string>();
  }
}
