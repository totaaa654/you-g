using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YouG.Application.Common.Interfaces;
using YouG.Application.Features.Availability.Jobs;
using YouG.Infrastructure.Auth;
using YouG.Infrastructure.BackgroundJobs;
using YouG.Infrastructure.Persistence;
using YouG.Infrastructure.Persistence.Repositories;

namespace YouG.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<IAvailabilityRuleRepository, AvailabilityRuleRepository>();
        services.AddScoped<IAvailabilityInstanceRepository, AvailabilityInstanceRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventTimeOptionRepository, EventTimeOptionRepository>();
        services.AddScoped<IEventLocationOptionRepository, EventLocationOptionRepository>();
        services.AddScoped<IEventTimeVoteRepository, EventTimeVoteRepository>();
        services.AddScoped<IEventLocationVoteRepository, EventLocationVoteRepository>();
        services.AddScoped<IEventAttendanceRepository, EventAttendanceRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddScoped<IRecurrenceMaterializationJob, RecurrenceMaterializationJob>();
        services.AddHostedService<RecurrenceMaterializationBackgroundService>();

        return services;
    }
}
