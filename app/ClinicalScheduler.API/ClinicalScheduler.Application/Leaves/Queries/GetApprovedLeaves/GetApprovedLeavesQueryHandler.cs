using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Leaves.Dtos;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Leaves.Queries.GetApprovedLeaves;

public class GetApprovedLeavesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetApprovedLeavesQuery, List<ApprovedLeaveStaffDto>>
{
    public async Task<List<ApprovedLeaveStaffDto>> Handle(
        GetApprovedLeavesQuery request, CancellationToken cancellationToken)
    {
        return await context.LeaveRequests
            .Where(l => l.Status == LeaveStatus.Approved &&
                        l.StartDate <= request.To &&
                        l.EndDate >= request.From)
            .Select(l => new ApprovedLeaveStaffDto(l.StaffId, l.StartDate, l.EndDate))
            .ToListAsync(cancellationToken);
    }
}