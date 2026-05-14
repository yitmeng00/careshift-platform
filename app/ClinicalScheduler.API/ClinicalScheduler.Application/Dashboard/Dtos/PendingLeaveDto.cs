namespace ClinicalScheduler.Application.Dashboard.Dtos;

public record PendingLeaveDto(
    int Id,
    string StaffName,
    string StaffInitials,
    string DepartmentName,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    int DaysCount,
    string Reason,
    DateTime SubmittedAt);
