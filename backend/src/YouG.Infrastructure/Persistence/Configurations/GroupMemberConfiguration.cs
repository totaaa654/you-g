using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence.Configurations;

public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable("GroupMembers");

        builder.Property(gm => gm.Role).HasConversion<short>();

        builder.HasIndex(gm => new { gm.GroupId, gm.UserId }).IsUnique();
        builder.HasIndex(gm => gm.UserId);

        builder.HasOne<Group>().WithMany().HasForeignKey(gm => gm.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(gm => gm.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
