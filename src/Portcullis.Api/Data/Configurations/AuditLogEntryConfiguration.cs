using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portcullis.Api.Domain.Entities;

namespace Portcullis.Api.Data.Configurations;

class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    void IEntityTypeConfiguration<AuditLogEntry>.Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.HasOne<User>();
        builder.Property(a => a.Action).HasConversion<string>();
    }
}
