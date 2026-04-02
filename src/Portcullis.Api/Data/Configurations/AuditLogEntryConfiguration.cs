using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Domain.Entities;

class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
  void IEntityTypeConfiguration<AuditLogEntry>.Configure(
    Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AuditLogEntry> builder
  )
  {
    builder.HasOne<User>();
  }
}
