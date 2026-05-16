namespace ClinicalScheduler.Application.Auth.Dtos;

public record LoginResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    StaffProfileDto Staff);