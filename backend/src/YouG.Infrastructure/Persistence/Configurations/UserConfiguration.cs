using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.Email).IsRequired().HasMaxLength(320).HasColumnType("citext");
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Username).IsRequired().HasMaxLength(32).HasColumnType("citext");
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.FriendCode).IsRequired().HasMaxLength(10);
        builder.HasIndex(u => u.FriendCode).IsUnique();

        builder.HasIndex(u => u.GoogleId).IsUnique();

        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.TimeZoneId).IsRequired().HasMaxLength(64);

        builder.Property(u => u.ThemePreference).HasConversion<short>().HasDefaultValue(ThemeMode.System);
        builder.Property(u => u.IsSearchable).HasDefaultValue(true);
        builder.Property(u => u.NotifyOnFriendRequest).HasDefaultValue(true);
        builder.Property(u => u.NotifyOnGroupInvite).HasDefaultValue(true);
        builder.Property(u => u.NotifyOnEventReminder).HasDefaultValue(true);
        builder.Property(u => u.NotifyOnScheduleUpdate).HasDefaultValue(true);
    }
}
