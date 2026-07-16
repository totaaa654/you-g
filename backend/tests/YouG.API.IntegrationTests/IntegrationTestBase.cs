using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.API.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    // The API serializes enums as strings (a global JsonStringEnumConverter registered in
    // Program.cs) - the test client needs the same converter, since System.Net.Http.Json's
    // per-call default options know nothing about the server's configuration.
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    protected IntegrationTestBase(IntegrationTestWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected HttpClient Client { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected Task<HttpResponseMessage> PostJsonAsync(string url, object? body) =>
        Client.PostAsJsonAsync(url, body, JsonOptions);

    protected Task<HttpResponseMessage> PutJsonAsync(string url, object? body) =>
        Client.PutAsJsonAsync(url, body, JsonOptions);

    protected static async Task<T> ReadAsAsync<T>(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;

    /// <summary>
    /// Registers a brand-new user with randomized email/username (so parallel/repeated test
    /// runs never collide on the shared database) and returns the real tokens the API issued.
    /// </summary>
    protected async Task<AuthResultDto> RegisterUserAsync(string? displayName = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..12];
        var response = await PostJsonAsync("/api/v1/auth/register", new
        {
            Email = $"{unique}@integration-test.local",
            Password = "P@ssw0rd123!",
            Username = $"user{unique}",
            DisplayName = displayName ?? "Integration Test User",
            TimeZoneId = "UTC",
        });

        response.EnsureSuccessStatusCode();
        return await ReadAsAsync<AuthResultDto>(response);
    }

    /// <summary>Attaches the given access token to every subsequent request on this client.</summary>
    protected void AuthenticateAs(AuthResultDto auth) =>
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

    /// <summary>Registers a fresh user and immediately authenticates the client as them.</summary>
    protected async Task<AuthResultDto> RegisterAndAuthenticateAsync(string? displayName = null)
    {
        var auth = await RegisterUserAsync(displayName);
        AuthenticateAs(auth);
        return auth;
    }
}
