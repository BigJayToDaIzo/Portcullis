using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

class SecretConfiguration : IEntityTypeConfiguration<Secret>
{
  void IEntityTypeConfiguration<Secret>.Configure(
    EntityTypeBuilder<Secret> builder
  )
  {
    builder.Property(s => s.Id).IsRequired();
    builder.HasIndex(s => new { s.UserId, s.Name }).IsUnique();
    builder.HasOne<User>();
  }
}
