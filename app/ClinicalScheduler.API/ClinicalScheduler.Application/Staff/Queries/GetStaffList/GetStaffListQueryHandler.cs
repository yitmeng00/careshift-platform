using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Personnel.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Personnel.Queries.GetStaffList;

public class GetStaffListQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetStaffListQuery, List<StaffSummaryDto>>
{
    public async Task<List<StaffSummaryDto>> Handle(GetStaffListQuery request, CancellationToken cancellationToken)
        => await context.Staff
            .Include(s => s.Department)
            .Where(s => s.IsActive)
            .OrderBy(s => s.FullName)
            .Select(s => new StaffSummaryDto(s.Id, s.FullName, s.Initials, s.DepartmentId, s.Department.Name))
            .ToListAsync(cancellationToken);
}
