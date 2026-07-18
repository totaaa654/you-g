namespace YouG.Infrastructure.Email;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string SendGridApiKey { get; set; } = "";
    public string FromEmail { get; set; } = "noreply@youg.app";
    public string FromName { get; set; } = "You G?";
}
