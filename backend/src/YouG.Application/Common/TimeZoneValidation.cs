namespace YouG.Application.Common;

/// <summary>Shared IANA timezone validation — used by Register and UpdateProfile.</summary>
public static class TimeZoneValidation
{
    public static bool IsValidIanaTimeZone(string timeZoneId)
    {
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}
