using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class EventTimeVoteConfiguration : IEntityTypeConfiguration<EventTimeVote>
{
    public void Configure(EntityTypeBuilder<EventTimeVote> builder)
    {
        builder.ToTable("EventTimeVotes");

        builder.HasIndex(v => new { v.EventTimeOptionId, v.UserId }).IsUnique();

        builder.HasOne<EventTimeOption>().WithMany()
            .HasForeignKey(v => v.EventTimeOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>().WithMany().HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
