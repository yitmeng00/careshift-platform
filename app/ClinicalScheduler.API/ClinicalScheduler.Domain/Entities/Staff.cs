using ClinicalScheduler.Domain.Enums;

namespace ClinicalScheduler.Domain.Entities;

public class Staff
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public StaffRole Role { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Phone { get; set; }
    public string Initials { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Department Department { get; set; } = null!;
    public ICollection<Shift> Shifts { get; set; } = [];
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = [];
    public ICollection<AuditLog> AuditLogs { get; set; } = [];
}
