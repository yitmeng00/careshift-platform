using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Leaves.Commands.CancelLeave;

public class CancelLeaveCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CancelLeaveCommand>
{
    public async Task Handle(CancelLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await context.LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == request.LeaveRequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId);

        if (leave.StaffId != request.CallerStaffId)
            throw new UnauthorizedException("You can only cancel your own leave requests.");

        if (leave.Status != LeaveStatus.Pending)
            throw new ConflictException("Only pending leave requests can be cancelled.");

        context.LeaveRequests.Remove(leave);
        await context.SaveChangesAsync(cancellationToken);
    }
}