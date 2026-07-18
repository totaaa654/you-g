namespace YouG.Application.Common;

/// <summary>Lives in Application (not Infrastructure) because a handler — not just Infrastructure —
/// needs it to build a reset link. Bound from config in Infrastructure's DependencyInjection.</summary>
public class ClientUrlSettings
{
    public const string SectionName = "ClientUrls";

    public string ResetPasswordUrlBase { get; set; } = "http://localhost:5000/reset-password";
}
