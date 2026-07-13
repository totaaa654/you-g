using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("DeviceTokens");

        builder.Property(t => t.Token).IsRequired();
        builder.HasIndex(t => t.Token).IsUnique();
        builder.HasIndex(t => t.UserId);

        builder.Property(t => t.Platform).HasConversion<short>();

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
