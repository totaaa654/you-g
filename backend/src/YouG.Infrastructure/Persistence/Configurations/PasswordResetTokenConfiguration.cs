using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");

        builder.Property(t => t.TokenHash).IsRequired();

        // No longer a uniqueness constraint on TokenHash: a 6-digit OTP-style code, unlike a
        // high-entropy blob, isn't collision-proof across different users' concurrent codes.
        // Lookups are scoped by UserId instead — see IPasswordResetTokenRepository.
        builder.HasIndex(t => t.UserId);

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
