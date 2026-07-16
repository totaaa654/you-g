namespace YouG.API.IntegrationTests;

/// <summary>
/// Shares one <see cref="IntegrationTestWebApplicationFactory"/> (and its one Testcontainers
/// Postgres instance) across every test class tagged <c>[Collection(Name)]</c> — starting a
/// fresh container per test class would work but is needlessly slow.
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebApplicationFactory>
{
    public const string Name = "Integration";
}
