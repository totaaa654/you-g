using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class EventLocationVoteConfiguration : IEntityTypeConfiguration<EventLocationVote>
{
    public void Configure(EntityTypeBuilder<EventLocationVote> builder)
    {
        builder.ToTable("EventLocationVotes");

        builder.HasIndex(v => new { v.EventLocationOptionId, v.UserId }).IsUnique();

        builder.HasOne<EventLocationOption>().WithMany()
            .HasForeignKey(v => v.EventLocationOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>().WithMany().HasForeignKey(v => v.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
