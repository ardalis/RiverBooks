using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverBooks.Users.Domain;

namespace RiverBooks.Users.Data;

public class UserStreetAddressConfiguration : IEntityTypeConfiguration<UserStreetAddress>
{
  public void Configure(EntityTypeBuilder<UserStreetAddress> builder)
  {
    builder.ToTable(nameof(UserStreetAddress));
    builder
      .Property(x => x.Id)
      .ValueGeneratedNever();

    builder.ComplexProperty(usa => usa.StreetAddress);
  }
}
