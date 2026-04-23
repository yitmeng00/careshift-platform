using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Dashboard.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Dashboard.Queries.GetRecentActivity;

public class GetRecentActivityQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetRecentActivityQuery, IReadOnlyList<ActivityEntryDto>>
{
    public async Task<IReadOnlyList<ActivityEntryDto>> Handle(
        GetRecentActivityQuery request,
        CancellationToken cancellationToken)
    {
        return await context.AuditLogs
            .OrderByDescending(log => log.Timestamp)
            .Take(request.Count)
            .Select(log => new ActivityEntryDto(
                log.Id,
                log.Action,
                log.EntityType,
                log.EntityId,
                log.Detail,
                log.Icon,
                log.PerformedBy,
                log.Timestamp))
            .ToListAsync(cancellationToken);
    }
}
