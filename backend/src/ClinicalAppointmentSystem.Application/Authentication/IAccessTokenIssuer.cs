namespace ClinicalAppointmentSystem.Application.Authentication;


public sealed record TokenSubject(
    string Subject,
    string Email,
    string DisplayName,
    string? PictureUrl);

public sealed record AccessToken(string Value, DateTime ExpiresAtUtc, int ExpiresInSeconds);

public interface IAccessTokenIssuer
{
    AccessToken Issue(TokenSubject subject);
}
