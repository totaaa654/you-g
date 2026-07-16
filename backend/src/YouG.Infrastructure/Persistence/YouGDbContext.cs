using Microsoft.EntityFrameworkCore;
using YouG.Domain.Entities;

namespace YouG.Infrastructure.Persistence;

public class YouGDbContext(DbContextOptions<YouGDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<BlockedUser> BlockedUsers => Set<BlockedUser>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<GroupInviteLink> GroupInviteLinks => Set<GroupInviteLink>();
    public DbSet<GroupJoinRequest> GroupJoinRequests => Set<GroupJoinRequest>();
    public DbSet<AvailabilityRule> AvailabilityRules => Set<AvailabilityRule>();
    public DbSet<AvailabilityInstance> AvailabilityInstances => Set<AvailabilityInstance>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventTimeOption> EventTimeOptions => Set<EventTimeOption>();
    public DbSet<EventLocationOption> EventLocationOptions => Set<EventLocationOption>();
    public DbSet<EventTimeVote> EventTimeVotes => Set<EventTimeVote>();
    public DbSet<EventLocationVote> EventLocationVotes => Set<EventLocationVote>();
    public DbSet<EventAttendance> EventAttendances => Set<EventAttendance>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // citext gives Email/Username case-insensitive uniqueness at the DB layer (see docs/03-DATABASE.md).
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YouGDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
