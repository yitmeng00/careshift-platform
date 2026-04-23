using ClinicalScheduler.Domain.Enums;

namespace ClinicalScheduler.Domain.Entities;

public class Shift
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ShiftType ShiftType { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Staff Staff { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
