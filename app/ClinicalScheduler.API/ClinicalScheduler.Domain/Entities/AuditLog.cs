namespace ClinicalScheduler.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int? StaffId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Detail { get; set; }
    public string? Icon { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Staff? Staff { get; set; }
}
