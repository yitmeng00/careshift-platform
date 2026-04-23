namespace ClinicalScheduler.Domain.Entities;

public class LeaveAuditEntry
{
    public int Id { get; set; }
    public int LeaveRequestId { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
    public string By { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Note { get; set; }

    public LeaveRequest LeaveRequest { get; set; } = null!;
}
