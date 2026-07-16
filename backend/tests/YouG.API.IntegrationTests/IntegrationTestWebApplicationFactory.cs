using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using YouG.Infrastructure.Persistence;

namespace YouG.API.IntegrationTests;

/// <summary>
/// Boots the real ASP.NET Core pipeline (`Program`) against a real, disposable Postgres
/// container instead of fakes — per the documented Phase 7 plan in docs/02-ARCHITECTURE.md
/// ("WebApplicationFactory + Testcontainers Postgres"). One instance is shared across every
/// test in the assembly via <see cref="IntegrationTestCollection"/> so the container only
/// starts once; individual tests avoid colliding by using randomized data (see
/// <see cref="IntegrationTestBase"/>), not by resetting the database between tests.
/// </summary>
public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("youg_test")
        .WithUsername("youg_test")
        .WithPassword("youg_test_password")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // The recurrence-materialization sweep would otherwise write to the same test
            // database on its own schedule, mid-test, for no reason any of these tests need.
            services.RemoveAll<IHostedService>();
        });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // Program.cs reads ConnectionStrings:Default and the Jwt:* section synchronously at
        // startup (before WebApplicationFactory's ConfigureWebHost/ConfigureAppConfiguration
        // override actually takes effect on a minimal-hosting-model Program) - environment
        // variables avoid that ordering pitfall entirely, since AddEnvironmentVariables() is
        // part of WebApplication.CreateBuilder's own setup, read fresh whenever the host builds.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _postgres.GetConnectionString());
        Environment.SetEnvironmentVariable("Jwt__Issuer", "youg-api-integration-tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "youg-app-integration-tests");
        Environment.SetEnvironmentVariable("Jwt__SigningKey", "integration-test-signing-key-needs-to-be-256-bits-minimum-length");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenLifetimeMinutes", "15");

        // Forces the host to build now (picking up the environment variables set above) rather
        // than lazily on the first HTTP request.
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<YouGDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}
