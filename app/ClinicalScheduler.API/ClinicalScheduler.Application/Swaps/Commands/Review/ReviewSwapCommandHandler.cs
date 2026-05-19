using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Swaps.Dtos;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Swaps.Commands.Review;

public class ReviewSwapCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ReviewSwapCommand, SwapRequestDto>
{
    public async Task<SwapRequestDto> Handle(ReviewSwapCommand request, CancellationToken cancellationToken)
    {
        var swap = await context.ShiftSwapRequests
            .Include(s => s.Requester).ThenInclude(s => s.Department)
            .Include(s => s.Requestee).ThenInclude(s => s.Department)
            .Include(s => s.RequesterShift)
            .Include(s => s.RequesteeShift)
            .Include(s => s.AuditEntries)
            .FirstOrDefaultAsync(s => s.Id == request.SwapId, cancellationToken)
            ?? throw new NotFoundException("ShiftSwapRequest", request.SwapId);

        if (swap.Status != SwapStatus.PendingAdmin)
            throw new ConflictException("This swap request is not awaiting admin review.");

        if (request.Action == "Approve")
        {
            swap.Status = SwapStatus.Approved;

            // Swap the staff assignments on both shifts
            var requesterShift = await context.Shifts.FindAsync([swap.RequesterShiftId], cancellationToken)!;
            var requesteeShift = await context.Shifts.FindAsync([swap.RequesteeShiftId], cancellationToken)!;

            requesterShift!.StaffId = swap.RequesteeId;
            requesteeShift!.StaffId = swap.RequesterId;
        }
        else
        {
            swap.Status = SwapStatus.Rejected;
        }

        swap.AuditEntries.Add(new SwapAuditEntry
        {
            At = DateTime.UtcNow,
            By = request.CallerName,
            Action = request.Action == "Approve" ? "approved" : "rejected",
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