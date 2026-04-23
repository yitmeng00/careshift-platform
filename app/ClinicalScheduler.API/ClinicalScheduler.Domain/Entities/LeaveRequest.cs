using ClinicalScheduler.Domain.Enums;

namespace ClinicalScheduler.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public LeaveType LeaveType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public int? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Staff Staff { get; set; } = null!;
    public Staff? ReviewedBy { get; set; }
    public ICollection<LeaveAuditEntry> AuditEntries { get; set; } = [];
}
