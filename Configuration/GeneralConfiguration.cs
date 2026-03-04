namespace Automate_Startup.Configuration;

public static class GeneralConfiguration
{
    public static TimeSpan RetryDurationMinutes { get; } = TimeSpan.FromMinutes(3);
    public static TimeSpan RetryDelaySeconds { get; } = TimeSpan.FromSeconds(10);
}