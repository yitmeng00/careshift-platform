using ClinicalScheduler.Application.Common.Exceptions;
using ClinicalScheduler.Application.Common.Interfaces;
using ClinicalScheduler.Application.Shifts.Dtos;
using ClinicalScheduler.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicalScheduler.Application.Shifts.Commands.UpdateShift;

public class UpdateShiftCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateShiftCommand, ShiftDto>
{
    public async Task<ShiftDto> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await context.Shifts
            .Include(s => s.Staff)
            .Include(s => s.Department)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Shift), request.Id);

        var (startTime, endTime) = ShiftTimeHelper.Compute(request.NewDate, shift.ShiftType);

        var hasConflict = await context.Shifts.AnyAsync(
            s => s.StaffId == shift.StaffId &&
                 s.Id != shift.Id &&
                 s.StartTime < endTime &&
                 s.EndTime > startTime,
            cancellationToken);

        if (hasConflict)
            throw new ConflictException(
                $"{shift.Staff.FullName} already has a conflicting shift on {request.NewDate:d}.");

        shift.StartTime = startTime;
        shift.EndTime = endTime;

        await context.SaveChangesAsync(cancellationToken);

        return new ShiftDto(
            shift.Id,
            shift.StaffId,
            shift.Staff.FullName,
            shift.Staff.Initials,
            shift.DepartmentId,
            shift.Department.Name,
            shift.StartTime,
            shift.EndTime,
            shift.ShiftType.ToString(),
            shift.Notes);
    }
}
