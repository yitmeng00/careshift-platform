namespace ClinicalScheduler.Application.Auth.Dtos;

public record LoginResponseDto(
    string AccessToken,
    int ExpiresIn,
    StaffProfileDto Staff);

public record StaffProfileDto(
    int Id,
    string FullName,
    string Email,
    string Role,
    string Department,
    string Initials);
