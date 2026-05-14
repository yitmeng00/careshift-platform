namespace ClinicalScheduler.Application.Shifts.Dtos;

public record ShiftDto(
    int Id,
    int StaffId,
    string StaffName,
    string StaffInitials,
    int DepartmentId,
    string DepartmentName,
    DateTime StartTime,
    DateTime EndTime,
    string ShiftType,
    string? Notes);
