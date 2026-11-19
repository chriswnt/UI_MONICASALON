using UAS_APP_MONICASALON.Services.Api.Dtos;

namespace UAS_APP_MONICASALON.Services.Api;

public interface IAuthApi
{
    Task<AuthResponse> LoginAsync(
        string emailOrUsername,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthResponse> RegisterAsync(
        string name,
        string email,
        string phone,
        string password,
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetMeAsync(CancellationToken cancellationToken = default);

    Task LogoutAsync(CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}
