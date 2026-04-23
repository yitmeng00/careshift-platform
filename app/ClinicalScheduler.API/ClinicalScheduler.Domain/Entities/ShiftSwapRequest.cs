using ClinicalScheduler.Domain.Enums;

namespace ClinicalScheduler.Domain.Entities;

public class ShiftSwapRequest
{
    public int Id { get; set; }
    public int RequesterId { get; set; }
    public int RequesteeId { get; set; }
    public int RequesterShiftId { get; set; }
    public int RequesteeShiftId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public SwapStatus Status { get; set; } = SwapStatus.PendingRequestee;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Staff Requester { get; set; } = null!;
    public Staff Requestee { get; set; } = null!;
    public Shift RequesterShift { get; set; } = null!;
    public Shift RequesteeShift { get; set; } = null!;
    public ICollection<SwapAuditEntry> AuditEntries { get; set; } = [];
}
