using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.ToTable("FriendRequests");

        builder.Property(fr => fr.Status).HasConversion<short>();

        builder.HasIndex(fr => new { fr.RequesterId, fr.AddresseeId }).IsUnique();
        builder.HasIndex(fr => new { fr.AddresseeId, fr.Status });

        builder.HasOne<User>().WithMany().HasForeignKey(fr => fr.RequesterId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(fr => fr.AddresseeId).OnDelete(DeleteBehavior.Restrict);
    }
}
