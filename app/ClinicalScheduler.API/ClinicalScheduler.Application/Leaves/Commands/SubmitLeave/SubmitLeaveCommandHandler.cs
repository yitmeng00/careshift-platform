using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Leaves.Dtos;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Leaves.Commands.SubmitLeave;

public class SubmitLeaveCommandHandler(IApplicationDbContext context)
    : IRequestHandler<SubmitLeaveCommand, LeaveRequestDto>
{
    public async Task<LeaveRequestDto> Handle(SubmitLeaveCommand request, CancellationToken cancellationToken)
    {
        var staff = await context.Staff
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == request.StaffId, cancellationToken)
            ?? throw new NotFoundException(nameof(Staff), request.StaffId);

        var overlap = await context.LeaveRequests.AnyAsync(
            l => l.StaffId == request.StaffId &&
                 l.Status != LeaveStatus.Rejected &&
                 l.StartDate <= request.EndDate &&
                 l.EndDate >= request.StartDate,
            cancellationToken);

        if (overlap)
            throw new ConflictException("You already have a leave request that overlaps with the selected dates.");

        var leaveType = Enum.Parse<LeaveType>(request.LeaveType);

        var leave = new LeaveRequest
        {
            StaffId = request.StaffId,
            LeaveType = leaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = LeaveStatus.Pending,
        };

        leave.AuditEntries.Add(new LeaveAuditEntry
        {
            By = staff.FullName,
            Action = "submitted",
        });

        context.LeaveRequests.Add(leave);
        await context.SaveChangesAsync(cancellationToken);

        return ToDto(leave, staff);
    }

    private static LeaveRequestDto ToDto(LeaveRequest leave, Staff staff) => new(
        leave.Id,
        staff.Id,
        staff.FullName,
        staff.Initials,
        staff.Department?.Name ?? string.Empty,
        leave.LeaveType.ToString(),
        leave.StartDate,
        leave.EndDate,
        leave.EndDate.DayNumber - leave.StartDate.DayNumber + 1,
        leave.Reason,
        leave.Status.ToString(),
        leave.ReviewNote,
        null,
        leave.ReviewedAt,
        leave.SubmittedAt,
        leave.AuditEntries.Select(e => new LeaveAuditEntryDto(e.At, e.By, e.Action, e.Note)).ToList());
}