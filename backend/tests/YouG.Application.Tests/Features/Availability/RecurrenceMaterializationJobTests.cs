using YouG.Application.Features.Availability.Jobs;
using YouG.Application.Tests.Fakes;
using YouG.Domain.Entities;
using YouG.Domain.Enums;

namespace YouG.Application.Tests.Features.Availability;

public class RecurrenceMaterializationJobTests
{
    private static readonly DateTimeOffset Today = new(2026, 7, 13, 0, 0, 0, TimeSpan.Zero); // a Monday

    private static (RecurrenceMaterializationJob Job, FakeAvailabilityRuleRepository Rules, FakeAvailabilityInstanceRepository Instances)
        CreateJob()
    {
        var rules = new FakeAvailabilityRuleRepository();
        var instances = new FakeAvailabilityInstanceRepository();
        var job = new RecurrenceMaterializationJob(rules, instances, new FakeUnitOfWork(), new FakeDateTimeProvider(Today));
        return (job, rules, instances);
    }

    [Fact]
    public async Task RunForUserAsync_CreatesInstancesOnlyForMatchingDayOfWeek()
    {
        var (job, rules, instances) = CreateJob();
        var userId = Guid.CreateVersion7();

        rules.Rules.Add(new AvailabilityRule
        {
            UserId = userId,
            DayOfWeek = DayOfWeek.Monday, // matches "Today"
            Daypart = Daypart.Evening,
            Status = AvailabilityStatus.Available,
            EffectiveFrom = DateOnly.FromDateTime(Today.Date)
        });

        await job.RunForUserAsync(userId, CancellationToken.None);

        Assert.All(instances.Instances, i => Assert.Equal(DayOfWeek.Monday, i.Date.DayOfWeek));
        Assert.All(instances.Instances, i => Assert.Equal(AvailabilityStatus.Available, i.Status));
        // Inclusive 56-day horizon produces 9 Mondays (0, 7, 14, ..., 56 days out).
        Assert.Equal(9, instances.Instances.Count);
    }

    [Fact]
    public async Task RunForUserAsync_ManualOverride_IsNeverOverwritten()
    {
        var (job, rules, instances) = CreateJob();
        var userId = Guid.CreateVersion7();
        var today = DateOnly.FromDateTime(Today.Date);

        var rule = new AvailabilityRule
        {
            UserId = userId,
            DayOfWeek = DayOfWeek.Monday,
            Daypart = Daypart.Evening,
            Status = AvailabilityStatus.Available,
            EffectiveFrom = today
        };
        rules.Rules.Add(rule);

        // A manual override already sits on the rule's very first target date.
        instances.Instances.Add(new AvailabilityInstance
        {
            UserId = userId,
            Date = today,
            Daypart = Daypart.Evening,
            Status = AvailabilityStatus.Busy,
            SourceRuleId = null,
            UpdatedAt = Today
        });

        await job.RunForUserAsync(userId, CancellationToken.None);

        var overriddenSlot = instances.Instances.Single(i => i.Date == today);
        Assert.Equal(AvailabilityStatus.Busy, overriddenSlot.Status);
        Assert.Null(overriddenSlot.SourceRuleId);

        // The other 8 Mondays in the horizon should still have been generated normally.
        Assert.Equal(9, instances.Instances.Count);
    }

    [Fact]
    public async Task RunForUserAsync_InstanceFromSameRule_UpdatesWhenRuleStatusChanged()
    {
        var (job, rules, instances) = CreateJob();
        var userId = Guid.CreateVersion7();
        var today = DateOnly.FromDateTime(Today.Date);

        var rule = new AvailabilityRule
        {
            UserId = userId,
            DayOfWeek = DayOfWeek.Monday,
            Daypart = Daypart.Evening,
            Status = AvailabilityStatus.Available, // rule now says Available
            EffectiveFrom = today
        };
        rules.Rules.Add(rule);

        // Simulates a previous sweep run that generated Busy before the user edited the rule.
        instances.Instances.Add(new AvailabilityInstance
        {
            UserId = userId,
            Date = today,
            Daypart = Daypart.Evening,
            Status = AvailabilityStatus.Busy,
            SourceRuleId = rule.Id,
            UpdatedAt = Today.AddDays(-1)
        });

        await job.RunForUserAsync(userId, CancellationToken.None);

        var slot = instances.Instances.Single(i => i.Date == today);
        Assert.Equal(AvailabilityStatus.Available, slot.Status);
        Assert.Equal(9, instances.Instances.Count); // still just 9 Mondays, not duplicated
    }

    [Fact]
    public async Task RunForUserAsync_RespectsEffectiveFromAndEffectiveUntil()
    {
        var (job, rules, instances) = CreateJob();
        var userId = Guid.CreateVersion7();
        var today = DateOnly.FromDateTime(Today.Date);

        rules.Rules.Add(new AvailabilityRule
        {
            UserId = userId,
            DayOfWeek = DayOfWeek.Monday,
            Daypart = Daypart.Evening,
            Status = AvailabilityStatus.Available,
            EffectiveFrom = today.AddDays(14), // skip the first two Mondays
            EffectiveUntil = today.AddDays(28)  // stop after the fourth Monday (day 28)
        });

        await job.RunForUserAsync(userId, CancellationToken.None);

        Assert.All(instances.Instances, i => Assert.True(i.Date >= today.AddDays(14) && i.Date <= today.AddDays(28)));
        Assert.Equal(3, instances.Instances.Count); // Mondays at day 14, 21, 28
    }

    [Fact]
    public async Task RunForUserAsync_NoRules_DoesNothing()
    {
        var (job, _, instances) = CreateJob();

        await job.RunForUserAsync(Guid.CreateVersion7(), CancellationToken.None);

        Assert.Empty(instances.Instances);
    }

    [Fact]
    public async Task RunFullSweepAsync_MaterializesForEveryUserWithRules()
    {
        var (job, rules, instances) = CreateJob();
        var userA = Guid.CreateVersion7();
        var userB = Guid.CreateVersion7();
        var today = DateOnly.FromDateTime(Today.Date);

        rules.Rules.Add(new AvailabilityRule { UserId = userA, DayOfWeek = DayOfWeek.Monday, Daypart = Daypart.Morning, Status = AvailabilityStatus.Available, EffectiveFrom = today });
        rules.Rules.Add(new AvailabilityRule { UserId = userB, DayOfWeek = DayOfWeek.Monday, Daypart = Daypart.Night, Status = AvailabilityStatus.Maybe, EffectiveFrom = today });

        await job.RunFullSweepAsync(CancellationToken.None);

        Assert.Equal(9, instances.Instances.Count(i => i.UserId == userA));
        Assert.Equal(9, instances.Instances.Count(i => i.UserId == userB));
    }
}
