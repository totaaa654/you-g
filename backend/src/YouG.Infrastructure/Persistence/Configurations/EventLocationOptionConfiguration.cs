using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class EventLocationOptionConfiguration : IEntityTypeConfiguration<EventLocationOption>
{
    public void Configure(EntityTypeBuilder<EventLocationOption> builder)
    {
        builder.ToTable("EventLocationOptions");

        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);

        builder.HasIndex(o => o.EventId);

        builder.HasOne<Event>().WithMany().HasForeignKey(o => o.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(o => o.ProposedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
