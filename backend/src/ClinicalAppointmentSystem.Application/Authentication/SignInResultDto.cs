namespace ClinicalAppointmentSystem.Application.Authentication;

public sealed record AuthConfigDto(string GoogleClientId);

public sealed record SignedInUserDto(
    string Subject,
    string Email,
    string DisplayName,
    string? PictureUrl);

public sealed record SignInResultDto(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds,
    DateTime ExpiresAtUtc,
    SignedInUserDto User);
