namespace ClinicalScheduler.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Staff> Staff { get; set; } = [];
    public ICollection<Shift> Shifts { get; set; } = [];
}
