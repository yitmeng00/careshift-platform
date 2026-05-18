using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Shifts.Dtos;
using ClinicalScheduler.Domain.Entities;
using ClinicalScheduler.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;



namespace ClinicalScheduler.Application.Shifts.Commands.CreateShift;

public class CreateShiftCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateShiftCommand, ShiftDto>
{
    public async Task<ShiftDto> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
    {
        var staff = await context.Staff
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == request.StaffId, cancellationToken)
            ?? throw new NotFoundException(nameof(Staff), request.StaffId);

        var shiftType = Enum.Parse<ShiftType>(request.ShiftType);
        var (startTime, endTime) = ShiftTimeHelper.Compute(request.Date, shiftType);

        var hasConflict = await context.Shifts.AnyAsync(
            s => s.StaffId == request.StaffId &&
                 s.StartTime < endTime &&
                 s.EndTime > startTime,
            cancellationToken);

        if (hasConflict)
            throw new ConflictException(
                $"{staff.FullName} already has a shift that overlaps with {request.ShiftType} on {request.Date:d}.");

        var isOnLeave = await context.LeaveRequests.AnyAsync(
            l => l.StaffId == request.StaffId &&
                 l.Status == LeaveStatus.Approved &&
                 l.StartDate <= request.Date &&
                 l.EndDate >= request.Date,
            cancellationToken);

        if (isOnLeave)
            throw new ConflictException(
                $"{staff.FullName} has an approved leave request on {request.Date:d}.");

        var shift = new Shift
        {
            StaffId = request.StaffId,
            DepartmentId = request.DepartmentId,
            StartTime = startTime,
            EndTime = endTime,
            ShiftType = shiftType,
            Notes = request.Notes,
        };

        context.Shifts.Add(shift);
        await context.SaveChangesAsync(cancellationToken);

        return new ShiftDto(
            shift.Id,
            staff.Id,
            staff.FullName,
            staff.Initials,
            shift.DepartmentId,
            staff.Department.Name,
            shift.StartTime,
            shift.EndTime,
            shift.ShiftType.ToString(),
            shift.Notes);
    }
}
