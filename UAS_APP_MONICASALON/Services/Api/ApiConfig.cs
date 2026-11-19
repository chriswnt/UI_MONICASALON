using System.Reflection;

namespace UAS_APP_MONICASALON.Services.Api;

/// <summary>
/// Reads the client configuration from the embedded `.env` file
/// (or `.env.example` when `.env` is not present). Environment variables
/// with the same name override the embedded values.
/// </summary>
public static class ApiConfig
{
    private const string ResourceName = "UAS_APP_MONICASALON.appconfig";

    private static readonly Dictionary<string, string> Values = Load();

    public static string BaseUrl { get; } = ResolveBaseUrl();

    public static string Prefix { get; } = NormalizePrefix(Get("API_PREFIX", "/api/v1"));

    public static int TimeoutSeconds { get; } = ParseInt(Get("API_TIMEOUT_SECONDS", "60"), 60);

    public static string EnvironmentName { get; } = Get("API_ENVIRONMENT", "Development");

    public static string BuildUrl(string path) =>
        $"{BaseUrl}{Prefix}/{path.TrimStart('/')}";

    private static string ResolveBaseUrl()
    {
        var key = "API_BASE_URL";

#if ANDROID
        key = "API_BASE_URL_ANDROID";
#endif

        var value = Get(key, string.Empty);

        if (string.IsNullOrWhiteSpace(value))
        {
            value = Get("API_BASE_URL", "http://localhost:5277");
        }

        return value.Trim().TrimEnd('/');
    }

    private static string NormalizePrefix(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return "/api/v1";
        }

        return "/" + prefix.Trim().Trim('/');
    }

    private static string Get(string key, string fallback)
    {
        var environmentValue = System.Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrWhiteSpace(environmentValue))
        {
            return environmentValue.Trim();
        }

        return Values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }

    private static int ParseInt(string value, int fallback) =>
        int.TryParse(value, out var parsed) && parsed > 0 ? parsed : fallback;

    private static Dictionary<string, string> Load()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var assembly = typeof(ApiConfig).Assembly;

        using var stream = assembly.GetManifestResourceStream(ResourceName)
                           ?? OpenFallbackResource(assembly);

        if (stream is null)
        {
            return result;
        }

        using var reader = new StreamReader(stream);
        while (reader.ReadLine() is { } rawLine)
        {
            var line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            var separator = line.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"').Trim('\'');

            if (key.Length > 0)
            {
                result[key] = value;
            }
        }

        return result;
    }

    private static Stream? OpenFallbackResource(Assembly assembly)
    {
        var name = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(resource => resource.EndsWith(".appconfig", StringComparison.OrdinalIgnoreCase));

        return name is null ? null : assembly.GetManifestResourceStream(name);
    }
}
