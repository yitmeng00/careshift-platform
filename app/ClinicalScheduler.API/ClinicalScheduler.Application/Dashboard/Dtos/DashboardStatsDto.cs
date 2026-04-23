namespace ClinicalScheduler.Application.Dashboard.Dtos;

public record DashboardStatsDto(
    int OnDutyToday,
    int PendingLeaves,
    int OvertimeAlerts,
    int ActiveStaff);
