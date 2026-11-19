using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

/// <summary>
/// Single HTTP entry point for the Monica Salon API. Adds the bearer token,
/// refreshes it once when it is expired or rejected, and surfaces the API's
/// ProblemDetails message as an <see cref="ApiException"/>.
/// </summary>
public sealed class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _http;
    private readonly TokenStore _tokens;
    private readonly SemaphoreSlim _refreshGate = new(1, 1);

    public ApiClient(HttpClient http, TokenStore tokens)
    {
        _http = http;
        _tokens = tokens;
    }

    public Task<T?> GetAsync<T>(string path, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Get, path, null, authenticated: true, allowRefresh: true, ct);

    public Task<T?> PostAsync<T>(string path, object? body = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Post, path, body, authenticated: true, allowRefresh: true, ct);

    public Task PostAsync(string path, object? body = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Post, path, body, ct);

    public Task<T?> PutAsync<T>(string path, object? body = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Put, path, body, authenticated: true, allowRefresh: true, ct);

    public Task<T?> PostAnonymousAsync<T>(string path, object? body = null, CancellationToken ct = default) =>
        SendAsync<T>(HttpMethod.Post, path, body, authenticated: false, allowRefresh: false, ct);

    public async Task<byte[]> GetBytesAsync(string path, CancellationToken ct = default)
    {
        using var response = await SendCoreAsync(
            HttpMethod.Get,
            path,
            null,
            authenticated: true,
            allowRefresh: true,
            ct);

        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    /// <summary>
    /// Rotates the refresh token. Returns false (and clears the session) when
    /// the backend rejects the refresh token.
    /// </summary>
    public async Task<bool> RefreshTokensAsync(CancellationToken ct = default)
    {
        await _refreshGate.WaitAsync(ct);

        try
        {
            var refreshToken = await _tokens.GetRefreshTokenAsync();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, ApiConfig.BuildUrl("auth/refresh"))
            {
                Content = JsonContent(new RefreshRequest(refreshToken))
            };

            using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            if (!response.IsSuccessStatusCode)
            {
                await _tokens.ClearAsync();
                return false;
            }

            var payload = await response.Content.ReadAsStringAsync(ct);
            var auth = JsonSerializer.Deserialize<AuthResponse>(payload, JsonOptions);

            if (auth is null || string.IsNullOrWhiteSpace(auth.AccessToken))
            {
                await _tokens.ClearAsync();
                return false;
            }

            await _tokens.SaveAsync(auth);
            return true;
        }
        catch (Exception)
        {
            await _tokens.ClearAsync();
            return false;
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    private async Task<T?> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        bool authenticated,
        bool allowRefresh,
        CancellationToken ct)
    {
        using var response = await SendCoreAsync(method, path, body, authenticated, allowRefresh, ct);

        await EnsureSuccessAsync(response, ct);

        var payload = await response.Content.ReadAsStringAsync(ct);

        if (string.IsNullOrWhiteSpace(payload))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(payload, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new ApiException(500, $"Respons API tidak dapat dibaca. {ex.Message}");
        }
    }

    private async Task SendAsync(
        HttpMethod method,
        string path,
        object? body,
        CancellationToken ct)
    {
        using var response = await SendCoreAsync(method, path, body, authenticated: true, allowRefresh: true, ct);
        await EnsureSuccessAsync(response, ct);
    }

    private async Task<HttpResponseMessage> SendCoreAsync(
        HttpMethod method,
        string path,
        object? body,
        bool authenticated,
        bool allowRefresh,
        CancellationToken ct)
    {
        try
        {
            if (!authenticated)
            {
                using var anonymousRequest = CreateRequest(method, path, body);
                return await _http.SendAsync(anonymousRequest, HttpCompletionOption.ResponseHeadersRead, ct);
            }

            await _tokens.EnsureLoadedAsync();

            if (_tokens.HasSession &&
                _tokens.IsExpired &&
                !string.IsNullOrWhiteSpace(await _tokens.GetRefreshTokenAsync()))
            {
                await RefreshTokensAsync(ct);
            }

            using var request = CreateRequest(method, path, body);
            await AddAuthorizationAsync(request);

            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            if (allowRefresh &&
                response.StatusCode == HttpStatusCode.Unauthorized &&
                !string.IsNullOrWhiteSpace(await _tokens.GetRefreshTokenAsync()))
            {
                response.Dispose();

                if (await RefreshTokensAsync(ct))
                {
                    using var retry = CreateRequest(method, path, body);
                    await AddAuthorizationAsync(retry);

                    return await _http.SendAsync(retry, HttpCompletionOption.ResponseHeadersRead, ct);
                }

                using var finalRequest = CreateRequest(method, path, body);
                return await _http.SendAsync(finalRequest, HttpCompletionOption.ResponseHeadersRead, ct);
            }

            return response;
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new ApiException(0, "Request ke server timeout. Coba lagi sebentar lagi.");
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(0, $"Tidak bisa terhubung ke server. Periksa koneksi internet. ({ex.Message})");
        }
    }

    private async Task AddAuthorizationAsync(HttpRequestMessage request)
    {
        var token = await _tokens.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string path, object? body)
    {
        var request = new HttpRequestMessage(method, ApiConfig.BuildUrl(path));

        if (body is not null)
        {
            request.Content = JsonContent(body);
        }

        return request;
    }

    private static StringContent JsonContent(object body) =>
        new(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json");

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var status = (int)response.StatusCode;
        var message = status switch
        {
            401 => "Sesi kamu sudah berakhir. Silakan masuk lagi.",
            403 => "Kamu tidak punya akses ke fitur ini.",
            429 => "Terlalu banyak percobaan. Coba lagi sebentar lagi.",
            _ => $"Permintaan gagal ({status})."
        };

        try
        {
            var payload = await response.Content.ReadAsStringAsync(ct);

            if (!string.IsNullOrWhiteSpace(payload))
            {
                var problem = JsonSerializer.Deserialize<ProblemDetailsDto>(payload, JsonOptions);

                if (!string.IsNullOrWhiteSpace(problem?.Detail))
                {
                    message = problem.Detail;
                }
                else if (!string.IsNullOrWhiteSpace(problem?.Title))
                {
                    message = problem.Title;
                }
            }
        }
        catch (JsonException)
        {
            // Keep the status-based message.
        }

        throw new ApiException(status, message);
    }
}
