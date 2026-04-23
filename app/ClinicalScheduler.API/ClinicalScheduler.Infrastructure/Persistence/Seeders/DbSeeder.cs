using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicalScheduler.Infrastructure.Persistence.Seeders;

public class DbSeeder(AppDbContext context, ILogger<DbSeeder> logger, IPasswordHasher passwordHasher)
{
    public async Task SeedAsync()
    {
        if (await context.Departments.AnyAsync())
        {
            logger.LogInformation("Database already seeded — skipping.");
            return;
        }

        logger.LogInformation("Seeding database...");

        var departments = new List<Department>
        {
            new() { Name = "Emergency",   Description = "Emergency department" },
            new() { Name = "Cardiology",  Description = "Cardiology department" },
            new() { Name = "Pediatrics",  Description = "Pediatrics department" },
            new() { Name = "ICU",         Description = "Intensive Care Unit" },
            new() { Name = "Front Desk",  Description = "Front desk and reception" },
            new() { Name = "Radiology",   Description = "Radiology department" },
            new() { Name = "Oncology",    Description = "Oncology department" },
        };

        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        var deptMap = departments.ToDictionary(d => d.Name, d => d.Id);

        var staffSeed = new[]
        {
            new { Name="Dr. Sarah Chen",  Role=StaffRole.Doctor,         Dept="Emergency",  Employment=EmploymentType.FullTime,  Email="s.chen@hospital.org",    Phone="555-0101", Password="password123" },
            new { Name="Dr. James Park",  Role=StaffRole.Doctor,         Dept="Cardiology", Employment=EmploymentType.FullTime,  Email="j.park@hospital.org",    Phone="555-0102", Password="password123" },
            new { Name="Dr. Priya Nair",  Role=StaffRole.Doctor,         Dept="Pediatrics", Employment=EmploymentType.PartTime,  Email="p.nair@hospital.org",    Phone="555-0103", Password="password123" },
            new { Name="Emma White",      Role=StaffRole.Nurse,          Dept="Emergency",  Employment=EmploymentType.FullTime,  Email="e.white@hospital.org",   Phone="555-0104", Password="password123" },
            new { Name="Luis Torres",     Role=StaffRole.Nurse,          Dept="ICU",        Employment=EmploymentType.FullTime,  Email="l.torres@hospital.org",  Phone="555-0105", Password="password123" },
            new { Name="Aisha Johnson",   Role=StaffRole.Nurse,          Dept="Pediatrics", Employment=EmploymentType.FullTime,  Email="a.johnson@hospital.org", Phone="555-0106", Password="password123" },
            new { Name="Kira Patel",      Role=StaffRole.Nurse,          Dept="Cardiology", Employment=EmploymentType.PartTime,  Email="k.patel@hospital.org",   Phone="555-0107", Password="password123" },
            new { Name="Mark Stevens",    Role=StaffRole.Receptionist,   Dept="Front Desk", Employment=EmploymentType.FullTime,  Email="m.stevens@hospital.org", Phone="555-0108", Password="password123" },
            new { Name="Lisa Wong",       Role=StaffRole.Receptionist,   Dept="Front Desk", Employment=EmploymentType.PartTime,  Email="l.wong@hospital.org",    Phone="555-0109", Password="password123" },
            new { Name="Dr. Marcus Kim",  Role=StaffRole.DepartmentLead, Dept="Emergency",  Employment=EmploymentType.FullTime,  Email="m.kim@hospital.org",     Phone="555-0110", Password="password123" },
            new { Name="Diane Foster",    Role=StaffRole.ChargeNurse,    Dept="ICU",        Employment=EmploymentType.FullTime,  Email="d.foster@hospital.org",  Phone="555-0111", Password="password123" },
            new { Name="Admin User",      Role=StaffRole.Admin,          Dept="Emergency",  Employment=EmploymentType.FullTime,  Email="admin@hospital.org",     Phone="555-0100", Password="admin123" },
        };

        var staffEntities = staffSeed.Select(s =>
        {
            var initials = string.Join("", s.Name.Split(' ').Where(w => w.Length > 0).Take(2).Select(w => w[0])).ToUpper();
            return new Staff
            {
                FullName = s.Name,
                Email = s.Email.ToLowerInvariant(),
                PasswordHash = passwordHasher.Hash(s.Password),
                Role = s.Role,
                DepartmentId = deptMap[s.Dept],
                Initials = initials,
                EmploymentType = s.Employment,
                Phone = s.Phone,
                IsActive = true,
            };
        }).ToList();

        context.Staff.AddRange(staffEntities);
        await context.SaveChangesAsync();

        var today = DateTime.UtcNow.Date;
        var staffMap = staffEntities.ToDictionary(s => s.FullName, s => s);

        var shifts = new List<Shift>
        {
            // Dr. Sarah Chen - Emergency
            new() { StaffId=staffMap["Dr. Sarah Chen"].Id, DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(0).AddHours(7),  EndTime=today.AddDays(0).AddHours(15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"].Id, DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(1).AddHours(7),  EndTime=today.AddDays(1).AddHours(15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"].Id, DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Afternoon, StartTime=today.AddDays(2).AddHours(15), EndTime=today.AddDays(2).AddHours(23) },
            new() { StaffId=staffMap["Dr. Sarah Chen"].Id, DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(4).AddHours(7),  EndTime=today.AddDays(4).AddHours(15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"].Id, DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(5).AddHours(7),  EndTime=today.AddDays(5).AddHours(15) },
            // Emma White - Emergency nurse
            new() { StaffId=staffMap["Emma White"].Id,     DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(0).AddHours(7),  EndTime=today.AddDays(0).AddHours(15) },
            new() { StaffId=staffMap["Emma White"].Id,     DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(1).AddHours(7),  EndTime=today.AddDays(1).AddHours(15) },
            new() { StaffId=staffMap["Emma White"].Id,     DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(2).AddHours(7),  EndTime=today.AddDays(2).AddHours(15) },
            new() { StaffId=staffMap["Emma White"].Id,     DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(3).AddHours(7),  EndTime=today.AddDays(3).AddHours(15) },
            new() { StaffId=staffMap["Emma White"].Id,     DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=today.AddDays(4).AddHours(7),  EndTime=today.AddDays(4).AddHours(15) },
            // Luis Torres - ICU night shifts
            new() { StaffId=staffMap["Luis Torres"].Id,    DepartmentId=deptMap["ICU"],        ShiftType=ShiftType.Night,    StartTime=today.AddDays(0).AddHours(23), EndTime=today.AddDays(1).AddHours(7) },
            new() { StaffId=staffMap["Luis Torres"].Id,    DepartmentId=deptMap["ICU"],        ShiftType=ShiftType.Night,    StartTime=today.AddDays(1).AddHours(23), EndTime=today.AddDays(2).AddHours(7) },
            new() { StaffId=staffMap["Luis Torres"].Id,    DepartmentId=deptMap["ICU"],        ShiftType=ShiftType.Night,    StartTime=today.AddDays(2).AddHours(23), EndTime=today.AddDays(3).AddHours(7) },
        };

        context.Shifts.AddRange(shifts);

        var leaveRequests = new List<LeaveRequest>
        {
            new()
            {
                StaffId = staffMap["Dr. James Park"].Id,
                LeaveType = LeaveType.Annual,
                StartDate = DateOnly.FromDateTime(today.AddDays(3)),
                EndDate = DateOnly.FromDateTime(today.AddDays(4)),
                Reason = "Family event",
                Status = LeaveStatus.Pending,
                SubmittedAt = DateTime.UtcNow.AddDays(-2),
                AuditEntries = [ new LeaveAuditEntry { At=DateTime.UtcNow.AddDays(-2), By="Dr. James Park", Action="submitted" } ],
            },
            new()
            {
                StaffId = staffMap["Emma White"].Id,
                LeaveType = LeaveType.Annual,
                StartDate = DateOnly.FromDateTime(today.AddDays(8)),
                EndDate = DateOnly.FromDateTime(today.AddDays(10)),
                Reason = "Vacation",
                Status = LeaveStatus.Pending,
                SubmittedAt = DateTime.UtcNow.AddDays(-3),
                AuditEntries = [ new LeaveAuditEntry { At=DateTime.UtcNow.AddDays(-3), By="Emma White", Action="submitted" } ],
            },
            new()
            {
                StaffId = staffMap["Aisha Johnson"].Id,
                LeaveType = LeaveType.Sick,
                StartDate = DateOnly.FromDateTime(today.AddDays(1)),
                EndDate = DateOnly.FromDateTime(today.AddDays(1)),
                Reason = "Flu symptoms",
                Status = LeaveStatus.Approved,
                SubmittedAt = DateTime.UtcNow.AddDays(-1),
                ReviewedById = staffMap["Admin User"].Id,
                ReviewedAt = DateTime.UtcNow.AddHours(-1),
                ReviewNote = "Cover arranged with Kira",
                AuditEntries =
                [
                    new LeaveAuditEntry { At=DateTime.UtcNow.AddDays(-1), By="Aisha Johnson", Action="submitted" },
                    new LeaveAuditEntry { At=DateTime.UtcNow.AddHours(-1), By="Admin User", Action="approved", Note="Cover arranged with Kira" },
                ],
            },
        };

        context.LeaveRequests.AddRange(leaveRequests);

        context.AuditLogs.AddRange([
            new AuditLog { StaffId=staffMap["Admin User"].Id, Action="Approved leave", EntityType="LeaveRequest", PerformedBy="Admin User", Detail="Aisha Johnson · Sick Leave", Icon="✓", Timestamp=DateTime.UtcNow.AddHours(-1) },
            new AuditLog { StaffId=staffMap["Admin User"].Id, Action="Database seeded", EntityType="System", PerformedBy="System", Detail="Initial seed completed", Icon="+", Timestamp=DateTime.UtcNow },
        ]);

        await context.SaveChangesAsync();

        logger.LogInformation("Database seeding completed.");
    }
}
