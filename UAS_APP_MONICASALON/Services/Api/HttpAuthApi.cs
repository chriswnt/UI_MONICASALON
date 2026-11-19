using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public sealed class HttpAuthApi : IAuthApi
{
    private readonly ApiClient _client;
    private readonly TokenStore _tokens;

    public HttpAuthApi(ApiClient client, TokenStore tokens)
    {
        _client = client;
        _tokens = tokens;
    }

    public async Task<AuthResponse> LoginAsync(
        string emailOrUsername,
        string password,
        CancellationToken cancellationToken = default)
    {
        var auth = await _client.PostAnonymousAsync<AuthResponse>(
            "auth/login",
            new LoginRequest(emailOrUsername, password),
            cancellationToken);

        if (auth is null)
        {
            throw new ApiException(500, "Respons login tidak valid.");
        }

        await _tokens.SaveAsync(auth);
        return auth;
    }

    public async Task<AuthResponse> RegisterAsync(
        string name,
        string email,
        string phone,
        string password,
        CancellationToken cancellationToken = default)
    {
        var auth = await _client.PostAnonymousAsync<AuthResponse>(
            "auth/register",
            new RegisterRequest(name, email, phone, password),
            cancellationToken);

        if (auth is null)
        {
            throw new ApiException(500, "Respons pendaftaran tidak valid.");
        }

        await _tokens.SaveAsync(auth);
        return auth;
    }

    public Task<UserDto?> GetMeAsync(CancellationToken cancellationToken = default) =>
        _client.GetAsync<UserDto>("auth/me", cancellationToken);

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.PostAsync("auth/logout", null, cancellationToken);
        }
        finally
        {
            await _tokens.ClearAsync();
        }
    }

    public Task ChangePasswordAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync(
            "auth/change-password",
            new ChangePasswordRequest(currentPassword, newPassword),
            cancellationToken);
}
