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
        if (!await context.Departments.AnyAsync())
        {
            logger.LogInformation("Seeding database...");
            await SeedCoreDataAsync();
        }

        // Always ensure the current ISO week has shifts so the schedule is never empty.
        await EnsureCurrentWeekShiftsAsync();
    }

    private async Task SeedCoreDataAsync()
    {
        var departments = new List<Department>
        {
            new() { Name = "Emergency",  Description = "Emergency department" },
            new() { Name = "Cardiology", Description = "Cardiology department" },
            new() { Name = "Pediatrics", Description = "Pediatrics department" },
            new() { Name = "ICU",        Description = "Intensive Care Unit" },
            new() { Name = "Front Desk", Description = "Front desk and reception" },
            new() { Name = "Radiology",  Description = "Radiology department" },
            new() { Name = "Oncology",   Description = "Oncology department" },
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

        var staffMap = staffEntities.ToDictionary(s => s.FullName, s => s);

        var today = DateTime.UtcNow.Date;

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
                AuditEntries = [new LeaveAuditEntry { At = DateTime.UtcNow.AddDays(-2), By = "Dr. James Park", Action = "submitted" }],
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
                AuditEntries = [new LeaveAuditEntry { At = DateTime.UtcNow.AddDays(-3), By = "Emma White", Action = "submitted" }],
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
                    new LeaveAuditEntry { At = DateTime.UtcNow.AddDays(-1), By = "Aisha Johnson", Action = "submitted" },
                    new LeaveAuditEntry { At = DateTime.UtcNow.AddHours(-1), By = "Admin User", Action = "approved", Note = "Cover arranged with Kira" },
                ],
            },
        };

        context.LeaveRequests.AddRange(leaveRequests);
        await context.SaveChangesAsync();
        logger.LogInformation("Core data seeding completed.");
    }

    private async Task EnsureCurrentWeekShiftsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var monday = today.AddDays(-daysFromMonday);
        var sunday = monday.AddDays(7);

        if (await context.Shifts.AnyAsync(s => s.StartTime >= monday && s.StartTime < sunday))
        {
            logger.LogInformation("Shifts already exist for the current week — skipping shift seed.");
            return;
        }

        logger.LogInformation("Seeding shifts for week {WeekStart}...", monday.ToString("yyyy-MM-dd"));

        var staffMap = await context.Staff.ToDictionaryAsync(s => s.FullName, s => s.Id);
        var deptMap = await context.Departments.ToDictionaryAsync(d => d.Name, d => d.Id);

        // Helper: create DateTime in UTC for a given week-offset day and hour
        DateTime Slot(int dayOffset, int hour) => monday.AddDays(dayOffset).AddHours(hour);

        var shifts = new List<Shift>
        {
            // ── Emergency ─────────────────────────────────────────────────────────
            // Dr. Marcus Kim (DeptLead) – Mon/Tue/Wed Morning, Thu Afternoon
            new() { StaffId=staffMap["Dr. Marcus Kim"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(0,7),  EndTime=Slot(0,15) },
            new() { StaffId=staffMap["Dr. Marcus Kim"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(1,7),  EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Dr. Marcus Kim"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(2,7),  EndTime=Slot(2,15) },
            new() { StaffId=staffMap["Dr. Marcus Kim"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Afternoon, StartTime=Slot(3,15), EndTime=Slot(3,23) },
            // Dr. Sarah Chen – Mon-Fri Morning, Sat Afternoon
            new() { StaffId=staffMap["Dr. Sarah Chen"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(0,7),  EndTime=Slot(0,15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(1,7),  EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Afternoon, StartTime=Slot(2,15), EndTime=Slot(2,23) },
            new() { StaffId=staffMap["Dr. Sarah Chen"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(3,7),  EndTime=Slot(3,15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(4,7),  EndTime=Slot(4,15) },
            new() { StaffId=staffMap["Dr. Sarah Chen"],  DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Afternoon, StartTime=Slot(5,15), EndTime=Slot(5,23) },
            // Emma White – Mon/Tue/Wed/Thu/Fri Morning
            new() { StaffId=staffMap["Emma White"],      DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(0,7),  EndTime=Slot(0,15) },
            new() { StaffId=staffMap["Emma White"],      DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(1,7),  EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Emma White"],      DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(2,7),  EndTime=Slot(2,15) },
            new() { StaffId=staffMap["Emma White"],      DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(3,7),  EndTime=Slot(3,15) },
            new() { StaffId=staffMap["Emma White"],      DepartmentId=deptMap["Emergency"], ShiftType=ShiftType.Morning,   StartTime=Slot(4,7),  EndTime=Slot(4,15) },

            // ── Cardiology ────────────────────────────────────────────────────────
            // Dr. James Park – Tue/Wed/Thu Morning, Fri Afternoon
            new() { StaffId=staffMap["Dr. James Park"],  DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Morning,   StartTime=Slot(1,7),  EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Dr. James Park"],  DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Morning,   StartTime=Slot(2,7),  EndTime=Slot(2,15) },
            new() { StaffId=staffMap["Dr. James Park"],  DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Morning,   StartTime=Slot(3,7),  EndTime=Slot(3,15) },
            new() { StaffId=staffMap["Dr. James Park"],  DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Afternoon, StartTime=Slot(4,15), EndTime=Slot(4,23) },
            // Kira Patel – Mon/Wed/Fri Afternoon
            new() { StaffId=staffMap["Kira Patel"],      DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Afternoon, StartTime=Slot(0,15), EndTime=Slot(0,23) },
            new() { StaffId=staffMap["Kira Patel"],      DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Afternoon, StartTime=Slot(2,15), EndTime=Slot(2,23) },
            new() { StaffId=staffMap["Kira Patel"],      DepartmentId=deptMap["Cardiology"], ShiftType=ShiftType.Afternoon, StartTime=Slot(4,15), EndTime=Slot(4,23) },

            // ── ICU ───────────────────────────────────────────────────────────────
            // Luis Torres – Night shifts Mon–Fri
            new() { StaffId=staffMap["Luis Torres"],     DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Night, StartTime=Slot(0,23), EndTime=Slot(1,7) },
            new() { StaffId=staffMap["Luis Torres"],     DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Night, StartTime=Slot(1,23), EndTime=Slot(2,7) },
            new() { StaffId=staffMap["Luis Torres"],     DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Night, StartTime=Slot(2,23), EndTime=Slot(3,7) },
            new() { StaffId=staffMap["Luis Torres"],     DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Night, StartTime=Slot(3,23), EndTime=Slot(4,7) },
            new() { StaffId=staffMap["Luis Torres"],     DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Night, StartTime=Slot(4,23), EndTime=Slot(5,7) },
            // Diane Foster – Mon/Tue/Wed/Thu Morning
            new() { StaffId=staffMap["Diane Foster"],    DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Morning, StartTime=Slot(0,7), EndTime=Slot(0,15) },
            new() { StaffId=staffMap["Diane Foster"],    DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Morning, StartTime=Slot(1,7), EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Diane Foster"],    DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Morning, StartTime=Slot(2,7), EndTime=Slot(2,15) },
            new() { StaffId=staffMap["Diane Foster"],    DepartmentId=deptMap["ICU"], ShiftType=ShiftType.Morning, StartTime=Slot(3,7), EndTime=Slot(3,15) },

            // ── Pediatrics ────────────────────────────────────────────────────────
            // Dr. Priya Nair – Mon/Tue/Thu Morning (part-time)
            new() { StaffId=staffMap["Dr. Priya Nair"],  DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Morning, StartTime=Slot(0,7), EndTime=Slot(0,15) },
            new() { StaffId=staffMap["Dr. Priya Nair"],  DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Morning, StartTime=Slot(1,7), EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Dr. Priya Nair"],  DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Morning, StartTime=Slot(3,7), EndTime=Slot(3,15) },
            // Aisha Johnson – Mon/Tue/Wed/Fri Afternoon
            new() { StaffId=staffMap["Aisha Johnson"],   DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Afternoon, StartTime=Slot(0,15), EndTime=Slot(0,23) },
            new() { StaffId=staffMap["Aisha Johnson"],   DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Afternoon, StartTime=Slot(1,15), EndTime=Slot(1,23) },
            new() { StaffId=staffMap["Aisha Johnson"],   DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Afternoon, StartTime=Slot(2,15), EndTime=Slot(2,23) },
            new() { StaffId=staffMap["Aisha Johnson"],   DepartmentId=deptMap["Pediatrics"], ShiftType=ShiftType.Afternoon, StartTime=Slot(4,15), EndTime=Slot(4,23) },

            // ── Front Desk ────────────────────────────────────────────────────────
            // Mark Stevens – Mon–Fri Morning
            new() { StaffId=staffMap["Mark Stevens"],    DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Morning, StartTime=Slot(0,7), EndTime=Slot(0,15) },
            new() { StaffId=staffMap["Mark Stevens"],    DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Morning, StartTime=Slot(1,7), EndTime=Slot(1,15) },
            new() { StaffId=staffMap["Mark Stevens"],    DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Morning, StartTime=Slot(2,7), EndTime=Slot(2,15) },
            new() { StaffId=staffMap["Mark Stevens"],    DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Morning, StartTime=Slot(3,7), EndTime=Slot(3,15) },
            new() { StaffId=staffMap["Mark Stevens"],    DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Morning, StartTime=Slot(4,7), EndTime=Slot(4,15) },
            // Lisa Wong – Mon/Wed/Fri Afternoon (part-time)
            new() { StaffId=staffMap["Lisa Wong"],       DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Afternoon, StartTime=Slot(0,15), EndTime=Slot(0,23) },
            new() { StaffId=staffMap["Lisa Wong"],       DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Afternoon, StartTime=Slot(2,15), EndTime=Slot(2,23) },
            new() { StaffId=staffMap["Lisa Wong"],       DepartmentId=deptMap["Front Desk"], ShiftType=ShiftType.Afternoon, StartTime=Slot(4,15), EndTime=Slot(4,23) },
        };

        context.Shifts.AddRange(shifts);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} shifts for week starting {WeekStart}.", shifts.Count, monday.ToString("yyyy-MM-dd"));
    }
}
