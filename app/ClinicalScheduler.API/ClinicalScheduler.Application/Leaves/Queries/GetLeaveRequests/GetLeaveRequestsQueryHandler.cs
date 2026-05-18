using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Leaves.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Leaves.Queries.GetLeaveRequests;

public class GetLeaveRequestsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetLeaveRequestsQuery, List<LeaveRequestDto>>
{
    public async Task<List<LeaveRequestDto>> Handle(
        GetLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = context.LeaveRequests
            .Include(l => l.Staff).ThenInclude(s => s.Department)
            .Include(l => l.ReviewedBy)
            .Include(l => l.AuditEntries)
            .AsQueryable();

        query = request.CallerRole switch
        {
            "Admin" or "DepartmentLead" => query,
            "ChargeNurse" => query.Where(l => l.Staff.DepartmentId == request.CallerDepartmentId),
            _ => query.Where(l => l.StaffId == request.CallerStaffId),
        };

        return await query
            .OrderByDescending(l => l.SubmittedAt)
            .Select(l => new LeaveRequestDto(
                l.Id,
                l.StaffId,
                l.Staff.FullName,
                l.Staff.Initials,
                l.Staff.Department.Name,
                l.LeaveType.ToString(),
                l.StartDate,
                l.EndDate,
                l.EndDate.DayNumber - l.StartDate.DayNumber + 1,
                l.Reason,
                l.Status.ToString(),
                l.ReviewNote,
                l.ReviewedBy != null ? l.ReviewedBy.FullName : null,
                l.ReviewedAt,
                l.SubmittedAt,
                l.AuditEntries
                    .OrderBy(e => e.At)
                    .Select(e => new LeaveAuditEntryDto(e.At, e.By, e.Action, e.Note))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}