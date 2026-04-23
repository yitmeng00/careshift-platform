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

        // Run all four counts concurrently
        var onDutyTask = context.Shifts
            .CountAsync(shift => shift.StartTime < tomorrow && shift.EndTime > today, cancellationToken);

        var pendingLeavesTask = context.LeaveRequests
            .CountAsync(leave => leave.Status == LeaveStatus.Pending, cancellationToken);

        // Each shift is 8 h; > 5 shifts in a week exceeds 40 h
        var overtimeTask = context.Shifts
            .Where(shift => shift.StartTime >= weekStart && shift.StartTime < weekEnd)
            .GroupBy(shift => shift.StaffId)
            .Where(group => group.Count() > 5)
            .CountAsync(cancellationToken);

        var activeStaffTask = context.Staff
            .CountAsync(staff => staff.IsActive, cancellationToken);

        await Task.WhenAll(onDutyTask, pendingLeavesTask, overtimeTask, activeStaffTask);

        return new DashboardStatsDto(
            OnDutyToday: onDutyTask.Result,
            PendingLeaves: pendingLeavesTask.Result,
            OvertimeAlerts: overtimeTask.Result,
            ActiveStaff: activeStaffTask.Result);
    }
}
