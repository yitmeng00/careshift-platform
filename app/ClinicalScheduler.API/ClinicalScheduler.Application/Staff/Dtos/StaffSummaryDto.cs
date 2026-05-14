namespace ClinicalScheduler.Application.Personnel.Dtos;

public record StaffSummaryDto(
    int Id,
    string FullName,
    string Initials,
    int DepartmentId,
    string DepartmentName);
