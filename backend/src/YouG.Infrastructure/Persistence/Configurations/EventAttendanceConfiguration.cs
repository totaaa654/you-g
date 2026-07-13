using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class EventAttendanceConfiguration : IEntityTypeConfiguration<EventAttendance>
{
    public void Configure(EntityTypeBuilder<EventAttendance> builder)
    {
        builder.ToTable("EventAttendance");

        builder.Property(a => a.Status).HasConversion<short>();

        builder.HasIndex(a => new { a.EventId, a.UserId }).IsUnique();
        builder.HasIndex(a => a.UserId);

        builder.HasOne<Event>().WithMany().HasForeignKey(a => a.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
