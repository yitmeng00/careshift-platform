using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Shifts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Shifts.Queries.GetUpcomingShifts;

public class GetUpcomingShiftsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetUpcomingShiftsQuery, List<ShiftDto>>
{
    public async Task<List<ShiftDto>> Handle(GetUpcomingShiftsQuery request, CancellationToken cancellationToken)
    {
        var from = DateTime.UtcNow.Date;
        var to = from.AddDays(30);

        return await context.Shifts
            .Include(s => s.Staff)
            .Include(s => s.Department)
            .Where(s => s.StaffId == request.StaffId && s.StartTime >= from && s.StartTime < to)
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