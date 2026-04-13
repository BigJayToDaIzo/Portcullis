using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portcullis.Api.Domain.Entities;

namespace Portcullis.Api.Data.Configurations;

class SecretConfiguration : IEntityTypeConfiguration<Secret>
{
    void IEntityTypeConfiguration<Secret>.Configure(EntityTypeBuilder<Secret> builder)
    {
        builder.Property(s => s.Id).IsRequired();
        builder.HasIndex(s => new { s.UserId, s.Name }).IsUnique();
        builder.HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId);
    }
}
