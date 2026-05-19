using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Swaps.Commands.Cancel;

public class CancelSwapCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CancelSwapCommand>
{
    public async Task Handle(CancelSwapCommand request, CancellationToken cancellationToken)
    {
        var swap = await context.ShiftSwapRequests
            .Include(s => s.AuditEntries)
            .FirstOrDefaultAsync(s => s.Id == request.SwapId, cancellationToken)
            ?? throw new NotFoundException("ShiftSwapRequest", request.SwapId);

        if (swap.RequesterId != request.CallerId)
            throw new ForbiddenException("Only the requester can cancel this swap.");

        if (swap.Status is not (SwapStatus.PendingRequestee or SwapStatus.PendingAdmin))
            throw new ConflictException("Only pending swap requests can be cancelled.");

        swap.Status = SwapStatus.Cancelled;
        swap.AuditEntries.Add(new SwapAuditEntry
        {
            At = DateTime.UtcNow,
            By = request.CallerName,
            Action = "cancelled",
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}