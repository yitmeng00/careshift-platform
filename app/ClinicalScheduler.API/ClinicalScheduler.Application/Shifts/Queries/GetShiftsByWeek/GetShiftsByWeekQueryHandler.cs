using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Shifts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Shifts.Queries.GetShiftsByWeek;

public class GetShiftsByWeekQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetShiftsByWeekQuery, List<ShiftDto>>
{
    public async Task<List<ShiftDto>> Handle(GetShiftsByWeekQuery request, CancellationToken cancellationToken)
    {
        DateTime from, to;

        if (request.MonthStart.HasValue)
        {
            from = request.MonthStart.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var daysInMonth = DateTime.DaysInMonth(request.MonthStart.Value.Year, request.MonthStart.Value.Month);
            to = from.AddDays(daysInMonth);
        }
        else
        {
            from = request.WeekStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            to = from.AddDays(7);
        }

        var query = context.Shifts
            .Include(s => s.Staff)
            .Include(s => s.Department)
            .Where(s => s.StartTime >= from && s.StartTime < to);

        if (request.DepartmentId.HasValue)
            query = query.Where(s => s.DepartmentId == request.DepartmentId.Value);

        return await query
            .OrderBy(s => s.StartTime)
            .Select(s => new ShiftDto(
                s.Id,
                s.StaffId,
                s.Staff.FullName,
                s.Staff.Initials,
                s.DepartmentId,
                s.Department.Name,
                s.StartTime,
                s.EndTime,
                s.ShiftType.ToString(),
                s.Notes))
            .ToListAsync(cancellationToken);
    }
}
