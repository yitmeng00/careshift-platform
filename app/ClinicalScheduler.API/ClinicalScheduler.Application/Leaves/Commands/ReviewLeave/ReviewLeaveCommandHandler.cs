using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Leaves.Dtos;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Leaves.Commands.ReviewLeave;

public class ReviewLeaveCommandHandler(IApplicationDbContext context)
    : IRequestHandler<ReviewLeaveCommand, LeaveRequestDto>
{
    public async Task<LeaveRequestDto> Handle(ReviewLeaveCommand request, CancellationToken cancellationToken)
    {
        var leave = await context.LeaveRequests
            .Include(l => l.Staff).ThenInclude(s => s.Department)
            .Include(l => l.ReviewedBy)
            .Include(l => l.AuditEntries)
            .FirstOrDefaultAsync(l => l.Id == request.LeaveRequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(LeaveRequest), request.LeaveRequestId);

        if (leave.Status != LeaveStatus.Pending)
            throw new ConflictException("Only pending leave requests can be reviewed.");

        leave.Status = request.Action == "Approve" ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leave.ReviewedById = request.ReviewerId;
        leave.ReviewedAt = DateTime.UtcNow;
        leave.ReviewNote = request.Note;

        leave.AuditEntries.Add(new LeaveAuditEntry
        {
            By = request.ReviewerName,
            Action = request.Action == "Approve" ? "approved" : "rejected",
            Note = request.Note,
        });

        await context.SaveChangesAsync(cancellationToken);

        return new LeaveRequestDto(
            leave.Id,
            leave.StaffId,
            leave.Staff.FullName,
            leave.Staff.Initials,
            leave.Staff.Department?.Name ?? string.Empty,
            leave.LeaveType.ToString(),
            leave.StartDate,
            leave.EndDate,
            leave.EndDate.DayNumber - leave.StartDate.DayNumber + 1,
            leave.Reason,
            leave.Status.ToString(),
            leave.ReviewNote,
            leave.ReviewedBy?.FullName,
            leave.ReviewedAt,
            leave.SubmittedAt,
            leave.AuditEntries.Select(e => new LeaveAuditEntryDto(e.At, e.By, e.Action, e.Note)).ToList());
    }
}