using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Status).HasConversion<short>();

        builder.HasIndex(e => new { e.GroupId, e.Status });

        builder.HasOne<Group>().WithMany().HasForeignKey(e => e.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(e => e.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        // Restrict, not Cascade — EventTimeOption/EventLocationOption already cascade from Event.EventId,
        // so cascading here too would form a cycle (Event -> Option -> Event).
        builder.HasOne<EventTimeOption>().WithMany()
            .HasForeignKey(e => e.ConfirmedTimeOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<EventLocationOption>().WithMany()
            .HasForeignKey(e => e.ConfirmedLocationOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
