using ClinicalScheduler.Application.Shifts.Dtos;
using MediatR;

namespace ClinicalScheduler.Application.Shifts.Commands.UpdateShift;

public record UpdateShiftCommand(int Id, DateOnly NewDate) : IRequest<ShiftDto>;
