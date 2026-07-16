using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class AvailabilityInstanceConfiguration : IEntityTypeConfiguration<AvailabilityInstance>
{
    public void Configure(EntityTypeBuilder<AvailabilityInstance> builder)
    {
        builder.ToTable("AvailabilityInstances");

        builder.Property(i => i.Status).HasConversion<short>();

        // Upsert target for the recurrence sweep, and unique-per-slot invariant.
        builder.HasIndex(i => new { i.UserId, i.Date, i.StartTime }).IsUnique();

        // Critical path for the Smart Time Finder overlap query (docs/03-DATABASE.md Section 4).
        builder.HasIndex(i => new { i.UserId, i.Date });

        builder.HasOne<User>().WithMany().HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Cascade);

        // Null = manual one-off override that survived the sweep; deleting a rule shouldn't delete instances.
        builder.HasOne<AvailabilityRule>().WithMany()
            .HasForeignKey(i => i.SourceRuleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
