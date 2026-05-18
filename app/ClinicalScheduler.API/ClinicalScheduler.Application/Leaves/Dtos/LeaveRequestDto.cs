namespace ClinicalScheduler.Application.Leaves.Dtos;

public record LeaveAuditEntryDto(
    DateTime At,
    string By,
    string Action,
    string? Note);

public record LeaveRequestDto(
    int Id,
    int StaffId,
    string StaffFullName,
    string StaffInitials,
    string StaffDepartment,
    string LeaveType,
    DateOnly StartDate,
    DateOnly EndDate,
    int DurationDays,
    string Reason,
    string Status,
    string? ReviewNote,
    string? ReviewedBy,
    DateTime? ReviewedAt,
    DateTime SubmittedAt,
    List<LeaveAuditEntryDto> AuditEntries);

public record ApprovedLeaveStaffDto(int StaffId, DateOnly StartDate, DateOnly EndDate);