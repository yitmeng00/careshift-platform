using ClinicalScheduler.Application.Shifts.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Shifts.Commands.CreateShift;

public record CreateShiftCommand(
    int StaffId,
    int DepartmentId,
    DateOnly Date,
    string ShiftType,
    string? Notes) : IRequest<ShiftDto>;
