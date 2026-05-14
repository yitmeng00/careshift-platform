namespace ClinicalScheduler.Application.Dashboard.Dtos;

public record TodayShiftDto(
    int Id,
    string StaffName,
    string StaffInitials,
    string DepartmentName,
    string ShiftType,
    DateTime StartTime,
    DateTime EndTime);
