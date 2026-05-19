using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Swaps.Dtos;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Swaps.Commands.Respond;

public class RespondSwapCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RespondSwapCommand, SwapRequestDto>
{
    public async Task<SwapRequestDto> Handle(RespondSwapCommand request, CancellationToken cancellationToken)
    {
        var swap = await context.ShiftSwapRequests
            .Include(s => s.Requester).ThenInclude(s => s.Department)
            .Include(s => s.Requestee).ThenInclude(s => s.Department)
            .Include(s => s.RequesterShift)
            .Include(s => s.RequesteeShift)
            .Include(s => s.AuditEntries)
            .FirstOrDefaultAsync(s => s.Id == request.SwapId, cancellationToken)
            ?? throw new NotFoundException("ShiftSwapRequest", request.SwapId);

        if (swap.Status != SwapStatus.PendingRequestee)
            throw new ConflictException("This swap request is not awaiting your response.");

        if (swap.RequesteeId != request.CallerId)
            throw new ForbiddenException("Only the target staff member can respond to this request.");

        swap.Status = request.Action == "Accept" ? SwapStatus.PendingAdmin : SwapStatus.Rejected;

        swap.AuditEntries.Add(new SwapAuditEntry
        {
            At = DateTime.UtcNow,
            By = request.CallerName,
            Action = request.Action == "Accept" ? "accepted" : "rejected",
            Note = request.Note,
        });

        await context.SaveChangesAsync(cancellationToken);

        return ToDto(swap);
    }

    private static SwapRequestDto ToDto(ShiftSwapRequest s) => new(
        s.Id,
        s.RequesterId, s.Requester.FullName, s.Requester.Initials, s.Requester.Department.Name,
        s.RequesteeId, s.Requestee.FullName, s.Requestee.Initials, s.Requestee.Department.Name,
        s.RequesterShiftId, DateOnly.FromDateTime(s.RequesterShift.StartTime), s.RequesterShift.ShiftType.ToString(),
        s.RequesteeShiftId, DateOnly.FromDateTime(s.RequesteeShift.StartTime), s.RequesteeShift.ShiftType.ToString(),
        s.Reason, s.Status.ToString(), s.SubmittedAt,
        s.AuditEntries.OrderBy(e => e.At).Select(e => new SwapAuditEntryDto(e.At, e.By, e.Action, e.Note)).ToList());
}