using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class GroupInviteLinkConfiguration : IEntityTypeConfiguration<GroupInviteLink>
{
    public void Configure(EntityTypeBuilder<GroupInviteLink> builder)
    {
        builder.ToTable("GroupInviteLinks");

        builder.Property(l => l.Code).IsRequired().HasMaxLength(12);
        builder.HasIndex(l => l.Code).IsUnique();

        builder.HasOne<Group>().WithMany().HasForeignKey(l => l.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(l => l.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
