using System.Net;
using YouG.Application.Features.Auth.Dtos;

namespace YouG.API.IntegrationTests;

public class AuthControllerTests(IntegrationTestWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Register_NewUser_Returns201WithTokensAndUser()
    {
        var unique = Guid.NewGuid().ToString("N")[..12];

        var response = await PostJsonAsync("/api/v1/auth/register", new
        {
            Email = $"{unique}@integration-test.local",
            Password = "P@ssw0rd123!",
            Username = $"user{unique}",
            DisplayName = "New User",
            TimeZoneId = "UTC",
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await ReadAsAsync<AuthResultDto>(response);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        Assert.Equal("New User", result.User.DisplayName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var unique = Guid.NewGuid().ToString("N")[..12];
        var email = $"{unique}@integration-test.local";
        var registerRequest = new
        {
            Email = email,
            Password = "P@ssw0rd123!",
            Username = $"user{unique}",
            DisplayName = "First User",
            TimeZoneId = "UTC",
        };

        var first = await PostJsonAsync("/api/v1/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await PostJsonAsync("/api/v1/auth/register", registerRequest with { Username = $"user{Guid.NewGuid():N}"[..16] });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Login_CorrectCredentials_Returns200WithTokens()
    {
        var unique = Guid.NewGuid().ToString("N")[..12];
        var email = $"{unique}@integration-test.local";
        const string password = "P@ssw0rd123!";

        await PostJsonAsync("/api/v1/auth/register", new
        {
            Email = email,
            Password = password,
            Username = $"user{unique}",
            DisplayName = "Login Test User",
            TimeZoneId = "UTC",
        });

        var response = await PostJsonAsync("/api/v1/auth/login", new { Email = email, Password = password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await ReadAsAsync<AuthResultDto>(response);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var unique = Guid.NewGuid().ToString("N")[..12];
        var email = $"{unique}@integration-test.local";

        await PostJsonAsync("/api/v1/auth/register", new
        {
            Email = email,
            Password = "P@ssw0rd123!",
            Username = $"user{unique}",
            DisplayName = "Wrong Password User",
            TimeZoneId = "UTC",
        });

        var response = await PostJsonAsync("/api/v1/auth/login", new { Email = email, Password = "TotallyWrong123!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ValidToken_ReturnsNewTokenPair()
    {
        var auth = await RegisterUserAsync();

        var response = await PostJsonAsync("/api/v1/auth/refresh", new { RefreshToken = auth.RefreshToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await ReadAsAsync<AuthResultDto>(response);
        Assert.NotEqual(auth.RefreshToken, result.RefreshToken);
    }

    [Fact]
    public async Task Refresh_ReusedRotatedToken_Returns401()
    {
        var auth = await RegisterUserAsync();

        var firstRefresh = await PostJsonAsync("/api/v1/auth/refresh", new { RefreshToken = auth.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, firstRefresh.StatusCode);

        // The original token was rotated away by the first refresh - replaying it must fail.
        var secondRefresh = await PostJsonAsync("/api/v1/auth/refresh", new { RefreshToken = auth.RefreshToken });

        Assert.Equal(HttpStatusCode.Unauthorized, secondRefresh.StatusCode);
    }

    [Fact]
    public async Task Logout_ValidToken_RevokesIt()
    {
        var auth = await RegisterAndAuthenticateAsync();

        var logoutResponse = await PostJsonAsync("/api/v1/auth/logout", new { RefreshToken = auth.RefreshToken });
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        var refreshAfterLogout = await PostJsonAsync("/api/v1/auth/refresh", new { RefreshToken = auth.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, refreshAfterLogout.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutAuthentication_Returns401()
    {
        var response = await PostJsonAsync("/api/v1/auth/logout", new { RefreshToken = "irrelevant" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
