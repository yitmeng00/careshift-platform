using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Dashboard.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetTodayShifts;

public class GetTodayShiftsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetTodayShiftsQuery, List<TodayShiftDto>>
{
    public async Task<List<TodayShiftDto>> Handle(
        GetTodayShiftsQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await context.Shifts
            .Include(s => s.Staff)
            .Include(s => s.Department)
            .Where(s => s.StartTime < tomorrow && s.EndTime > today)
            .OrderBy(s => s.StartTime)
            .Select(s => new TodayShiftDto(
                s.Id,
                s.Staff.FullName,
                s.Staff.Initials,
                s.Department.Name,
                s.ShiftType.ToString(),
                s.StartTime,
                s.EndTime))
            .ToListAsync(cancellationToken);
    }
}
