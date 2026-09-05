using System.ComponentModel.DataAnnotations;

namespace ClinicalAppointmentSystem.Application.Authentication;

public sealed class GoogleSignInRequest
{
    [Required(ErrorMessage = "Google ID token is required.")]
    public string? IdToken { get; set; }
}
