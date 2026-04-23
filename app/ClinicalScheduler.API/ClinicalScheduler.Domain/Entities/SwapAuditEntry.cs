namespace ClinicalScheduler.Domain.Entities;

public class SwapAuditEntry
{
    public int Id { get; set; }
    public int ShiftSwapRequestId { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
    public string By { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Note { get; set; }

    public ShiftSwapRequest ShiftSwapRequest { get; set; } = null!;
}
