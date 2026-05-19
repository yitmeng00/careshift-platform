using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Swaps.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Swaps.Queries.GetSwapRequests;

public class GetSwapRequestsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSwapRequestsQuery, List<SwapRequestDto>>
{
    public async Task<List<SwapRequestDto>> Handle(GetSwapRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = context.ShiftSwapRequests
            .Include(s => s.Requester)
            .Include(s => s.Requestee)
            .Include(s => s.RequesterShift)
            .Include(s => s.RequesteeShift)
            .Include(s => s.AuditEntries)
            .AsQueryable();

        query = request.CallerRole switch
        {
            "Admin" or "DepartmentLead" => query,
            "ChargeNurse" => query.Where(s =>
                s.Requester.DepartmentId == request.CallerDepartmentId ||
                s.Requestee.DepartmentId == request.CallerDepartmentId),
            _ => query.Where(s =>
                s.RequesterId == request.CallerStaffId ||
                s.RequesteeId == request.CallerStaffId),
        };

        return await query
            .OrderByDescending(s => s.SubmittedAt)
            .Select(s => new SwapRequestDto(
                s.Id,
                s.RequesterId,
                s.Requester.FullName,
                s.Requester.Initials,
                s.Requester.Department.Name,
                s.RequesteeId,
                s.Requestee.FullName,
                s.Requestee.Initials,
                s.Requestee.Department.Name,
                s.RequesterShiftId,
                DateOnly.FromDateTime(s.RequesterShift.StartTime),
                s.RequesterShift.ShiftType.ToString(),
                s.RequesteeShiftId,
                DateOnly.FromDateTime(s.RequesteeShift.StartTime),
                s.RequesteeShift.ShiftType.ToString(),
                s.Reason,
                s.Status.ToString(),
                s.SubmittedAt,
                s.AuditEntries
                    .OrderBy(e => e.At)
                    .Select(e => new SwapAuditEntryDto(e.At, e.By, e.Action, e.Note))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}