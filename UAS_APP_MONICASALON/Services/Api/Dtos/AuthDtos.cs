namespace UAS_APP_MONICASALON.Services.Api.Dtos;

public sealed record RegisterRequest(
    string Name,
    string Email,
    string Phone,
    string Password);

public sealed record LoginRequest(
    string EmailOrUsername,
    string Password);

public sealed record RefreshRequest(string RefreshToken);

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    UserDto User);

public sealed record UserDto(
    long Id,
    string Name,
    string Email,
    string? Username,
    string Role);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
