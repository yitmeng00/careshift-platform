using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Swaps.Dtos;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Swaps.Commands.Submit;

public class SubmitSwapCommandHandler(IApplicationDbContext context)
    : IRequestHandler<SubmitSwapCommand, SwapRequestDto>
{
    public async Task<SwapRequestDto> Handle(SubmitSwapCommand request, CancellationToken cancellationToken)
    {
        var requesterShift = await context.Shifts
            .Include(s => s.Staff).ThenInclude(s => s.Department)
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == request.RequesterShiftId, cancellationToken)
            ?? throw new NotFoundException("Shift", request.RequesterShiftId);

        if (requesterShift.StaffId != request.RequesterId)
            throw new ForbiddenException("You can only swap your own shifts.");

        var requesteeShift = await context.Shifts
            .Include(s => s.Staff).ThenInclude(s => s.Department)
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == request.RequesteeShiftId, cancellationToken)
            ?? throw new NotFoundException("Shift", request.RequesteeShiftId);

        if (requesteeShift.StaffId == request.RequesterId)
            throw new ConflictException("Cannot swap a shift with yourself.");

        var swap = new ShiftSwapRequest
        {
            RequesterId = request.RequesterId,
            RequesteeId = requesteeShift.StaffId,
            RequesterShiftId = request.RequesterShiftId,
            RequesteeShiftId = request.RequesteeShiftId,
            Reason = request.Reason,
            Status = SwapStatus.PendingRequestee,
        };

        swap.AuditEntries.Add(new SwapAuditEntry
        {
            At = DateTime.UtcNow,
            By = request.RequesterName,
            Action = "submitted",
        });

        context.ShiftSwapRequests.Add(swap);
        await context.SaveChangesAsync(cancellationToken);

        return new SwapRequestDto(
            swap.Id,
            swap.RequesterId,
            requesterShift.Staff.FullName,
            requesterShift.Staff.Initials,
            requesterShift.Staff.Department.Name,
            swap.RequesteeId,
            requesteeShift.Staff.FullName,
            requesteeShift.Staff.Initials,
            requesteeShift.Staff.Department.Name,
            swap.RequesterShiftId,
            DateOnly.FromDateTime(requesterShift.StartTime),
            requesterShift.ShiftType.ToString(),
            swap.RequesteeShiftId,
            DateOnly.FromDateTime(requesteeShift.StartTime),
            requesteeShift.ShiftType.ToString(),
            swap.Reason,
            swap.Status.ToString(),
            swap.SubmittedAt,
            swap.AuditEntries
                .Select(e => new SwapAuditEntryDto(e.At, e.By, e.Action, e.Note))
                .ToList());
    }
}