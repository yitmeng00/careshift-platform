using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Dashboard.Dtos;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetPendingLeaves;

public class GetPendingLeavesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPendingLeavesQuery, List<PendingLeaveDto>>
{
    public async Task<List<PendingLeaveDto>> Handle(
        GetPendingLeavesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.LeaveRequests
            .Include(l => l.Staff).ThenInclude(s => s.Department)
            .Where(l => l.Status == LeaveStatus.Pending);

        // Department leads see only their own department's requests
        if (request.Role != "Admin")
            query = query.Where(l => l.Staff.DepartmentId == request.DepartmentId);

        return await query
            .OrderBy(l => l.StartDate)
            .Select(l => new PendingLeaveDto(
                l.Id,
                l.Staff.FullName,
                l.Staff.Initials,
                l.Staff.Department.Name,
                l.LeaveType.ToString(),
                l.StartDate,
                l.EndDate,
                l.EndDate.DayNumber - l.StartDate.DayNumber + 1,
                l.Reason,
                l.SubmittedAt))
            .ToListAsync(cancellationToken);
    }
}
