using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YouG.Application.Common.Interfaces;

namespace YouG.Infrastructure.BackgroundJobs;

/// <summary>
/// Timer-driven trigger for the recurrence sweep — a hosted BackgroundService now, swappable for
/// Hangfire later without touching IRecurrenceMaterializationJob or its callers
/// (docs/02-ARCHITECTURE.md Section 5.2). Runs once at startup, then every 24 hours, so the
/// rolling horizon keeps advancing even for users who never edit their rules again.
/// </summary>
public class RecurrenceMaterializationBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<RecurrenceMaterializationBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService<IRecurrenceMaterializationJob>();
                await job.RunFullSweepAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // A failed sweep shouldn't crash the whole API — log and retry on the next tick.
                logger.LogError(ex, "Recurrence materialization sweep failed");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
