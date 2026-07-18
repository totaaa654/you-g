using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YouG.Application.Common;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Jobs;
using YouG.Infrastructure.Auth;
using YouG.Infrastructure.BackgroundJobs;
using YouG.Infrastructure.Email;
using YouG.Infrastructure.Persistence;
using YouG.Infrastructure.Persistence.Repositories;
using YouG.Infrastructure.Push;

namespace YouG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing 'ConnectionStrings:Default' configuration value.");

        services.AddDbContext<YouGDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IGroupInviteLinkRepository, GroupInviteLinkRepository>();
        services.AddScoped<IGroupJoinRequestRepository, GroupJoinRequestRepository>();
        services.AddScoped<IAvailabilityRuleRepository, AvailabilityRuleRepository>();
        services.AddScoped<IAvailabilityInstanceRepository, AvailabilityInstanceRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventTimeOptionRepository, EventTimeOptionRepository>();
        services.AddScoped<IEventLocationOptionRepository, EventLocationOptionRepository>();
        services.AddScoped<IEventTimeVoteRepository, EventTimeVoteRepository>();
        services.AddScoped<IEventLocationVoteRepository, EventLocationVoteRepository>();
        services.AddScoped<IEventAttendanceRepository, EventAttendanceRepository>();
        services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
        services.AddScoped<IBlockedUserRepository, BlockedUserRepository>();
        services.AddScoped<IDeviceTokenRepository, DeviceTokenRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddScoped<IRecurrenceMaterializationJob, RecurrenceMaterializationJob>();
        services.AddHostedService<RecurrenceMaterializationBackgroundService>();

        services.Configure<FirebaseSettings>(configuration.GetSection(FirebaseSettings.SectionName));
        AddPushNotifications(services, configuration, environment);

        services.Configure<ClientUrlSettings>(configuration.GetSection(ClientUrlSettings.SectionName));
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        AddEmail(services, configuration);

        return services;
    }

    private static void AddEmail(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>() ?? new EmailSettings();

        if (string.IsNullOrWhiteSpace(settings.SendGridApiKey))
        {
            // No provider configured (CI, integration tests, local dev before SendGrid is set up) —
            // emails are logged instead of sent, so the reset-password flow is still testable end to end.
            services.AddSingleton<IEmailSender, LoggingEmailSender>();
            return;
        }

        services.AddSingleton<IEmailSender, SendGridEmailSender>();
    }

    private static void AddPushNotifications(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var settings = configuration.GetSection(FirebaseSettings.SectionName).Get<FirebaseSettings>() ?? new FirebaseSettings();
        var serviceAccountPath = Path.IsPathRooted(settings.ServiceAccountPath)
            ? settings.ServiceAccountPath
            : Path.Combine(environment.ContentRootPath, settings.ServiceAccountPath);

        if (!File.Exists(serviceAccountPath))
        {
            // No credential on this machine (CI, integration tests, local dev before Firebase setup) —
            // in-app notifications still work, pushes are silently skipped.
            services.AddSingleton<IPushNotificationSender, NoOpPushNotificationSender>();
            return;
        }

        // Guard against re-creating the default app if this method somehow runs twice in one process
        // (e.g. a second host build in tests) — FirebaseApp.Create throws if a default instance already exists.
        if (FirebaseApp.DefaultInstance is null)
        {
            FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(serviceAccountPath) });
        }

        services.AddSingleton<IPushNotificationSender, FcmPushNotificationSender>();
    }
}
