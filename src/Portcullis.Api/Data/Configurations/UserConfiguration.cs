using Microsoft.EntityFrameworkCore;
using Portcullis.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

class UserConfiguration : IEntityTypeConfiguration<User>
{
  void IEntityTypeConfiguration<User>.Configure(
    EntityTypeBuilder<User> builder
      )
  {
    // not never generated, never DBMS generated
    builder.Property(u => u.Id).ValueGeneratedNever(); 
  }
}
