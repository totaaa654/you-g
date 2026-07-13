using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class EventTimeOptionConfiguration : IEntityTypeConfiguration<EventTimeOption>
{
    public void Configure(EntityTypeBuilder<EventTimeOption> builder)
    {
        builder.ToTable("EventTimeOptions");

        builder.HasIndex(o => o.EventId);

        builder.HasOne<Event>().WithMany().HasForeignKey(o => o.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(o => o.ProposedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
