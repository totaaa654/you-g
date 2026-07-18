namespace YouG.Infrastructure.Push;

public class FirebaseSettings
{
    public const string SectionName = "Firebase";

    /// <summary>Path to the Firebase Admin SDK service account JSON, relative to the content root.</summary>
    public string ServiceAccountPath { get; set; } = "firebase-service-account.json";
}
