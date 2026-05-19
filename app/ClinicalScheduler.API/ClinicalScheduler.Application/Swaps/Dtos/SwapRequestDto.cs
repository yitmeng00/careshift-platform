namespace ClinicalScheduler.Application.Swaps.Dtos;

public record SwapAuditEntryDto(
    DateTime At,
    string By,
    string Action,
    string? Note);

public record SwapRequestDto(
    int Id,
    int RequesterId,
    string RequesterName,
    string RequesterInitials,
    string RequesterDepartment,
    int RequesteeId,
    string RequesteeName,
    string RequesteeInitials,
    string RequesteeDepartment,
    int RequesterShiftId,
    DateOnly RequesterShiftDate,
    string RequesterShiftType,
    int RequesteeShiftId,
    DateOnly RequesteeShiftDate,
    string RequesteeShiftType,
    string Reason,
    string Status,
    DateTime SubmittedAt,
    List<SwapAuditEntryDto> AuditEntries);