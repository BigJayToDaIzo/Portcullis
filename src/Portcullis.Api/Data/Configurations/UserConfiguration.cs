using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portcullis.Api.Domain.Entities;

namespace Portcullis.Api.Data.Configurations;

class UserConfiguration : IEntityTypeConfiguration<User>
{
    void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
    {
        // not never generated, never DBMS generated
        builder.Property(u => u.Id).ValueGeneratedNever();
    }
}
