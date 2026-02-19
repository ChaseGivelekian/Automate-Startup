using dotenv.net;

namespace Automate_Startup.Services;

public static class DotEnvService
{
    public static void Load() => DotEnv.Load();

    public static string? GetValue(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);

        // Throws an error if the environment variable isn't set
        return string.IsNullOrEmpty(value) ? throw new InvalidOperationException($"Required environment variable '{key}' is not set or is empty.") : value;
    }

    public static string GetValueOrDefault(string key, string defaultValue)
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}