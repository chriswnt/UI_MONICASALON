using System.Globalization;
using System.Text.Json;
using Microsoft.Maui.Storage;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

/// <summary>
/// Persists the JWT pair and the signed-in user. Tokens use SecureStorage when
/// the platform supports it and fall back to Preferences on desktop hosts where
/// SecureStorage is not available in unpackaged builds.
/// </summary>
public sealed class TokenStore
{
    private const string AccessTokenKey = "monica.access_token";
    private const string RefreshTokenKey = "monica.refresh_token";
    private const string ExpiresAtKey = "monica.expires_at";
    private const string UserKey = "monica.user";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _loaded;

    public string? AccessToken { get; private set; }

    public string? RefreshToken { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public UserDto? User { get; private set; }

    public bool HasSession => !string.IsNullOrWhiteSpace(AccessToken);

    public bool IsExpired => ExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(1);

    public async Task EnsureLoadedAsync()
    {
        if (_loaded)
        {
            return;
        }

        await _gate.WaitAsync();

        try
        {
            if (_loaded)
            {
                return;
            }

            AccessToken = await GetSecretAsync(AccessTokenKey);
            RefreshToken = await GetSecretAsync(RefreshTokenKey);

            var expiresAt = Preferences.Default.Get(ExpiresAtKey, string.Empty);
            ExpiresAt = DateTimeOffset.TryParse(
                expiresAt,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var parsed)
                ? parsed
                : default;

            var userJson = Preferences.Default.Get(UserKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(userJson))
            {
                try
                {
                    User = JsonSerializer.Deserialize<UserDto>(userJson, JsonOptions);
                }
                catch (JsonException)
                {
                    User = null;
                }
            }

            _loaded = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        await EnsureLoadedAsync();
        return AccessToken;
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        await EnsureLoadedAsync();
        return RefreshToken;
    }

    public async Task SaveAsync(AuthResponse auth)
    {
        await _gate.WaitAsync();

        try
        {
            AccessToken = auth.AccessToken;
            RefreshToken = auth.RefreshToken;
            ExpiresAt = auth.ExpiresAt;
            User = auth.User;

            await SetSecretAsync(AccessTokenKey, auth.AccessToken);
            await SetSecretAsync(RefreshTokenKey, auth.RefreshToken);
            Preferences.Default.Set(ExpiresAtKey, auth.ExpiresAt.ToString("O", CultureInfo.InvariantCulture));
            Preferences.Default.Set(UserKey, JsonSerializer.Serialize(auth.User, JsonOptions));

            _loaded = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ClearAsync()
    {
        await _gate.WaitAsync();

        try
        {
            AccessToken = null;
            RefreshToken = null;
            ExpiresAt = default;
            User = null;

            RemoveSecret(AccessTokenKey);
            RemoveSecret(RefreshTokenKey);
            Preferences.Default.Remove(ExpiresAtKey);
            Preferences.Default.Remove(UserKey);

            _loaded = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    private static async Task<string?> GetSecretAsync(string key)
    {
        try
        {
            var value = await SecureStorage.Default.GetAsync(key);

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }
        catch (Exception)
        {
            // SecureStorage is not available on every desktop host.
        }

        var fallback = Preferences.Default.Get(key, string.Empty);
        return string.IsNullOrWhiteSpace(fallback) ? null : fallback;
    }

    private static async Task SetSecretAsync(string key, string value)
    {
        try
        {
            await SecureStorage.Default.SetAsync(key, value);
            return;
        }
        catch (Exception)
        {
            // Fall through to Preferences.
        }

        Preferences.Default.Set(key, value);
    }

    private static void RemoveSecret(string key)
    {
        try
        {
            SecureStorage.Default.Remove(key);
        }
        catch (Exception)
        {
            // Ignore, Preferences is removed below as well.
        }

        Preferences.Default.Remove(key);
    }
}
