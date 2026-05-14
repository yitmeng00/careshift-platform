using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Dashboard.Dtos;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(
        GetDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // ISO week: Monday = day 0
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-daysFromMonday);
        var weekEnd = weekStart.AddDays(7);

        var onDutyToday = await context.Shifts
            .CountAsync(shift => shift.StartTime < tomorrow && shift.EndTime > today, cancellationToken);

        var pendingLeaves = await context.LeaveRequests
            .CountAsync(leave => leave.Status == LeaveStatus.Pending, cancellationToken);

        // Each shift is 8 h; > 5 shifts in a week = overtime
        var overtimeAlerts = await context.Shifts
            .Where(shift => shift.StartTime >= weekStart && shift.StartTime < weekEnd)
            .GroupBy(shift => shift.StaffId)
            .Select(g => new { Count = g.Count() })
            .CountAsync(x => x.Count > 5, cancellationToken);

        var activeStaff = await context.Staff
            .CountAsync(staff => staff.IsActive, cancellationToken);

        return new DashboardStatsDto(
            OnDutyToday: onDutyToday,
            PendingLeaves: pendingLeaves,
            OvertimeAlerts: overtimeAlerts,
            ActiveStaff: activeStaff);
    }
}
