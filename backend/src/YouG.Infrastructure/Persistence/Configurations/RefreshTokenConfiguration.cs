using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(rt => rt.TokenHash).IsRequired();
        builder.HasIndex(rt => rt.TokenHash).IsUnique();
        builder.HasIndex(rt => rt.UserId);

        builder.HasOne<User>().WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);

        // Self-referencing rotation chain — restrict so revoking/rotating never cascades.
        builder.HasOne<RefreshToken>().WithMany()
            .HasForeignKey(rt => rt.ReplacedByTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
